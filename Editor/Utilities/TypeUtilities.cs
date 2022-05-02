using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    //TODO: refactor
    public static class TypeUtilities
    {
        public static readonly Dictionary<int, List<Type>> cachedTypes = new Dictionary<int, List<Type>>();
        public static readonly Dictionary<int, TypesGroupInfo> cachedInfo = new Dictionary<int, TypesGroupInfo>();
        public static readonly Dictionary<string, Type> managedReferenceTypes = new Dictionary<string, Type>();


        public static List<Type> GetTypes(Type parentType)
        {
            return GetTypes(new TypeConstraint(parentType));
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

        public static TypesGroupInfo GetGroupedInfo(Type parentType)
        {
            return GetGroupedInfo(new TypeConstraint(parentType), true, TypeGrouping.None);
        }

        public static TypesGroupInfo GetGroupedInfo(TypeConstraint constraint, bool addEmptyValue, TypeGrouping grouping)
        {
            var types = GetTypes(constraint);
            return new TypesGroupInfo(constraint, types, addEmptyValue, grouping);
        }

        public static TypesGroupInfo GetGroupedInfo(TypesGroupSettings settings)
        {
            var key = settings.GetHashCode();
            if (cachedInfo.TryGetValue(key, out var info))
            {
                return info;
            }
            else
            {
                var types = GetTypes(settings.Constraint);
                return cachedInfo[key] = new TypesGroupInfo(settings.Constraint, types, 
                    settings.AddEmptyValue, settings.Grouping);
            }
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