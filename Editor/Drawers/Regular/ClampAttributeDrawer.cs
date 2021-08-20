using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ClampAttribute))]
    public class ClampAttributeDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var minValue = Attribute.MinValue;
            var maxValue = Attribute.MaxValue;
            if (property.hasMultipleDifferentValues)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    property.floatValue = Mathf.Clamp(property.floatValue, minValue, maxValue);
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = (int)Mathf.Clamp(property.intValue, minValue, maxValue);
                    break;
                default:
                    break;
            }

            EditorGUI.PropertyField(position, property, label);
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Float ||
                   property.propertyType == SerializedPropertyType.Integer;
        }


        private ClampAttribute Attribute => attribute as ClampAttribute;
    }
}