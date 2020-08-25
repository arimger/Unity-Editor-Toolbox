using System;

namespace UnityEngine
{
    /// <summary>
    /// This attribute will mark any <see cref="Object"/> field as prefab-only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PrefabReferenceAttribute : PropertyAttribute
    { }
}
