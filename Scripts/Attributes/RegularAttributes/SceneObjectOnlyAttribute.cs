using System;

namespace UnityEngine
{
    /// <summary>
    /// Validates input values and accepts only objects instantiated on the Scene.
    /// Supported types: <see cref="GameObject"/> and any <see cref="Component"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SceneObjectOnlyAttribute : PropertyAttribute
    { }
}