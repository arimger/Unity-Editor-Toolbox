using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Changes GUI color of all related controls (other decorators and the property field).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class GuiColorAttribute : ToolboxDecoratorAttribute
    {
        public GuiColorAttribute(float r, float g, float b)
        {
            Color = new Color(r, g, b);
        }

        public GuiColorAttribute(string hexColor)
        {
            Color = ColorUtility.TryParseHtmlString(hexColor, out var color) ? color : Color.magenta;
        }

        public Color Color { get; private set; }
    }
}