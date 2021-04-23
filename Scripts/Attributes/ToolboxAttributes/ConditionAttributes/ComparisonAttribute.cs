using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class ComparisonAttribute : ToolboxConditionAttribute
    {
        /// <param name="propertyName">L-value or the mask if <see cref="TestMethod"/> is set to <see cref="ComparisionTestMethod.Mask"/></param>
        /// <param name="valueToMatch">R-value or the flag if <see cref="TestMethod"/> is set to <see cref="ComparisionTestMethod.Mask"/></param>
        public ComparisonAttribute(string propertyName, object valueToMatch)
        {
            PropertyName = propertyName;
            ValueToMatch = valueToMatch;
        }

        public string PropertyName { get; private set; }

        public object ValueToMatch { get; private set; }

        /// <summary>
        /// Indicates what method will be used to compare value of the target property and <see cref="ValueToMatch"/>.
        /// </summary>
        public ComparisionTestMethod TestMethod { get; set; } = ComparisionTestMethod.Equal;
    }

    public enum ComparisionTestMethod
    {
        /// <summary>
        /// Checks if values are equal.
        /// </summary>
        Equal,
        /// <summary>
        /// Checks if the target value is greater.
        /// <para>Allowed only for numeric types.</para>
        /// </summary>
        Greater,
        /// <summary>
        /// Checks if the target value is less.
        /// <para>Allowed only for numeric types.</para>
        /// </summary>
        Less,
        /// <summary>
        /// Checks if the target value is greater or equal.
        /// <para>Allowed only for numeric types.</para>
        /// </summary>
        GreaterEqual,
        /// <summary>
        /// Checks if the target value is less or equal.
        /// <para>Allowed only for numeric types.</para>
        /// </summary>
        LessEqual,
        /// <summary>
        /// Checks if the target value is included in a mask.
        /// <para>Allowed only for <see cref="Enum"/> fields marked with <see cref="FlagsAttribute"/>.</para>
        /// </summary>
        Mask
    }
}