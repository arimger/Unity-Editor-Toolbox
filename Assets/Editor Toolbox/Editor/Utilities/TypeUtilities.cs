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
        private static readonly Dictionary<int, TypesCachedCollection> cachedCollections = new Dictionary<int, TypesCachedCollection>();
        private static readonly Dictionary<int, TypesEditorCollection> editorCollections = new Dictionary<int, TypesEditorCollection>();

        private static readonly Dictionary<int, TypesGroupInfo> cachedInfo = new Dictionary<int, TypesGroupInfo>();
        private static readonly Dictionary<string, Type> referenceTypesByNames = new Dictionary<string, Type>();


        public static TypesCachedCollection GetCollection(Type parentType)
        {
            return GetCollection(new TypeConstraint(parentType));
        }

        public static TypesCachedCollection GetCollection(TypeConstraint constraint)
        {
            var key = constraint.GetHashCode();
            if (cachedCollections.TryGetValue(key, out var cachedCollection))
            {
                return cachedCollection;
            }

            var parentType = constraint.TargetType;
            if (parentType == null)
            {
                return new TypesCachedCollection();
            }

            var typesCache = TypeCache.GetTypesDerivedFrom(parentType);
            var typesList = typesCache.ToList();
            for (var i = typesList.Count - 1; i >= 0; i--)
            {
                var type = typesList[i];
                if (constraint.IsSatisfied(type))
                {
                    continue;
                }

                typesList.RemoveAt(i);
            }

            return cachedCollections[key] = new TypesCachedCollection(typesList);
        }

        [Obsolete]
        public static TypesGroupInfo GetGroupedInfo(Type parentType)
        {
            return GetGroupedInfo(new TypeConstraint(parentType), true, TypeGrouping.None);
        }

        [Obsolete]
        public static TypesGroupInfo GetGroupedInfo(TypeConstraint constraint, bool addEmptyValue, TypeGrouping grouping)
        {
            var types = GetCollection(constraint);
            return new TypesGroupInfo(constraint, types, addEmptyValue, grouping);
        }

        public static TypesGroupInfo GetGroupedInfo(TypeAppearance appearance)
        {
            var key = appearance.GetHashCode();
            if (cachedInfo.TryGetValue(key, out var info))
            {
                return info;
            }
            else
            {
                var types = GetCollection(appearance.Constraint);
                return cachedInfo[key] = new TypesGroupInfo(appearance.Constraint, types, 
                    appearance.AddEmptyValue, appearance.Grouping);
            }
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
    }
}