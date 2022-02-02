using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Creates a HelpBox within the Inspector Window above a serialized field.
    /// Unlike <see cref="HelpAttribute"/> this attribute is based on given, reflection source.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class DynamicHelpAttribute : ToolboxDecoratorAttribute
    {
        /// <param name="sourceHandle">Name of the declared source: field, property, or method.</param>
        public DynamicHelpAttribute(string sourceHandle, UnityMessageType type = UnityMessageType.Info)
        {
            SourceHandle = sourceHandle;
            Type = type;
        }

        public string SourceHandle { get; private set; }

        public UnityMessageType Type { get; private set; }
    }
}