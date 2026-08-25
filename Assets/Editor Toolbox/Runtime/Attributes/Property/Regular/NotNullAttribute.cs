using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Draws a information box if the associated value is null.
    /// 
    /// <para>Supported types: any <see cref="Object"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class NotNullAttribute : PropertyAttribute
    {
        public NotNullAttribute(UnityMessageType type = UnityMessageType.Error) : this("Variable has to be assigned.", type)
        { }

        public NotNullAttribute(string label, UnityMessageType type = UnityMessageType.Error)
        {
            Label = label;
            Type = type;
        }

        public string Label { get; private set; }
        public UnityMessageType Type { get; private set; }
    }
}
