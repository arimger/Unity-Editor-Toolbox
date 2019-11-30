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
                Debug.LogError(GetType().Name + " - property " + attribute.ComparedPropertyName + " not found.");
                return true;
            }

            switch (propertyToCheck.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return propertyToCheck.boolValue.Equals(attribute.ComparedConditionValue);
                case SerializedPropertyType.String:
                    return propertyToCheck.stringValue.Equals(attribute.ComparedConditionValue);
                case SerializedPropertyType.Integer:
                    return propertyToCheck.intValue.Equals(attribute.ComparedConditionValue);
                case SerializedPropertyType.Float:
                    return propertyToCheck.floatValue.Equals(attribute.ComparedConditionValue);
                case SerializedPropertyType.Enum:
                    return propertyToCheck.intValue.Equals((int)attribute.ComparedConditionValue);
                default:
                    Debug.LogError(GetType().Name + " - " + propertyToCheck.type + " value type is not supported by ConditionAttributeDrawers.");
                    return true;
            }
        }
    }
}