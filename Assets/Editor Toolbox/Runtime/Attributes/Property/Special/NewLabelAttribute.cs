using System;
using System.Diagnostics;

using Toolbox.Attributes.Property;

namespace UnityEngine
{
    /// <summary>
    /// Replaces old label with <see cref="NewLabel"/> value.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class NewLabelAttribute : PropertyAttribute, ILabelProcessorAttribute
    {
        public NewLabelAttribute(string newLabel)
        {
            NewLabel = newLabel;
        }

        [Obsolete]
        public NewLabelAttribute(string newLabel, string oldLabel = null)
        {
            NewLabel = newLabel;
            OldLabel = oldLabel;
        }

        /// <summary>
        /// New label that will be used in the Inspector.
        /// </summary>
        public string NewLabel { get; private set; }
        /// <summary>
        /// Indicates what name (or part) has to be replaced.
        /// Useful to change labels for array elements.
        /// </summary>
        [Obsolete]
        public string OldLabel { get; private set; }
    }
}