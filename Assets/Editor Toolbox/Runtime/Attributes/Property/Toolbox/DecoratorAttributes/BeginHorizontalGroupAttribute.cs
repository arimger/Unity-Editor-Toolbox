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
        public BeginHorizontalGroupAttribute() : base()
        {
            WidthOffset = 32.0f;
        }

        [Obsolete("Ratios are no longer valid, use default constructor, ControlFieldWidth and linked properties to specify width of layout elements.")]
        public BeginHorizontalGroupAttribute(float labelToWidthRatio = 0.0f, float fieldToWidthRatio = 0.0f, string label = null) : base(labelToWidthRatio, fieldToWidthRatio)
        {
            Label = label;
        }

        /// <summary>
        /// Optional label (header) that can be displayed at the group's top.
        /// </summary>
        public string Label { get; set; }
        public bool HasLabel => !string.IsNullOrEmpty(Label);
        /// <summary>
        /// Indicates what style should be used to render the group.
        /// </summary>
#if UNITY_2019_3_OR_NEWER
        public GroupStyle Style { get; set; } = GroupStyle.Round;
#else
        public GroupStyle Style { get; set; } = GroupStyle.Boxed;
#endif

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