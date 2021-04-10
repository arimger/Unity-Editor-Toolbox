using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws an additional suffix label.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SuffixAttribute : PropertyAttribute
    {
        public SuffixAttribute(string suffixLabel)
        {
            SuffixLabel = suffixLabel;
        }

        public string SuffixLabel { get; private set; }
    }
}