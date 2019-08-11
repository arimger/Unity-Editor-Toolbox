namespace Toolbox.Editor.Tools
{
	/// <summary>
	/// Template generator for basic C# struct.
	/// </summary>
	[ScriptTemplate("C# Struct")]
	public sealed class StructTemplate : BasicTemplate
    {
		/// <summary>
		/// Initialize new <see cref="StructTemplate"/> instance.
		/// </summary>
		public StructTemplate() : base("struct")
        { }
	}
}