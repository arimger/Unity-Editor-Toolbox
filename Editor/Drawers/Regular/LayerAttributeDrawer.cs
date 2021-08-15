using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerAttributeDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var value = property.intValue;
            var layer = EditorGUI.LayerField(position, label, value);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = layer;
            }

            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Integer;
        }
    }
}