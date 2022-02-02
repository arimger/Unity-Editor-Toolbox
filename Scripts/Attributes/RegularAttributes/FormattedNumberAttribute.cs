using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Creates customized property field for numeric values.
    /// 
    /// <para>
    /// Supported formats:
    /// <para>
    /// <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings?redirectedfrom=MSDN">MS documentation</a>
    /// </para>
    /// </para>
    /// 
    /// <para>Supported types: <see cref="int"/>, <see cref="float"/>, <see cref="double"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class FormattedNumberAttribute : PropertyAttribute
    {
        public FormattedNumberAttribute(string format = "n")
        {
            Format = format;
        }

        public string Format { get; private set; }

        /// <summary>
        /// Indicates number of visible decimals in the text field.
        /// For <see cref="int"/> this property will be always ignored.
        /// </summary>
        public int DecimalsToShow { get; set; } = 2;
    }
}