using System;

namespace UnityEngine
{
    /// <summary>
    /// Extended version of the built-in <see cref="HeaderAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class LabelAttribute : ToolboxDecoratorAttribute
    {
        public LabelAttribute(string label, FontStyle fontStyle = FontStyle.Bold, SkinStyle skinStyle = SkinStyle.Normal)
        {
            Label = label;
            FontStyle = fontStyle;
            SkinStyle = skinStyle;
        }

        public string Label { get; private set; }

        public string Asset { get; set; }

        public FontStyle FontStyle { get; private set; }

        public SkinStyle SkinStyle { get; private set; }

        public TextAnchor Alignment { get; set; } = TextAnchor.MiddleLeft;
    }

    public enum SkinStyle
    {
        Normal,
        Box,
        Round,
        [Obsolete("Use SkinStyle.Round instead")]
        Help
    }
}