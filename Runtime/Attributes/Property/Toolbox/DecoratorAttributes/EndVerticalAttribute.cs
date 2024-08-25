using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Use this attribute to close previously created <see cref="BeginVerticalAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class EndVerticalAttribute : ToolboxDecoratorAttribute
    {
        public EndVerticalAttribute()
        {
            Order = -1000;
        }
    }
}