using System;

namespace UnityEngine
{
    /// <summary>
    /// This attribute will mark <see cref="Object"/> as prefab-only.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false)]
    public class PrefabReferenceAttribute : PropertyAttribute
    { }
}
