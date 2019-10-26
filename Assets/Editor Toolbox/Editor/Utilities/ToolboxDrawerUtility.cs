namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    public static class ToolboxDrawerUtility
    {
        internal static ToolboxAreaDrawerBase GetAreaDrawer<T>(T attribute) where T : UnityEngine.ToolboxAreaAttribute
        {
            return ToolboxEditorUtility.GetAreaDrawer(attribute);
        }

        internal static ToolboxPropertyDrawerBase GetPropertyDrawer<T>(T attribute) where T : UnityEngine.ToolboxPropertyAttribute
        {
            return ToolboxEditorUtility.GetPropertyDrawer(attribute);
        }

        internal static ToolboxConditionDrawerBase GetConditionDrawer<T>(T attribute) where T : UnityEngine.ToolboxConditionAttribute
        {
            return ToolboxEditorUtility.GetConditionDrawer(attribute);
        }


        internal static bool ToolboxDrawersAllowed => ToolboxEditorUtility.ToolboxDrawersAllowed;
    }
}