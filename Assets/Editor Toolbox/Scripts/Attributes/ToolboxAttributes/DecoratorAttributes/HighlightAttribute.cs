using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class HighlightAttribute : ToolboxDecoratorAttribute
    {
        public HighlightAttribute(float r, float g, float b)
        {
            Color = new Color(r, g, b);
        }

        public Color Color { get; private set; }
    }
}