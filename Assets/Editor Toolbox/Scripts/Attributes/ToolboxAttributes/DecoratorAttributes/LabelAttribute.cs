using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class LabelAttribute : ToolboxDecoratorAttribute
    {
        public LabelAttribute(string label, FontStyle fontStyle = FontStyle.Bold, SkinStyle skinStyle = SkinStyle.Normal)
        {
            Label = label;
            FontStyle = fontStyle;
            SkinStyle = skinStyle;
        }

        public string Label { get; private set; }

        public string Content { get; set; }

        public FontStyle FontStyle { get; private set; }

        public SkinStyle SkinStyle { get; private set; }

        public TextAnchor Alignment { get; set; } = TextAnchor.MiddleLeft;
    }

    public enum SkinStyle
    {
        Normal,
        Box,
        Help,
    }
}