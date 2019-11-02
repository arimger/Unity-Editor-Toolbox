namespace Toolbox.Editor
{
    internal static class ToolboxHierarchyUtility
    {
        internal static bool ToolboxHierarchyAllowed => ToolboxSettingsUtility.Settings.UseToolboxHierarchy;
    }
}