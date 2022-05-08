using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class DynamicMinMaxBaseDrawer<T> : ToolboxSelfPropertyDrawer<T> where T : DynamicMinMaxBaseAttribute
    {
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, T attribute)
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

            OnGuiSafe(property, label, minValue, maxValue);
        }

        protected abstract void OnGuiSafe(SerializedProperty property, GUIContent label, float minValue, float maxValue);
    }
}