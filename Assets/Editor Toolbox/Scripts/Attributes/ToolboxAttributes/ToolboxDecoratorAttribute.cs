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
        /// Indicates if decorator should be visible only if the target property is visible.
        /// </summary>
        [Obsolete("Feature not implemented. Decorators will be visible even if the target property is hidden.")]
        public bool HideWithProperty { get; set; }
    }
}