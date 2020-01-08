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
                    LogWarning(property, attribute, PropertyToCheck + " has to be boolean value property.");
                }
            }
            else
            {
                LogWarning(property, attribute, PropertyToCheck + " does not exists.");
            }

            return true;
        }


        protected ConditionalAttribute Attribute => attribute as ConditionalAttribute;

        protected string PropertyToCheck => Attribute.PropertyToCheck;

        protected object CompareValue => Attribute.CompareValue;
    }
}