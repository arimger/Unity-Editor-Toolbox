using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Indicates if <see cref="SerializedScene"/> drawer should show additional metadata about the picked Scene.
    /// 
    /// <para>Supported types: <see cref="SerializedScene"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class SceneDetailsAttribute : PropertyAttribute
    { }
}