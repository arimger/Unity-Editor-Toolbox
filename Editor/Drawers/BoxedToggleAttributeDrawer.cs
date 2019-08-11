using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(BoxedToggleAttribute))]
    public class BoxedToggleAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Optional height offset, will be added to standard <see cref="height"/> value.
        /// </summary>
        private float additionalHeight;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Style.height + additionalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                var helpBoxRect = new Rect(position.x, position.y, position.width, position.height / 2);
                EditorGUI.HelpBox(helpBoxRect, "Toggle attribute used in non-boolean value field.", MessageType.Error);
                additionalHeight = Style.height + Style.spacing;
                position.height = additionalHeight - Style.spacing;
                position.y += additionalHeight + Style.spacing;
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
                return;
            }

            EditorGUI.LabelField(position, GUIContent.none, GUI.skin.box);
            label.text = string.IsNullOrEmpty(ToggleAttribute.Label) ? label.text : ToggleAttribute.Label;
            EditorGUI.BeginChangeCheck();
            position.x += Style.padding;
            position.y += Style.spacing;
            property.boolValue = EditorGUI.ToggleLeft(position, label, property.boolValue);
            EditorGUI.EndChangeCheck();
            property.serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="global::BoxedToggleAttribute"/>.
        /// </summary>
        private BoxedToggleAttribute ToggleAttribute => attribute as BoxedToggleAttribute;


        /// <summary>
        /// Static representation of toggle style.
        /// </summary>
        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight * 1.25f;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float padding = 5.0f;
        }
    }
}