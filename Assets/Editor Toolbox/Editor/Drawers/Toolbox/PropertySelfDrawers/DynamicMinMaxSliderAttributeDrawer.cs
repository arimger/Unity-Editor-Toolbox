using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class DynamicMinMaxSliderAttributeDrawer : DynamicMinMaxBaseDrawer<DynamicMinMaxSliderAttribute>
    {
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, float minValue, float maxValue)
        {
            var xValue = property.vector2Value.x;
            var yValue = property.vector2Value.y;
            ToolboxEditorGui.BeginProperty(property, ref label, out var position);
            EditorGUI.BeginChangeCheck();
            ToolboxEditorGui.DrawMinMaxSlider(position, label, ref xValue, ref yValue, minValue, maxValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.vector2Value = new Vector2(xValue, yValue);
            }

            ToolboxEditorGui.CloseProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Vector2;
        }
    }
}