using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Use this attribute to close previously created <see cref="BeginHorizontalGroupAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class EndHorizontalGroupAttribute : EndHorizontalAttribute
    { }
}