using System;

namespace UnityEngine
{
    /// <summary>
    /// Begins indentation group in inspector. Use <seealso cref="EndIndentAttribute"/> on serialized property to end this group.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class BeginIndentAttribute : ToolboxDecoratorAttribute
    {
        public BeginIndentAttribute(int indentToAdd = 1)
        {
            IndentToAdd = indentToAdd;
        }

        public int IndentToAdd { get; private set; }
    }
}