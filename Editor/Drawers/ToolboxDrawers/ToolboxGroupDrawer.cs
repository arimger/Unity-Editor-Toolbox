namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxGroupDrawer<T> : ToolboxGroupDrawerBase where T : ToolboxGroupAttribute
    {
        /// <summary>
        /// Attribute type associated with this drawer.
        /// </summary>
        public static System.Type GetAttributeType() => typeof(T);
    }
}
