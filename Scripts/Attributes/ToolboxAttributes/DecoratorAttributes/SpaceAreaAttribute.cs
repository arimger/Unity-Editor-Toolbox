using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Extended version of the built-in <see cref="SpaceAttribute"/>.
    /// Creates additional space before and after serialized field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class SpaceAreaAttribute : ToolboxDecoratorAttribute
    {
        public SpaceAreaAttribute(float spaceBefore = 5.0f, float spaceAfter = 0.0f)
        {
            SpaceBefore = spaceBefore;
            SpaceAfter = spaceAfter;
        }

        public float SpaceBefore { get; private set; }

        public float SpaceAfter { get; private set; }
    }
}