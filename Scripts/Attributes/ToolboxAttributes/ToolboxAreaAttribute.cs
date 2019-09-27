using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public abstract class ToolboxAreaAttribute : ToolboxAttribute
    {
        public int Order { get; set; }
    }
}