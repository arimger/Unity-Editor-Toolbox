using System;
using System.Diagnostics;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public abstract class ComparisonAttribute : ToolboxConditionAttribute
    {
        /// <param name="sourceHandle">L-value or the mask if <see cref="Comparison"/> is set to <see cref="UnityComparisonMethod.Mask"/></param>
        /// <param name="valueToMatch">R-value or the flag if <see cref="Comparison"/> is set to <see cref="UnityComparisonMethod.Mask"/></param>
        public ComparisonAttribute(string sourceHandle, object valueToMatch)
        {
            SourceHandle = sourceHandle;
            ValueToMatch = valueToMatch;
        }

        public string SourceHandle { get; private set; }

        public object ValueToMatch { get; private set; }

        /// <summary>
        /// Indicates what method will be used to compare value of the target property and <see cref="ValueToMatch"/>.
        /// </summary>
        [Obsolete("Use Comparsion property instead.")]
        public ComparisionTestMethod TestMethod { get; set; } = ComparisionTestMethod.Equal;
        /// <summary>
        /// Indicates what method will be used to compare value of the target property and <see cref="ValueToMatch"/>.
        /// </summary>
        public UnityComparisonMethod Comparison { get; set; } = UnityComparisonMethod.Equal;
    }

    [Obsolete]
    public enum ComparisionTestMethod
    {
        Equal,
        Greater,
        Less,
        GreaterEqual,
        LessEqual,
        Mask
    }
}