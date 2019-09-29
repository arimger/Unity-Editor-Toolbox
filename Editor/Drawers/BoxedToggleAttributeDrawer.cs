using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(BoxedToggleAttribute))]
    public class BoxedToggleAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Style.height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on boolean value properties.");
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
                return;
            }

            EditorGUI.LabelField(position, GUIContent.none, GUI.skin.box);
            label.text = string.IsNullOrEmpty(Attribute.Label) ? label.text : Attribute.Label;
            EditorGUI.BeginChangeCheck();
            position.x += Style.padding;
            position.y += Style.spacing;
            property.boolValue = EditorGUI.ToggleLeft(position, label, property.boolValue);
            EditorGUI.EndChangeCheck();
            property.serializedObject.ApplyModifiedProperties();
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
        }
    }
}