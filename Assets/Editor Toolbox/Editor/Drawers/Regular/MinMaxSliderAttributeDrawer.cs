using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderAttributeDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var minValue = Attribute.MinValue;
            var maxValue = Attribute.MaxValue;
            var xValue = property.vector2Value.x;
            var yValue = property.vector2Value.y;

            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            ToolboxEditorGui.DrawMinMaxSlider(position, label, ref xValue, ref yValue, minValue, maxValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.vector2Value = new Vector2(xValue, yValue);
            }

            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Vector2;
        }


        private MinMaxSliderAttribute Attribute => attribute as MinMaxSliderAttribute;
    }
}