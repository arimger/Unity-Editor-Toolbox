using UnityEditor;
using UnityEngine;

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

            //determine the real value of the property
            var value = property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;

            var color = attribute.GetBarColor();
            var minValue = attribute.MinValue;
            var maxValue = attribute.MaxValue;

            //set the value text label (handles different values)
            var valueText = property.hasMultipleDifferentValues ? "-" : value.ToString();
            var labelText = !string.IsNullOrEmpty(attribute.Name)
                ? attribute.Name + " " + valueText + "|" + maxValue : valueText + "|" + maxValue;

            //clamp current value between min and max values
            value = Mathf.Clamp(value, minValue, maxValue);

            //calculate the fill value and set the fill rect
            var fillValue = (value - minValue) / (maxValue - minValue);
            var fillRect = new Rect(position.x + Style.offset / 2,
                                    position.y + Style.offset / 2,
                                   (position.width - Style.offset) * fillValue,
                                    position.height - Style.offset);

            //draw the background and fill colors
            EditorGUI.DrawRect(position, Style.backgroundColor);
            EditorGUI.DrawRect(fillRect, color);

            //adjust rect for the shadow label
            var diff = Style.barHeight - Style.rowHeight;
            position.yMin += diff / 2;
            position.yMax -= diff / 2;
            position.y -= Style.spacing;
            //finally - draw the progress bar label
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