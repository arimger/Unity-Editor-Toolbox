using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

namespace Toolbox.Editor
{
    public static class TypeUtilities
    {
        private static readonly Dictionary<int, List<Type>> cachedTypes = new Dictionary<int, List<Type>>();


        public static List<Type> GetTypes(Type parentType)
        {
            var types = TypeCache.GetTypesDerivedFrom(parentType).ToList();
            for (var i = types.Count - 1; i >= 0; i--)
            {
                var type = types[i];
                //if (IsConstraintSatisfied(type))
                {
                    continue;
                }

                //types.RemoveAt(i);
            }

            return types;
        }
    }
}