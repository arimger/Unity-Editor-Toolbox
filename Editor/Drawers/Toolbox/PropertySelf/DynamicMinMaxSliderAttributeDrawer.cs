using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class DynamicMinMaxSliderAttributeDrawer : DynamicMinMaxBaseDrawer<DynamicMinMaxSliderAttribute>
    {
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, float minValue, float maxValue)
        {
            var xValue = 0.0f;
            var yValue = 0.0f;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                    xValue = property.vector2Value.x;
                    yValue = property.vector2Value.y;
                    break;
                case SerializedPropertyType.Vector2Int:
                    xValue = property.vector2IntValue.x;
                    yValue = property.vector2IntValue.y;
                    break;
            }

            ToolboxEditorGui.BeginProperty(property, ref label, out var position);
            position = EditorGUI.PrefixLabel(position, label);
            EditorGUI.BeginChangeCheck();
            using (new ZeroIndentScope())
            {
                ToolboxEditorGui.DrawMinMaxSlider(position, ref xValue, ref yValue, minValue, maxValue);
            }

            if (EditorGUI.EndChangeCheck())
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Vector2:
                        property.vector2Value = new Vector2(xValue, yValue);
                        break;
                    case SerializedPropertyType.Vector2Int:
                        var intXValue = Mathf.RoundToInt(xValue);
                        var intYValue = Mathf.RoundToInt(yValue);
                        property.vector2IntValue = new Vector2Int(intXValue, intYValue);
                        break;
                }
            }

            ToolboxEditorGui.CloseProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Vector2 || 
                property.propertyType == SerializedPropertyType.Vector2Int;
        }
    }
}