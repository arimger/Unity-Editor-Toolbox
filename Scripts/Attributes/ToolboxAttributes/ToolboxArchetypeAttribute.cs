using System;

namespace UnityEngine
{
    /// <summary>
    /// Base class for all attributes responsible for the creation of dedicated composition of <see cref="ToolboxAttribute"/>s.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public abstract class ToolboxArchetypeAttribute : ToolboxAttribute
    {
        public abstract ToolboxAttribute[] Process();
    }
}