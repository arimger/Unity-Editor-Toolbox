using System;

namespace UnityEngine
{
    /// <summary>
    /// Toolbox-based equivalent of the <see cref="IndentAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
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