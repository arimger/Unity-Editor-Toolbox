using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
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