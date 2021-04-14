using System;

namespace UnityEngine
{
    [Obsolete("Use LineAttribute instead.")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class SeparatorAttribute : PropertyAttribute
    {
        public SeparatorAttribute(float thickness = 0.75f, float padding = 6.0f)
        {
            //validate the thickness property
            Thickness = Mathf.Max(thickness, 0);
            //validate the padding property
            Padding = Mathf.Max(padding, 0);
        }

        public float Thickness { get; private set; }

        public float Padding { get; private set; }
    }
}