using System;
using System.Collections.Generic;

using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public static class ValueExtractionHelper
    {
        private static readonly Func<object, object, bool> defaultComparer = (o1, o2) =>
        {
            return o1.Equals(o2);
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

        public static bool TryGetValue(string source, SerializedProperty causer, out object value, out bool hasMixedValues)
        {
            return TryGetValue(source, causer, out value, out hasMixedValues, defaultComparer);
        }

        public static bool TryGetValue(string source, SerializedProperty causer, out object value, out bool hasMixedValues, Func<object, object, bool> nextValuesComparer)
        {
            value = null;
            hasMixedValues = false;
            var valueFound = false;
            var targetObjects = causer.serializedObject.targetObjects;
            for (var i = 0; i < targetObjects.Length; i++)
            {
                var declaringObject = causer.GetDeclaringObject(targetObjects[i]);
                if (TryGetValue(source, declaringObject, out var nextValue))
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
    }
}