using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class BeginGroupAttribute : ToolboxAreaAttribute
    {
        public BeginGroupAttribute(string label = null)
        {
            Label = label;
        }

        public string Label { get; private set; }
    }
}