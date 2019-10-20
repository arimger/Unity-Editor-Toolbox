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

            var path = AssetDatabase.GUIDToAssetPath(guid);

            //try to get icon for this folder
            if (!ToolboxFolderUtility.TryGetFolderData(path, out FolderData data))
            {
                return;
            }

            //adjust rect for icon depending on current view and handled texture
            if (rect.width > rect.height)
            {
                if (data.SmallIcon == null) return;

                rect.x += rect.width * Style.xToWidthRatioSmall;
                rect.y += rect.height * Style.yToHeightRatioSmall;
                rect.width = Style.iconWidthSmall;
                rect.height = Style.iconHeightSmall;

                GUI.DrawTexture(rect, data.SmallIcon);
            }
            else
            {
                if (data.Icon == null) return;

                rect.x += rect.width * Style.xToWidthRatio;
                rect.y += rect.height * Style.yToHeightRatio;
                rect.width = Style.iconWidth;
                rect.height = Style.iconHeight;

                GUI.DrawTexture(rect, data.Icon);
            }
        }


        internal static class Style
        {
            //x ratio will determinate OX position of icon
            internal const float xToWidthRatio = 0.43f;
            internal const float xToWidthRatioSmall = 0.025f;

            //y ratio will determinate OY position of icon
            internal const float yToHeightRatio = 0.25f;
            internal const float yToHeightRatioSmall = 0.4f;

            //icon width used to draw texture in proper rect
            internal const float iconWidth = 29.0f;
            internal const float iconWidthSmall = 10.0f;

            //icon height used to draw texture in proper rect
            internal const float iconHeight = 29.0f;
            internal const float iconHeightSmall = 10.0f;
        }
    }


    [System.Serializable]
    public struct FolderData
    {
        [SerializeField, Directory, Tooltip("Relative path from Assets directory.")]
        private string path;
        [SerializeField, AssetPreview(ToolboxEditorProject.Style.iconWidth, ToolboxEditorProject.Style.iconHeight)]
        private Texture icon;
        [SerializeField, AssetPreview(ToolboxEditorProject.Style.iconWidthSmall, ToolboxEditorProject.Style.iconHeightSmall)]
        private Texture smallIcon;


        public string Path
        {
            get => "Assets/" + path;
            set => path = value;
        }

        public Texture Icon
        {
            get => icon;
            set => icon = value;
        }

        public Texture SmallIcon
        {
            get => smallIcon;
            set => smallIcon = value;
        }
    }
}