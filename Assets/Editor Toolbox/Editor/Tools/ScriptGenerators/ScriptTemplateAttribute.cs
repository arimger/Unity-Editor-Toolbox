using System;

namespace Toolbox.Editor.Tools
{
    /// <summary>
    /// Marks class as script template. Every marked class can be used as template in
    /// scrip generation process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ScriptTemplateAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new <see cref="ScriptTemplateAttribute"/> instance.
        /// </summary>
        /// <param name="templateName">Description of script template.</param>
        /// <param name="priority">Provides some control over ordering in user interfaces</param>
        public ScriptTemplateAttribute(string templateName, int priority = 1000)
        {
            TemplateName = templateName;
            Priority = priority;
        }

        /// <summary>
        /// Gets priority of template generator providing control over ordering in user interfaces.
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// Gets description of script template.
        /// </summary>
        public string TemplateName { get; private set; }
    }
}