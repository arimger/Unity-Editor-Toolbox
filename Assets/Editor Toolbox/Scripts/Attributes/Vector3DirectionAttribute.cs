using System;

namespace UnityEngine
{
    /// <summary>
    /// Allows to pick valid 3D direction value.
    /// Supported types: <see cref="Vector3"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class Vector3DirectionAttribute : PropertyAttribute
    { }
}