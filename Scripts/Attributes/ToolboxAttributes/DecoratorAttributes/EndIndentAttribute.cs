using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EndIndentAttribute : ToolboxDecoratorAttribute
    {
        public EndIndentAttribute(int indentToSubtract = 1)
        {
            IndentToSubtract = indentToSubtract;
        }

        public int IndentToSubtract { get; private set; }
    }
}