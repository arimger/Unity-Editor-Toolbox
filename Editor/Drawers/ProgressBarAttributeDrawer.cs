using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return Style.barHeight;
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = Attribute;
          
            var value = property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;

            var color = attribute.GetBarColor();         
            var minValue = attribute.MinValue;
            var maxValue = attribute.MaxValue;

            var valueText = property.hasMultipleDifferentValues ? "-" : value.ToString();
            var labelText = !string.IsNullOrEmpty(attribute.Name)
                ? attribute.Name + " " + valueText + "|" + maxValue : valueText + "|" + maxValue;

            value = Mathf.Clamp(value, minValue, maxValue);

            var fillValue = (value - minValue) / (maxValue - minValue);
            var fillRect = new Rect(position.x + Style.offset / 2, 
                                    position.y + Style.offset / 2,
                                   (position.width  - Style.offset) * fillValue, 
                                    position.height - Style.offset);

            EditorGUI.DrawRect(position, Style.backgroundColor);
            EditorGUI.DrawRect(fillRect, color);
            position.y -= Style.barHeight / 4;
            EditorGUI.DropShadowLabel(position, labelText);
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Float ||
                   property.propertyType == SerializedPropertyType.Integer;
        }


        private ProgressBarAttribute Attribute => attribute as ProgressBarAttribute;


        private static class Style
        {
            internal static readonly float rowHeight = EditorGUIUtility.singleLineHeight;
            internal static readonly float barHeight = EditorGUIUtility.singleLineHeight * 1.25f;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float offset = 4.0f;

            internal static readonly Color backgroundColor = Color.grey;
        }
    }
}