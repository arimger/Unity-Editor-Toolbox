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
        public NotNullAttribute(UnityMessageType messageType = UnityMessageType.Error) : this("Variable has to be assigned.", messageType)
        { }

        public NotNullAttribute(string label, UnityMessageType messageType = UnityMessageType.Error)
        {
            Label = label;
            MessageType = messageType;
        }

        public string Label { get; private set; }
        public UnityMessageType MessageType { get; private set; }
    }
}
