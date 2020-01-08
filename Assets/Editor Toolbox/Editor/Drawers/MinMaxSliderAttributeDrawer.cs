using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderAttributeDrawer : ToolboxNativePropertyDrawer
    {
        /// <summary>
        /// Draws validated property.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            var fieldWidth = EditorGUIUtility.fieldWidth;

            var minValue = Attribute.MinValue;
            var maxValue = Attribute.MaxValue;
            var xValue = property.vector2Value.x;
            var yValue = property.vector2Value.y;

            var labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            var minFieldRect = new Rect(position.x + labelWidth, position.y, fieldWidth, position.height);
            var maxFieldRect = new Rect(position.xMax - fieldWidth, position.y, fieldWidth, position.height);
            //set slider rect between min and max fields + additional padding
            var minMaxSliderRect = new Rect
                (position.x + labelWidth + fieldWidth + Style.sliderPadding,
                 position.y,
                 position.width - labelWidth - fieldWidth * 2 - Style.sliderPadding * 2,
                 position.height);

            //begin drawing using GUI methods
            EditorGUI.BeginChangeCheck();
            EditorGUI.LabelField(labelRect, property.displayName);
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
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Vector2;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="MinMaxSliderAttribute"/>.
        /// </summary>
        private MinMaxSliderAttribute Attribute => attribute as MinMaxSliderAttribute;


        /// <summary>
        /// Static representation of slider style.
        /// </summary>
        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float sliderPadding = 8.0f;
        }
    }
}