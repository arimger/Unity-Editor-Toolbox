using System;

namespace Toolbox.Editor.Drawers
{
    public enum ValueComparisonMethod
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