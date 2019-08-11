using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
    public class ConditionalFieldAttributeDrawer : PropertyDrawer
    {
        private bool isVisible = true;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return isVisible ? EditorGUI.GetPropertyHeight(property) : 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!string.IsNullOrEmpty(PropertyToCheck))
            {
                var conditionProperty = property.GetSibiling(PropertyToCheck);
                if (conditionProperty != null)
                {
                    if (conditionProperty.propertyType == SerializedPropertyType.Boolean)
                    {
                        var compareValue = CompareValue != null && CompareValue is bool ? (bool)CompareValue : true;
                        isVisible = conditionProperty.boolValue == compareValue;
                        if (!isVisible) return;
                    }
                }
            }

            EditorGUI.PropertyField(position, property, label, true);
        }


        private ConditionalFieldAttribute Attribute => attribute as ConditionalFieldAttribute;

        private string PropertyToCheck => Attribute.PropertyToCheck;

        private object CompareValue => Attribute.CompareValue;
    }
}