using System;

namespace UnityEngine
{
    /// <summary>
    /// Begins horizontal group of properties. 
    /// Additionally, creates title label and scrollbar if needed.
    /// Has to be closed by the <see cref="EndHorizontalGroupAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class BeginHorizontalGroupAttribute : BeginHorizontalAttribute
    {
        public BeginHorizontalGroupAttribute(float labelToWidthRatio = 0.0f, float fieldToWidthRatio = 0.0f, string label = null) : base(labelToWidthRatio, fieldToWidthRatio)
        {
            Label = label;
        }

        public string Label { get; private set; }

        public bool HasLabel => !string.IsNullOrEmpty(Label);
    }
}