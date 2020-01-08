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
                ToolboxEditorLog.PropertyNotFoundWarning(property, attribute.ComparedPropertyName);
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
                    ToolboxEditorLog.TypeNotSupportedWarning(property, propertyToCheck.type);
                    return true;
            }
        }
    }
}