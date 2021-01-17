using System;

namespace UnityEngine
{
    /// <summary>
    /// Replaces old label with <see cref="NewLabel"/> value.
    /// Supported types: all.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
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