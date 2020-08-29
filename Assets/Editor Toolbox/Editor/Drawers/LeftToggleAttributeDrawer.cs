using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(LeftToggleAttribute))]
    public class LeftToggleAttributeDrawer : ToolboxNativePropertyDrawer
    {
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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}