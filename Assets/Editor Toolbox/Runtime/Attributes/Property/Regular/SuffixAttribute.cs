using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Draws an additional suffix label.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class SuffixAttribute : PropertyAttribute
    {
        public SuffixAttribute(string suffixLabel)
        {
            SuffixLabel = suffixLabel;
        }

        public string SuffixLabel { get; private set; }
    }
}