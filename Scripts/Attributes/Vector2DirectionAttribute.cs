using System;

namespace UnityEngine
{
    /// <summary>
    /// Allows to pick valid 2D direction value.
    /// Supported types: <see cref="Vector2"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class Vector2DirectionAttribute : PropertyAttribute
    { }
}