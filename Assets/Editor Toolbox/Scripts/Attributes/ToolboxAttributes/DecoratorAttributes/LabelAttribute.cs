using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Extended version of the built-in <see cref="HeaderAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class LabelAttribute : ToolboxDecoratorAttribute
    {
        public LabelAttribute(string label, FontStyle fontStyle = FontStyle.Bold, SkinStyle skinStyle = SkinStyle.Normal)
        {
            Label = label;
            FontStyle = fontStyle;
            SkinStyle = skinStyle;
        }

        public string Label { get; private set; }

        public FontStyle FontStyle { get; private set; }

        public SkinStyle SkinStyle { get; private set; }

        public TextAnchor Alignment { get; set; } = TextAnchor.MiddleLeft;

        /// <summary>
        /// Name of the built-in icon that should be placed into the label.
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// Additional space to apply before the label field.
        /// </summary>
        public float SpaceBefore { get; set; } = 5.0f;
        /// <summary>
        /// Additional space to apply after the label field.
        /// </summary>
        public float SpaceAfter { get; set; } = 0.0f;
    }

    public enum SkinStyle
    {
        Normal,
        Box,
        Round,
        [Obsolete("Use SkinStyle.Round instead.")]
        Help
    }
}