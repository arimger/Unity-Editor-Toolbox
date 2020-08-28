using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public abstract class ToolboxDecoratorAttribute : ToolboxAttribute
    {
        public int Order { get; set; }
    }
}