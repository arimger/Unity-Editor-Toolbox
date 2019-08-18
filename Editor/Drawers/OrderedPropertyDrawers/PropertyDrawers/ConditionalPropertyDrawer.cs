using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    public sealed class ConditionalPropertyDrawer : OrderedPropertyDrawer<DrawIfAttribute>
    {
        /// <summary>
        /// Checks if provided property is valid to display.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static bool IsDrawable(SerializedProperty property, DrawIfAttribute attribute)
        {
            if (attribute == null) return true;
            var propertyToCheck = property.serializedObject.FindProperty(attribute.ComparedPropertyName);
            if (propertyToCheck == null)
            {
                Debug.LogError("Error: Property " + attribute.ComparedPropertyName + " not found.");
                return true;
            }

            switch (propertyToCheck.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return propertyToCheck.boolValue.Equals(attribute.ComparedValue);
                case SerializedPropertyType.Enum:
                    //TODO: handling flags
                    var index = Array.IndexOf(Enum.GetValues(attribute.ComparedValue.GetType()), attribute.ComparedValue);
                    return propertyToCheck.enumValueIndex.Equals(index);
                case SerializedPropertyType.String:
                    return propertyToCheck.stringValue.Equals(attribute.ComparedValue);
                case SerializedPropertyType.Integer:
                    return propertyToCheck.intValue.Equals(attribute.ComparedValue);
                case SerializedPropertyType.Float:
                    return propertyToCheck.floatValue.Equals(attribute.ComparedValue);
                default:
                    Debug.LogError("Error: " + propertyToCheck.type + " is not supported in DrawIfAttribute.");
                    return true;
            }
        }


        /// <summary>
        /// Drawer method handled by ancestor class.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        /// <param name="attribute"></param>
        public override void HandleProperty(SerializedProperty property, DrawIfAttribute attribute)
        {
            if (!IsDrawable(property, attribute)) return;
            base.HandleProperty(property, attribute);
        }
    }
}