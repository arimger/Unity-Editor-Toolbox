using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    public static class TypeUtilities
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

            var parts = managedReferenceFullTypeName.Split(' ');
            managedReferenceInstanceType = parts.Length == 2
                ? Type.GetType($"{parts[1]}, {parts[0]}") : null;
            if (managedReferenceInstanceType != null)
            {
                referenceTypesByNames[managedReferenceFullTypeName] = managedReferenceInstanceType;
                return true;
            }

            return false;
        }

        public static List<Type> FindTypes(TypeConstraintContext constraint)
        {
#if UNITY_2019_2_OR_NEWER
            var parentType = constraint.TargetType;
            var typesCache = TypeCache.GetTypesDerivedFrom(parentType);
            var typesList = typesCache.ToList();
            typesList.Add(parentType);
            for (var i = typesList.Count - 1; i >= 0; i--)
            {
                var type = typesList[i];
                if (constraint.IsSatisfied(type))
                {
                    continue;
                }

                typesList.RemoveAt(i);
            }
#else
            var typesList = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                typesList.AddRange(FindTypes(constraint, assembly));
            }
#endif
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
    }
}