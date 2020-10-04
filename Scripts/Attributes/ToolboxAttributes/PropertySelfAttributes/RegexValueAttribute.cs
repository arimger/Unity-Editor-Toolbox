using System;

namespace UnityEngine
{
    /// <summary>
    /// Validates associated <see cref="string"/> using regular expression and given pattern.
    /// Supported types: any <see cref="string"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class RegexValueAttribute : ToolboxSelfPropertyAttribute
    {
        public RegexValueAttribute(string pattern) : this(pattern, null)
        { }

        public RegexValueAttribute(string pattern, string message)
        {
            Pattern = pattern;
            Message = message;
        }

        public string Pattern { get; private set; }
        public string Message { get; private set; }
    }
}