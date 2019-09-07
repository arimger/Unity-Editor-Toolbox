using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
    public class ConditionalFieldAttributeDrawer : PropertyDrawer
    {
        private bool IsVisible(SerializedProperty property)
        {
            //TODO: additional error and warnings;
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
                }
            }

            return true;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsVisible(property) ? EditorGUI.GetPropertyHeight(property) : -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsVisible(property)) EditorGUI.PropertyField(position, property, label, true);
        }


        private ConditionalFieldAttribute Attribute => attribute as ConditionalFieldAttribute;

        private string PropertyToCheck => Attribute.PropertyToCheck;

        private object CompareValue => Attribute.CompareValue;
    }
}