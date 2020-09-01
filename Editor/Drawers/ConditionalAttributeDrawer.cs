using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ConditionalAttribute))]
    public abstract class ConditionalAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected bool IsConditionMet(SerializedProperty property)
        {
            var propertyName = Attribute.PropertyToCheck;
            var propertyValue = Attribute.CompareValue;

            var conditionProperty = property.GetSibiling(propertyName);
            if (conditionProperty != null)
            {
                if (conditionProperty.propertyType == SerializedPropertyType.Boolean)
                {
                    var compareValue = propertyValue != null && propertyValue is bool ? (bool)propertyValue : true;
                    return conditionProperty.boolValue == compareValue;
                }
                else
                {
                    ToolboxEditorLog.AttributeUsageWarning(attribute, property, propertyName + " has to be a boolean value property.");
                }
            }
            else
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property, propertyName + " does not exists.");
            }

            return true;
        }


        protected ConditionalAttribute Attribute => attribute as ConditionalAttribute;
    }
}