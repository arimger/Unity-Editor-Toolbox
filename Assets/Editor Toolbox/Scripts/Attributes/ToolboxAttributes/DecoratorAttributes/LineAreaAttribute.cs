using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class LineAreaAttribute : ToolboxDecoratorAttribute
    {
        public LineAreaAttribute(float thickness = 0.75f, float padding = 6.0f)
        {
            Thickness = Math.Max(thickness, 0);
            Padding = Math.Max(padding, 0);
        }

        public float Thickness { get; private set; }

        public float Padding { get; private set; }
    }
}