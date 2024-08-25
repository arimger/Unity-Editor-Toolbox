using System;
using System.Diagnostics;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public abstract class ToolboxDecoratorAttribute : ToolboxAttribute
    {
        /// <summary>
        /// Order of the decorator relative to other drawers on the same property.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Indicates if decorator should be created independly to state of the property.
        /// If <see cref="ApplyCondition"/> equals <see langword="true"/> it means that the decorator can be hidden/disabled same as an associated property. 
        /// </summary>
        public bool ApplyCondition { get; set; }
    }
}