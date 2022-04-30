using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class DynamicRangeAttributeDrawer : DynamicMinMaxBaseDrawer<DynamicRangeAttribute>
    {
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, float minValue, float maxValue)
        {
            ToolboxEditorGui.BeginProperty(property, ref label, out var position);
            EditorGUI.BeginChangeCheck();
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    var intValue = EditorGUI.IntSlider(position, label, property.intValue, (int)minValue, (int)maxValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.intValue = intValue;
                    }
                    break;
                case SerializedPropertyType.Float:
                    var floatValue = EditorGUI.Slider(position, label, property.floatValue, minValue, maxValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.floatValue = floatValue;
                    }
                    break;
                default:
                    break;
            }

            ToolboxEditorGui.CloseProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Integer ||
                   property.propertyType == SerializedPropertyType.Float;
        }
    }
}