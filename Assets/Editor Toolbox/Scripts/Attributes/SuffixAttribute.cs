using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SuffixAttribute : PropertyAttribute
    {
        public SuffixAttribute(string suffixLabel)
        {
            SuffixLabel = suffixLabel;
        }

        public string SuffixLabel { get; private set; }
    }
}