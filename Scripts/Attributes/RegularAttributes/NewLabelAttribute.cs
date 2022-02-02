using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Replaces old label with <see cref="NewLabel"/> value.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class NewLabelAttribute : PropertyAttribute
    {
        public NewLabelAttribute(string newLabel, string oldLabel = null)
        {
            NewLabel = newLabel;
            OldLabel = oldLabel;
        }

        public string NewLabel { get; private set; }

        public string OldLabel { get; private set; }
    }
}