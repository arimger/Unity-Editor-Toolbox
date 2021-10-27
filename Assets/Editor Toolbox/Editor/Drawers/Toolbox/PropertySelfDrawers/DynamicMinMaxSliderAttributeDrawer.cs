using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class DynamicMinMaxSliderAttributeDrawer : ToolboxSelfPropertyDrawer<DynamicMinMaxSliderAttribute>
    {
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, DynamicMinMaxSliderAttribute attribute)
        {
            var minValueSource = attribute.MinValueSource;
            var maxValueSource = attribute.MaxValueSource;
            if (!ValueExtractionHelper.TryGetValue(minValueSource, property, out var minValueCandidate, out _) ||
                !ValueExtractionHelper.TryGetValue(maxValueSource, property, out var maxValueCandidate, out _))
            {
                ToolboxEditorLog.MemberNotFoundWarning(attribute, property,
                    string.Format("{0} or {1}", minValueSource, maxValueSource));
                base.OnGuiSafe(property, label, attribute);
                return;
            }

            var maxValue = 0.0f;
            var minValue = 0.0f;
            try
            {
                minValue = Convert.ToSingle(minValueCandidate);
                maxValue = Convert.ToSingle(maxValueCandidate);
            }
            catch (Exception e) when (e is InvalidCastException || e is FormatException)
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    string.Format("Invalid source types, cannot convert them to {0}", typeof(float)));
            }

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