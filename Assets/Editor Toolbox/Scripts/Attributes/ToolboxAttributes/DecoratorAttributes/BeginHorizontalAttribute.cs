using System;

namespace UnityEngine
{
    /// <summary>
    /// Begins horizontal group of properties. Has to be closed by the <see cref="EndHorizontalAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class BeginHorizontalAttribute : ToolboxDecoratorAttribute
    {
        public BeginHorizontalAttribute(float labelToWdithRatio = 0.1f)
        {
            LabelToWdithRatio = labelToWdithRatio;
        }

        public float LabelToWdithRatio { get; private set; }
    }
}