using System;

namespace UnityEngine
{
    /// <summary>
    /// Starts vertical group of properties in inspector window. Has to be closed by <see cref="EndGroupAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class BeginGroupAttribute : ToolboxDecoratorAttribute
    {
        public BeginGroupAttribute(string label = null)
        {
            Label = label;
        }

        public string Label { get; private set; }
    }
}