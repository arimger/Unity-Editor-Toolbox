using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Creates a HelpBox within the Inspector Window above a serialized field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class HelpAttribute : ToolboxDecoratorAttribute
    {
        public HelpAttribute(string text, UnityMessageType type = UnityMessageType.Info)
        {
            Text = text;
            Type = type;
        }

        public string Text { get; private set; }

        public UnityMessageType Type { get; private set; }
    }
}