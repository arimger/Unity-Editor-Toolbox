using System;

namespace UnityEngine
{
    /// <summary>
    /// Begins horizontal group of properties. Has to be closed by the <see cref="EndHorizontalAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class BeginHorizontalAttribute : ToolboxDecoratorAttribute
    {
        public BeginHorizontalAttribute(float labelToWidthRatio = 0.0f, float fieldToWidthRatio = 0.0f)
        {
            LabelToWidthRatio = labelToWidthRatio;
            FieldToWidthRatio = fieldToWidthRatio;
        }

        public float LabelToWidthRatio { get; private set; }

        public float FieldToWidthRatio { get; private set; }
    }
}