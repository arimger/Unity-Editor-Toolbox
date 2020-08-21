using System;

namespace UnityEngine
{
    /// <summary>
    /// Layout-based equivalent of the <see cref="HelpAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class HelpAreaAttribute : ToolboxDecoratorAttribute
    {        
        /// <summary>
        /// Adds a HelpBox to the Unity property inspector above this field.
        /// </summary>
        /// <param name="text">The help text to be displayed in the HelpBox.</param>
        /// <param name="type">The icon to be displayed in the HelpBox.</param>
        public HelpAreaAttribute(string text, UnityMessageType type = UnityMessageType.Info)
        {
            Text = text;
            Type = type;
        }

        public UnityMessageType Type { get; private set; }

        public string Text { get; private set; }
    }
}
