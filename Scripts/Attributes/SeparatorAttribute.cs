using System;

namespace UnityEngine
{
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class SeparatorAttribute : PropertyAttribute
    {
        public SeparatorAttribute(float thickness = 0.75f, float padding = 6.0f)
        {
            Thickness = Mathf.Max(thickness, 0);
            Padding = Mathf.Max(padding, 0);
        }

        public float Thickness { get; private set; }

        public float Padding { get; private set; }
    }
}