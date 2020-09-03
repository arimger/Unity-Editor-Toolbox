using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class ToolboxSelfPropertyAttribute : ToolboxPropertyAttribute
    { }
}