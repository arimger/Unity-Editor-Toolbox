using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarAttributeDrawer : PropertyDrawerBase
    {
        private static readonly int drawerHash = nameof(ProgressBarAttributeDrawer).GetHashCode();

        private void HandleGuiEvents(SerializedProperty property, Rect progressBarRect)
        {
            var mousePosition = Event.current.mousePosition;
            var id = GUIUtility.GetControlID(drawerHash, FocusType.Passive, progressBarRect);
            switch (Event.current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (progressBarRect.Contains(mousePosition))
                    {
                        GUIUtility.hotControl = id;
                        SetProgressValue(property, progressBarRect, mousePosition.x);
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        SetProgressValue(property, progressBarRect, mousePosition.x);
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                    break;
            }
        }

        private void SetProgressValue(SerializedProperty property, Rect progressBarRect, float xPosition)
        {
            var minValue = Attribute.MinValue;
            var maxValue = Attribute.MaxValue;

            var range = progressBarRect.xMax - progressBarRect.xMin;
            xPosition = Mathf.Clamp(xPosition - progressBarRect.xMin, 0, range);

            var fill = Mathf.Clamp01(xPosition / range);
            var newValue = (maxValue - minValue) * fill + minValue;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = Mathf.RoundToInt(newValue);
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = newValue;
                    break;
            }
        }

        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return Style.barHeight;
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = Attribute;
            //determine the real value of the property
            var value = property.propertyType == SerializedPropertyType.Integer
                ? property.intValue
                : property.floatValue;

            var fillLabel = attribute.Name;
            var fillColor = attribute.Color;
            var minValue = attribute.MinValue;
            var maxValue = attribute.MaxValue;

            //set the value text label (add name if needed)
            var valueText = property.hasMultipleDifferentValues ? "-" : value.ToString();
            var labelText = !string.IsNullOrEmpty(fillLabel)
                ? string.Format("{0} {1}|{2}", fillLabel, valueText, maxValue)
                : string.Format("{0}|{1}", valueText, maxValue);

            //clamp current value between min and max values
            value = Mathf.Clamp(value, minValue, maxValue);

            //calculate the fill value and set the fill rect
            var fillValue = (value - minValue) / (maxValue - minValue);
            var fillRect = new Rect(position.x + Style.fillOffset / 2,
                                    position.y + Style.fillOffset / 2,
                                   (position.width - Style.fillOffset) * fillValue,
                                    position.height - Style.fillOffset);

            //draw the background and fill colors
            EditorGUI.DrawRect(position, Style.backgroundColor);
            EditorGUI.DrawRect(fillRect, fillColor);

            //adjust rect for the shadow label
            var diff = Style.barHeight - Style.rowHeight;
            position.yMin += diff / 2;
            position.yMax -= diff / 2;
            position.y -= Style.textOffset;
            //finally draw the progress bar label
            EditorGUI.DropShadowLabel(position, labelText);

            if (!attribute.IsInteractable)
            {
                return;
            }

            HandleGuiEvents(property, position);
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
            internal static readonly float textOffset = 2.0f;
            internal static readonly float fillOffset = 4.0f;

            internal static readonly Color backgroundColor = Color.grey;
        }
    }
}