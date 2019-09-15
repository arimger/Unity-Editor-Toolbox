using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ConditionAttributeDrawer<T> : ToolboxConditionDrawer<T> where T : ConditionAttribute
    {
        protected bool IsConditionMet(SerializedProperty property, T attribute)
        {
            if (attribute == null) return true;
            var propertyToCheck = property.serializedObject.FindProperty(attribute.ComparedPropertyName);
            if (propertyToCheck == null)
            {
                Debug.LogError("Error - property " + attribute.ComparedPropertyName + " not found.");
                return true;
            }

            switch (propertyToCheck.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return propertyToCheck.boolValue.Equals(attribute.ComparedConditionValue);
                case SerializedPropertyType.Enum:
                    //TODO: handling flags
                    var index = Array.IndexOf(Enum.GetValues(attribute.ComparedConditionValue.GetType()), attribute.ComparedConditionValue);
                    return propertyToCheck.enumValueIndex.Equals(index);
                case SerializedPropertyType.String:
                    return propertyToCheck.stringValue.Equals(attribute.ComparedConditionValue);
                case SerializedPropertyType.Integer:
                    return propertyToCheck.intValue.Equals(attribute.ComparedConditionValue);
                case SerializedPropertyType.Float:
                    return propertyToCheck.floatValue.Equals(attribute.ComparedConditionValue);
                default:
                    Debug.LogError("Error - " + propertyToCheck.type + " value type is not supported by ConditionAttributeDrawers.");
                    return true;
            }
        }
    }
}