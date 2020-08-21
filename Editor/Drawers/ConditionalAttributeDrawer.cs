using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ConditionalAttribute))]
    public abstract class ConditionalAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected bool IsConditionMet(SerializedProperty property)
        {
            var conditionProperty = property.GetSibiling(PropertyToCheck);
            if (conditionProperty != null)
            {
                if (conditionProperty.propertyType == SerializedPropertyType.Boolean)
                {
                    var compareValue = CompareValue != null && CompareValue is bool ? (bool)CompareValue : true;
                    return conditionProperty.boolValue == compareValue;
                }
                else
                {
                    ToolboxEditorLog.AttributeUsageWarning(attribute, property, PropertyToCheck + " has to be boolean value property.");
                }
            }
            else
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property, PropertyToCheck + " does not exists.");
            }

            return true;
        }


        protected ConditionalAttribute Attribute => attribute as ConditionalAttribute;

        protected string PropertyToCheck => Attribute.PropertyToCheck;

        protected object CompareValue => Attribute.CompareValue;
    }
}