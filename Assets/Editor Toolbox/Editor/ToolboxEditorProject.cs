using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    public static class ToolboxEditorProject
    {
        static ToolboxEditorProject()
        {
            EditorApplication.projectWindowItemOnGUI += OnItemCallback;
        }


        private static void OnItemCallback(string guid, Rect rect)
        {
            //ignore drawing if ToolboxEditorProject functionalites are not allowed
            if (!ToolboxFolderUtility.ToolboxFoldersAllowed)
            {
                return;
            }

            //ignore drawing if current callback relates to tree view
            if (rect.width > rect.height)
            {
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(guid);

            //try to get icon for this folder
            if (!ToolboxFolderUtility.TryGetFolderIcon(path, out Texture icon))
            {
                return;
            }

            //adjust rect for icon
            rect.x += rect.width * Style.xToWidthRatio;
            rect.y += rect.height * Style.yToHeightRatio;
            rect.width = Style.iconWidth;
            rect.height = Style.iconHeight;

            GUI.DrawTexture(rect, icon);
        }


        internal static class Style
        {
            internal static readonly float xToWidthRatio = 0.25f;
            internal static readonly float yToHeightRatio = 0.25f;
            internal static readonly float iconWidth = 28.0f;
            internal static readonly float iconHeight = 28.0f;
        }
    }
}