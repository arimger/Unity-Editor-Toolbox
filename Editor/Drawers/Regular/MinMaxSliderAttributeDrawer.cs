using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderAttributeDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var minValue = Attribute.MinValue;
            var maxValue = Attribute.MaxValue;

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

            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            position = EditorGUI.PrefixLabel(position, label);
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

            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Vector2 ||
                property.propertyType == SerializedPropertyType.Vector2Int;
        }


        private MinMaxSliderAttribute Attribute => attribute as MinMaxSliderAttribute;
    }
}