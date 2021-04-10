using System;

namespace UnityEngine
{
    /// <summary>
    /// Validates input values and accepts only objects instantiated on the Scene.
    /// 
    /// <para>Supported types: <see cref="GameObject"/> and any <see cref="Component"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SceneObjectOnlyAttribute : PropertyAttribute
    { }
}