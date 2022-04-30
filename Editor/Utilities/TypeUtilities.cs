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
        private static readonly TypeConstraint defaultConstraint = new TypeConstraint(null);


        public static List<Type> GetTypes(Type parentType)
        {
            defaultConstraint.ApplyTarget(parentType);
            return GetTypes(defaultConstraint);
        }

        public static List<Type> GetTypes(TypeConstraint constraint)
        {
            var key = constraint.GetHashCode();
            if (cachedTypes.TryGetValue(key, out var types))
            {
                return types;
            }

            var parentType = constraint.TargetType;
            var collection = TypeCache.GetTypesDerivedFrom(parentType);
            types = collection.ToList();
            for (var i = types.Count - 1; i >= 0; i--)
            {
                var type = types[i];
                if (constraint.IsSatisfied(type))
                {
                    continue;
                }

                types.RemoveAt(i);
            }

            return cachedTypes[key] = types;
        }

        public static TypeCachedInfo GetCachedInfo(Type parentType)
        {
            var types = GetTypes(parentType);
            return new TypeCachedInfo(parentType, types);
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