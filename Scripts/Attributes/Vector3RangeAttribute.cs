using System;

namespace UnityEngine
{
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false)]
    public class Vector3RangeAttribute : PropertyAttribute
    {
        public Vector3RangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Min { get; private set; }
        public float Max { get; private set; }
    }
}
