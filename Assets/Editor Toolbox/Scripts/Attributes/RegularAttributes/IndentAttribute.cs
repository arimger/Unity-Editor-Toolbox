using System;

namespace UnityEngine
{
    /// <summary>
    /// Creates indent scope.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [Obsolete("Use IndentAreaAttribute instead.")]
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