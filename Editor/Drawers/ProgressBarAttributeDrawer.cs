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
            var value = property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;
            var minValue = Attribute.MinValue;
            var maxValue = Attribute.MaxValue;
            var barText = !string.IsNullOrEmpty(Attribute.Name)
                ? Attribute.Name + " " + value + "|" + maxValue
                : value + "|" + maxValue;
            value = Mathf.Clamp(value, minValue, maxValue);
            var fill = value / (maxValue - minValue);
            var fillRect = new Rect(position.x + Style.offset / 2, position.y + Style.offset / 2,
                (position.width - Style.offset) * fill, position.height - Style.offset);

            EditorGUI.DrawRect(position, Color.grey);
            EditorGUI.DrawRect(fillRect, Style.progressBarColor);
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


        /// <summary>
        /// Static representation of progress bar style.
        /// </summary>
        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float barHeight = EditorGUIUtility.singleLineHeight * 1.25f;
            internal static readonly float offset = 4;

            internal static readonly Color progressBarColor = new Color(0.28f, 0.38f, 0.88f);
        }
    }
}