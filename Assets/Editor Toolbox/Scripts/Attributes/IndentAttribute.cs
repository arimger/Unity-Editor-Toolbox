using System;

namespace UnityEngine
{
    /// <summary>
    /// Creates indent scope.
    /// Supported types: all.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class IndentAttribute : PropertyAttribute
    {
        public IndentAttribute()
        {
            IndentLevelChange = 1;
        }

        public IndentAttribute(int indentLevelChange)
        {
            IndentLevelChange = indentLevelChange;
        }

        public int IndentLevelChange { get; private set; }
    }
}