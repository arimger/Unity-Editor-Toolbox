using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Begins horizontal group of properties. 
    /// Additionally, creates title label and scrollbar if needed.
    /// Has to be closed by the <see cref="EndHorizontalGroupAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class BeginHorizontalGroupAttribute : BeginHorizontalAttribute
    {
        public BeginHorizontalGroupAttribute(float labelToWidthRatio = 0.0f, float fieldToWidthRatio = 0.0f, string label = null) : base(labelToWidthRatio, fieldToWidthRatio)
        {
            Label = label;
        }

        public string Label { get; private set; }

        public bool HasLabel => !string.IsNullOrEmpty(Label);

#if UNITY_2019_1_OR_NEWER
        /// <summary>
        /// Indicates the fixed height of the horizontal group, if is equal to 0 then height will be auto-sized.
        /// </summary>
        public float Height { get; set; } = 0.0f;
#else
        public float Height { get; set; } = 300.0f;
#endif
    }
}