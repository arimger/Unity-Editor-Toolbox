using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(BoxedToggleAttribute))]
    public class BoxedToggleAttributeDrawer : ToolboxNativePropertyDrawer
    {   
        /// <summary>
        /// Draws validated property.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(position, GUIContent.none, Style.boxStyle);
            label.text = string.IsNullOrEmpty(Attribute.Label) ? label.text : Attribute.Label;
            EditorGUI.BeginChangeCheck();
            position.x += Style.padding;
            position.y += Style.spacing;
            property.boolValue = EditorGUI.ToggleLeft(position, label, property.boolValue);
            EditorGUI.EndChangeCheck();
            property.serializedObject.ApplyModifiedProperties();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Boolean;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Style.height;
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="BoxedToggleAttribute"/>.
        /// </summary>
        private BoxedToggleAttribute Attribute => attribute as BoxedToggleAttribute;


        /// <summary>
        /// Static representation of toggle style.
        /// </summary>
        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight * 1.25f;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float padding = 5.0f;

            internal static readonly GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        }
    }
}