using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws color picker and sets color hex code.
    /// Supported types: any type with children.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LabelByChildAttribute : PropertyAttribute
    {
        public LabelByChildAttribute(string childName)
        {
            ChildName = childName;
        }

        public string ChildName { get; private set; }
    }
}