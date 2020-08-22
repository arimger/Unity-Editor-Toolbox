using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarAttributeDrawer : ToolboxNativePropertyDrawer
    {
        /// <summary>
        /// Draws validated property.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = Attribute;
          
            var value = property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;

            var color = attribute.GetBarColor();         
            var minValue = attribute.MinValue;
            var maxValue = attribute.MaxValue;
            var barText = !string.IsNullOrEmpty(attribute.Name)
                ? attribute.Name + " " + value + "|" + maxValue : value + "|" + maxValue;

            value = Mathf.Clamp(value, minValue, maxValue);

            var fillValue = value / (maxValue - minValue);
            var fillRect = new Rect(position.x + Style.offset / 2, 
                                    position.y + Style.offset / 2,
                                   (position.width  - Style.offset) * fillValue, 
                                    position.height - Style.offset);

            EditorGUI.DrawRect(position, Style.backgroundColor);
            EditorGUI.DrawRect(fillRect, color);
            position.y -= Style.barHeight / 4;
            EditorGUI.DropShadowLabel(position, barText);
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Float ||
                   property.propertyType == SerializedPropertyType.Integer;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Style.barHeight;
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="ProgressBarAttribute"/>.
        /// </summary>
        private ProgressBarAttribute Attribute => attribute as ProgressBarAttribute;


        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float barHeight = EditorGUIUtility.singleLineHeight * 1.25f;
            internal static readonly float offset = 4.0f;

            internal static readonly Color backgroundColor = Color.grey;
        }
    }
}