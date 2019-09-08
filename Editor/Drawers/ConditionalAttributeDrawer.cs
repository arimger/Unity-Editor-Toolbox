using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalAttribute))]
    public abstract class ConditionalAttributeDrawer : PropertyDrawer
    {
        protected bool IsConditionMet(SerializedProperty property)
        {
            if (!string.IsNullOrEmpty(PropertyToCheck))
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
                        Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                         " - " + PropertyToCheck + " has to be boolean value property.");
                    }
                }
                else
                {
                    Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                     " - " + PropertyToCheck + "does not exists.");
                }
            }
            else
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - propertyToCheck argument is null or empty.");
            }

            return true;
        }


        protected ConditionalDisableAttribute Attribute => attribute as ConditionalDisableAttribute;

        protected string PropertyToCheck => Attribute.PropertyToCheck;

        protected object CompareValue => Attribute.CompareValue;
    }
}