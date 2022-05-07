using System;
using System.Diagnostics;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class HighlightAttribute : ToolboxDecoratorAttribute
    {
        public HighlightAttribute(float r, float g, float b)
        {
            Color = new Color(r, g, b);
        }

        public HighlightAttribute(string hexColor)
        {
            Color = ColorUtility.TryParseHtmlString(hexColor, out var color) ? color : Color.magenta;
        }

        public Color Color { get; private set; }
    }
}