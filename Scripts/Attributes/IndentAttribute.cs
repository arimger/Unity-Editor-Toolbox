using System;

namespace UnityEngine
{
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
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