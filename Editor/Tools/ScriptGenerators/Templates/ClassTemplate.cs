namespace Toolbox.Editor.Tools
{
    /// <summary>
    /// Template generator for basic C# class.
    /// </summary>
    [ScriptTemplate("C# Class")]
    public sealed class ClassTemplate : BasicTemplate
    {
        /// <summary>
        /// Initialize new <see cref="ClassTemplate"/> instance.
        /// </summary>
        public ClassTemplate() : base("class")
        { }
    }
}