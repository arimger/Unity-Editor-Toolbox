using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Begins vertical layout of properties. Has to be closed by the <see cref="EndVerticalAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class BeginVerticalAttribute : ToolboxDecoratorAttribute
    { }
}