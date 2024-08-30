using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal.Types;

    public static class TypeUtility
    {
        internal static readonly Dictionary<int, TypesCachedCollection> cachedCollections = new Dictionary<int, TypesCachedCollection>();
        internal static readonly Dictionary<int, TypesEditorCollection> editorCollections = new Dictionary<int, TypesEditorCollection>();
        internal static readonly Dictionary<string, Type> referenceTypesByNames = new Dictionary<string, Type>();

        internal static void ClearCache()
        {
            cachedCollections.Clear();
            editorCollections.Clear();
            referenceTypesByNames.Clear();
        }

        internal static List<Type> GetDerviedTypesUsingAssemblies(Type parentType, Func<Type, Type> typeProcessor = null)
        {
            var typesList = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var derivedTypes = GetDerviedTypesUsingAssemblies(parentType, typeProcessor, assembly);
                typesList.AddRange(derivedTypes);
            }

            return typesList;
        }

        internal static List<Type> GetDerviedTypesUsingAssemblies(Type parentType, Func<Type, Type> typeProcessor, Assembly assembly)
        {
            var types = new List<Type>();
            var assemblyTypes = assembly.GetTypes();
            for (var i = 0; i < assemblyTypes.Length; i++)
            {
                var type = assemblyTypes[i];
                if (typeProcessor != null)
                {
                    type = typeProcessor(type);
                }

                if (type == null)
                {
                    continue;
                }

                if (!IsTypeAssignableFrom(parentType, type) || type == parentType)
                {
                    continue;
                }

                types.Add(type);
            }

            return types;
        }

        internal static List<Type> GetDerivedTypesUsingTypesCache(Type parentType, Func<Type, Type> typeProcessor)
        {
            var typesList = new List<Type>();
#if UNITY_2019_2_OR_NEWER
            var typesCache = TypeCache.GetTypesDerivedFrom(parentType);
            for (var i = 0; i < typesCache.Count; i++)
            {
                var derivedType = typesCache[i];
                if (typeProcessor != null)
                {
                    derivedType = typeProcessor(derivedType);
                }

                if (derivedType == null)
                {
                    continue;
                }

                typesList.Add(derivedType);
            }
#endif
            return typesList;
        }

        internal static List<Type> GetDerivedTypes(Type parentType, Func<Type, Type> typeProcessor)
        {
#if UNITY_2019_2_OR_NEWER
            return GetDerivedTypesUsingTypesCache(parentType, typeProcessor);
#else
            return GetDerviedTypesUsingAssemblies(parentType, typeProcessor);
#endif
        }

        internal static bool CanBeSourceForGenericTypes(Type type)
        {
            return type.IsGenericType && !type.IsGenericTypeDefinition;
        }

        public static TypesCachedCollection GetCollection(Type parentType)
        {
            return GetCollection(new TypeConstraintContext(parentType));
        }

        public static TypesCachedCollection GetCollection(TypeConstraintContext constraint)
        {
            var key = constraint.GetHashCode();
            if (cachedCollections.TryGetValue(key, out var collection))
            {
                return collection;
            }

            var parentType = constraint.TargetType;
            if (parentType == null)
            {
                return new TypesCachedCollection();
            }

            var typesList = FindTypes(constraint);
            return cachedCollections[key] = new TypesCachedCollection(typesList);
        }

        public static TypesEditorCollection GetCollection(TypeAppearanceContext appearance)
        {
            var key = appearance.GetHashCode();
            if (editorCollections.TryGetValue(key, out var collection))
            {
                return collection;
            }

            var types = GetCollection(appearance.Constraint);
            return editorCollections[key] = new TypesEditorCollection(types,
                appearance.AddEmptyValue, appearance.TypeGrouping);
        }

        public static bool TryGetTypeFromManagedReferenceFullTypeName(string managedReferenceFullTypeName, out Type managedReferenceInstanceType)
        {
            if (referenceTypesByNames.TryGetValue(managedReferenceFullTypeName, out managedReferenceInstanceType))
            {
                return true;
            }

            const int typeFormatParts = 2;

            var parts = managedReferenceFullTypeName.Split(new char[] { ' ' }, typeFormatParts, StringSplitOptions.None);
            managedReferenceInstanceType = parts.Length == typeFormatParts
                ? Type.GetType($"{parts[1]}, {parts[0]}") : null;
            if (managedReferenceInstanceType != null)
            {
                referenceTypesByNames[managedReferenceFullTypeName] = managedReferenceInstanceType;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finds all derived <see cref="Type"/>s using provided <see cref="TypeConstraintContext"/>.
        /// </summary>
        public static List<Type> FindTypes(TypeConstraintContext constraint)
        {
            List<Type> typesList;
            var parentType = constraint.TargetType;
            //NOTE: if parent type is generic and has non-generic arguments then we can try to create derived generic types with the same constraints
            if (CanBeSourceForGenericTypes(parentType))
            {
                var parentGenericType = parentType.GetGenericTypeDefinition();
                var parentGenericArgs = parentGenericType.GetGenericArguments();
                typesList = GetDerivedTypes(parentGenericType, (sourceType) =>
                {
                    Type targetType;
                    //NOTE: type is a generic definition, it means we can check if constraints are matched
                    if (sourceType.IsGenericTypeDefinition)
                    {
                        var foundGenericArgs = sourceType.GetGenericArguments();
                        if (foundGenericArgs.Length != parentGenericArgs.Length)
                        {
                            return null;
                        }

                        try
                        {
                            targetType = sourceType.MakeGenericType(parentType.GenericTypeArguments);
                        }
                        catch (ArgumentException)
                        {
                            //NOTE: unfortunately, this is the easiest way to check if all generic constraints are ok
                            return null;
                        }
                    }
                    else
                    {
                        targetType = sourceType;
                    }

                    //NOTE: we need to check inheritance since all processed types are derived from the generic type definition
                    if (!IsTypeAssignableFrom(parentType, targetType))
                    {
                        return null;
                    }

                    return constraint.IsSatisfied(targetType) ? targetType : null;
                });
            }
            else
            {
                typesList = GetDerivedTypes(parentType, (sourceType) =>
                {
                    return constraint.IsSatisfied(sourceType) ? sourceType : null;
                });
            }

            if (constraint.IsSatisfied(parentType))
            {
                typesList.Add(parentType);
            }

            if (constraint.IsOrdered)
            {
                var comparer = constraint.Comparer;
                typesList.Sort(comparer);
            }

            return typesList;
        }

        public static List<Type> FindTypes(TypeConstraintContext constraint, Assembly assembly)
        {
            var types = new List<Type>();
            foreach (var type in assembly.GetTypes())
            {
                if (!constraint.IsSatisfied(type))
                {
                    continue;
                }

                types.Add(type);
            }

            return types;
        }

        public static bool IsTypeAssignableFrom(Type parentType, Type type)
        {
            return parentType.IsGenericTypeDefinition
                ? parentType.IsAssignableFromGenericDefinition(type)
                : parentType.IsAssignableFrom(type);
        }
    }
}