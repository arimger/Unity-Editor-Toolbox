using System;

namespace UnityEngine
{
    /// <summary>
    /// Begins horizontal group in inspector. Use <seealso cref="EndHorizontalAttribute"/> on serialized property to end this group.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class BeginHorizontalAttribute : ToolboxDecoratorAttribute
    {
        public BeginHorizontalAttribute(float labelToWdithRatio = 0.1f)
        {
            LabelToWdithRatio = labelToWdithRatio;
        }

        public float LabelToWdithRatio { get; private set; }
    }
}