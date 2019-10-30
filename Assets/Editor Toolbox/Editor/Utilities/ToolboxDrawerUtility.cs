using UnityEditor;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    internal static class ToolboxDrawerUtility
    {
        [InitializeOnLoadMethod]
        internal static void InitializeEvents()
        {
            Selection.selectionChanged += onEditorReload;
        }


        internal static ToolboxAreaDrawerBase GetAreaDrawer<T>(T attribute) where T : UnityEngine.ToolboxAreaAttribute
        {
            return ToolboxSettingsUtility.GetAreaDrawer(attribute);
        }

        internal static ToolboxPropertyDrawerBase GetPropertyDrawer<T>(T attribute) where T : UnityEngine.ToolboxPropertyAttribute
        {
            return ToolboxSettingsUtility.GetPropertyDrawer(attribute);
        }

        internal static ToolboxPropertyDrawerBase GetCollectionDrawer<T>(T attribute) where T : UnityEngine.ToolboxCollectionAttribute
        {
            return ToolboxSettingsUtility.GetCollectionDrawer(attribute);
        }

        internal static ToolboxConditionDrawerBase GetConditionDrawer<T>(T attribute) where T : UnityEngine.ToolboxConditionAttribute
        {
            return ToolboxSettingsUtility.GetConditionDrawer(attribute);
        }


        internal static bool ToolboxDrawersAllowed => ToolboxSettingsUtility.ToolboxDrawersAllowed;


        internal static System.Action onEditorReload;
    }
}