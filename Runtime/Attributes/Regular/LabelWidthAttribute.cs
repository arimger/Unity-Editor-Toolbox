using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Change field label width.
    /// <para>Label width</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class LabelWidthAttribute : PropertyAttribute
    {
        public LabelWidthAttribute(float width = 120)
        {
            Width = width;
        }

        public float Width { get; private set; }
    }
}