using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HighlightAttribute : ToolboxDecoratorAttribute
    {
        public HighlightAttribute(float r, float g, float b)
        {
            Color = new Color(r, g, b);
        }

        public HighlightAttribute(string hexColor)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out var color))
            {
                Color = color;
            }
        }

        public Color Color { get; private set; } = Color.white;
    }
}