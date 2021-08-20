using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(LeftToggleAttribute))]
    public class LeftToggleAttributeDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.ToggleLeft(position, label, property.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = value;
            }
            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Boolean;
        }
    }
}