using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PasswordAttribute))]
    public class PasswordAttributeDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //begin the true property
            label = EditorGUI.BeginProperty(position, label, property);
            //draw the prefix label
            position = EditorGUI.PrefixLabel(position, label);
            //craete the password field and cache the current result
            property.stringValue = EditorGUI.PasswordField(position, GUIContent.none, property.stringValue);
            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.String;
        }
    }
}