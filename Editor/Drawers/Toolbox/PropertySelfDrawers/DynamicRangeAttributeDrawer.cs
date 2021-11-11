using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class DynamicRangeAttributeDrawer : ToolboxSelfPropertyDrawer<DynamicRangeAttribute>
    {
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, DynamicRangeAttribute attribute)
        {
            //TODO: base drawer or utility for min max value convertions
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

            float minValue;
            float maxValue;
            try
            {
                minValue = Convert.ToSingle(minValueCandidate);
                maxValue = Convert.ToSingle(maxValueCandidate);
            }
            catch (Exception e) when (e is InvalidCastException || e is FormatException)
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    string.Format("Invalid source types, cannot convert them to {0}", typeof(float)));
                base.OnGuiSafe(property, label, attribute);
                return;
            }

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