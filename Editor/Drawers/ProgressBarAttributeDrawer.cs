using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarAttributeDrawer : PropertyDrawer
    {
        private float additionalHeight;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Style.barHeight + additionalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Float && property.propertyType != SerializedPropertyType.Integer)
            {
                var helpBoxRect = new Rect(position.x, position.y, position.width, position.height / 2);
                EditorGUI.HelpBox(helpBoxRect, "ProgressBar attribute used in non-int/float value field.", MessageType.Error);
                additionalHeight = Style.height + Style.spacing;
                position.height = additionalHeight - Style.spacing;
                position.y += additionalHeight + Style.spacing;
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
                return;
            }

            var value = property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;
            var minValue = ProgressBarAttribute.MinValue;
            var maxValue = ProgressBarAttribute.MaxValue;
            var barText = !string.IsNullOrEmpty(ProgressBarAttribute.Name)
                ? ProgressBarAttribute.Name + " " + value + "|" + maxValue
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


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="global::ProgressBarAttribute"/>.
        /// </summary>
        private ProgressBarAttribute ProgressBarAttribute => attribute as ProgressBarAttribute;


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