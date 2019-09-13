namespace Toolbox.Editor.Drawers
{
    public class ToolboxConditionDrawer<T> : ToolboxConditionDrawerBase where T : ToolboxAttribute
    {
        /// <summary>
        /// Attribute type associated with this drawer.
        /// </summary>
        public static System.Type GetAttributeType() => typeof(T);
    }
}