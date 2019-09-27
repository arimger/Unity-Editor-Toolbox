using System;

namespace UnityEngine
{
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
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