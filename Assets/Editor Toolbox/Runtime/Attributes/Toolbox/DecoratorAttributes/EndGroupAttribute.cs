using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Use this attribute to close previously created <see cref="BeginGroupAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class EndGroupAttribute : ToolboxDecoratorAttribute
    {
        public EndGroupAttribute()
        {
            Order = -1000;
        }
    }
}