using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            var fieldWidth = EditorGUIUtility.fieldWidth;

            var minValue = Attribute.MinValue;
            var maxValue = Attribute.MaxValue;
            var xValue = property.vector2Value.x;
            var yValue = property.vector2Value.y;

            var labelRect = new Rect(position.x, position.y, labelWidth, position.height);

            var minFieldRect = new Rect(position.xMin + labelWidth, position.y, fieldWidth, position.height);
            var maxFieldRect = new Rect(position.xMax - fieldWidth, position.y, fieldWidth, position.height);
            //set slider rect between min and max fields + additional padding
            var minMaxSliderRect = new Rect
                (position.x + labelWidth + fieldWidth + Style.padding,
                 position.y,
                 position.width - labelWidth - fieldWidth * 2 - Style.padding * 2,
                 position.height);

            //begin drawing using GUI methods
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            EditorGUI.LabelField(labelRect, label);
            xValue = EditorGUI.FloatField(minFieldRect, xValue);
            yValue = EditorGUI.FloatField(maxFieldRect, yValue);
            EditorGUI.MinMaxSlider(minMaxSliderRect, ref xValue, ref yValue, minValue, maxValue);

            //values validation(xValue can't be higher than yValue etc.)
            xValue = Mathf.Clamp(xValue, minValue, Mathf.Min(maxValue, yValue));
            yValue = Mathf.Clamp(yValue, Mathf.Max(minValue, xValue), maxValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.vector2Value = new Vector2(xValue, yValue);
            }
            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Vector2;
        }


        private MinMaxSliderAttribute Attribute => attribute as MinMaxSliderAttribute;


        private static class Style
        {
            internal static readonly float padding = 8.0f;
        }
    }
}