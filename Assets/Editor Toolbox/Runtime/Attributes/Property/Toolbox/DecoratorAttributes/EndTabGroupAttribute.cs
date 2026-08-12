using System;
using System.Diagnostics;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class EndTabGroupAttribute : ToolboxDecoratorAttribute
    { }
}
