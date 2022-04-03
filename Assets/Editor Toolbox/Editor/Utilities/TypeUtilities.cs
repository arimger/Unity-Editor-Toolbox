using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    public static class TypeUtilities
    {
        private static readonly Dictionary<int, List<Type>> cachedTypes = new Dictionary<int, List<Type>>();
        private static readonly Dictionary<string, Type> managedReferenceTypes = new Dictionary<string, Type>();


        public static bool IsConstraintSatisfied(Type type, bool allowAbstract, bool allowObsolete)
        {
            //NOTE: it's possible to strip out ConstructedGenericTypes, but they are considered valid for now
            if (!type.IsVisible || !type.IsClass)
            {
                return false;
            }

            return (allowAbstract || !type.IsAbstract) && (allowObsolete || !Attribute.IsDefined(type, typeof(ObsoleteAttribute)));
        }

        public static List<Type> GetTypes(Type parentType, bool allowAbstract, bool allowObsolete)
        {
            var collection = TypeCache.GetTypesDerivedFrom(parentType);
            var types = collection.ToList();
            for (var i = types.Count - 1; i >= 0; i--)
            {
                var type = types[i];
                if (IsConstraintSatisfied(type, allowAbstract, allowObsolete))
                {
                    continue;
                }

                types.RemoveAt(i);
            }

            return types;
        }

        public static TypeCachedInfo GetCachedInfo(Type parentType, bool allowAbstract, bool allowObsolete)
        {
            var types = GetTypes(parentType, allowAbstract, allowObsolete);
            return new TypeCachedInfo(types);
        }

        public static bool TryGetTypeFromManagedReferenceFullTypeName(string managedReferenceFullTypeName, out Type managedReferenceInstanceType)
        {
            if (managedReferenceTypes.TryGetValue(managedReferenceFullTypeName, out managedReferenceInstanceType))
            {
                return true;
            }

            var parts = managedReferenceFullTypeName.Split(' ');
            managedReferenceInstanceType = parts.Length == 2
                ? Type.GetType($"{parts[1]}, {parts[0]}") : null;
            if (managedReferenceInstanceType != null)
            {
                managedReferenceTypes[managedReferenceFullTypeName] = managedReferenceInstanceType;
                return true;
            }

            return false;
        }
    }
}