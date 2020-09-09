using System;

namespace UnityEngine
{
    /// <summary>
    /// Begins vertical group of properties. Has to be closed by the <see cref="EndGroupAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class BeginGroupAttribute : ToolboxDecoratorAttribute
    {
        public BeginGroupAttribute(string label = null)
        {
            Label = label;
            Order = 1000;
        }

        public string Label { get; private set; }

        public bool HasLabel => !string.IsNullOrEmpty(Label);
    }
}