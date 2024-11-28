using System;
using System.Collections.Generic;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public static class ValueExtractionHelper
    {
        private static readonly Func<object, object, bool> defaultComparer = (o1, o2) =>
        {
            return o1 == o2;
        };

        private static readonly List<IValueExtractor> extractors = new List<IValueExtractor>()
        {
            new FieldValueExtractor(),
            new PropertyValueExtractor(),
            new MethodValueExtractor()
        };

        public static bool TryGetValue(string source, object declaringObject, out object value)
        {
            for (var i = 0; i < extractors.Count; i++)
            {
                if (extractors[i].TryGetValue(source, declaringObject, out value))
                {
                    return true;
                }
            }

            value = null;
            return false;
        }

        public static bool TryGetValue(string source, object[] parentObjects, out object value, out bool hasMixedValues)
        {
            return TryGetValue(source, parentObjects, out value, out hasMixedValues, defaultComparer);
        }

        public static bool TryGetValue(string source, object[] parentObjects, out object value, out bool hasMixedValues, Func<object, object, bool> nextValuesComparer)
        {
            value = null;
            hasMixedValues = false;
            var valueFound = false;
            for (var i = 0; i < parentObjects.Length; i++)
            {
                if (TryGetValue(source, parentObjects[i], out var nextValue))
                {
                    if (valueFound && !nextValuesComparer(value, nextValue))
                    {
                        hasMixedValues = true;
                        break;
                    }

                    value = nextValue;
                    valueFound = true;
                }
            }

            return valueFound;
        }

        public static bool TryGetValue(string source, SerializedProperty causer, out object value, out bool hasMixedValues)
        {
            return TryGetValue(source, causer, out value, out hasMixedValues, defaultComparer);
        }

        public static bool TryGetValue(string source, SerializedProperty causer, out object value, out bool hasMixedValues, Func<object, object, bool> nextValuesComparer)
        {
            //NOTE: consider using NonAlloc implementation
            var parentObjects = causer.GetDeclaringObjects();
            return TryGetValue(source, parentObjects, out value, out hasMixedValues, nextValuesComparer);
        }
    }
}