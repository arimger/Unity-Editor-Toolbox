using System;

namespace UnityEngine
{
    /// <summary>
    /// Starts vertical group of properties in the inspector window. Has to be closed by the <see cref="EndGroupAttribute"/>.
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