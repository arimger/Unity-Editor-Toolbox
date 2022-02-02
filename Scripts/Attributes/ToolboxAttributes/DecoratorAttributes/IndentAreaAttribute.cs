using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Changes indent level of the property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class IndentAreaAttribute : ToolboxDecoratorAttribute
    {
        public IndentAreaAttribute()
        {
            IndentLevelChange = 1;
        }

        public IndentAreaAttribute(int indentLevelChange)
        {
            IndentLevelChange = indentLevelChange;
        }

        public int IndentLevelChange { get; private set; }
    }
}