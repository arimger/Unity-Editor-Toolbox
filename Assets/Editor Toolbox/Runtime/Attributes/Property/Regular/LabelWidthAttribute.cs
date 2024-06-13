using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Change field's label width.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class LabelWidthAttribute : ToolboxDecoratorAttribute
    {
        public LabelWidthAttribute(float width = 120)
        {
            Width = width;
        }

        public float Width { get; private set; }
    }
}