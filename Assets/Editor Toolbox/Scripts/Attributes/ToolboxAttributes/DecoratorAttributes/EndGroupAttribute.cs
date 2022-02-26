using System;
using System.Diagnostics;

namespace UnityEngine
{
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