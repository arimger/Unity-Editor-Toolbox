using System;

namespace UnityEngine
{
    /// <summary>
    /// Indicates if <see cref="SerializedScene"/> drawer should show additional metadata about the picked Scene.
    /// 
    /// <para>Supported types: <see cref="SerializedScene"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SceneDetailsAttribute : PropertyAttribute
    { }
}