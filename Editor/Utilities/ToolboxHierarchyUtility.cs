namespace Toolbox.Editor
{
    internal static class ToolboxHierarchyUtility
    {
        //TODO:
        internal static bool ToolboxHierarchyAllowed => ToolboxSettingsUtility.Settings ? ToolboxSettingsUtility.Settings.UseToolboxHierarchy : false;
    }
}