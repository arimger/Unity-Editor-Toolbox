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

                //determinate exact folder icon rect
                rect = new Rect(rect.xMin, rect.y, Style.folderIconWidthSmall, Style.folderIconHeightSmall);

                rect.x += rect.width * Style.xToWidthRatioSmall;
                rect.y += rect.height * Style.yToHeightRatioSmall;
                rect.width = Style.iconWidthSmall;
                rect.height = Style.iconHeightSmall;

                GUI.DrawTexture(rect, data.SmallIcon);
            }
            else
            {
                if (data.Icon == null) return;

                //determinate exact folder icon rect
                rect = new Rect(rect.x, rect.yMin, Style.folderIconWidth, Style.folderIconHeight);
       
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
            internal const float xToWidthRatioSmall = 0.25f;

            //y ratio will determinate OY position of icon
            internal const float yToHeightRatio = 0.3f;
            internal const float yToHeightRatioSmall = 0.3f;

            //big icon dimensions
            internal const float iconWidth = 29.0f;
            internal const float iconHeight = 29.0f;

            //small icon dimensions
            internal const float iconWidthSmall = 10.0f;
            internal const float iconHeightSmall = 10.0f;

            //big folder icon dimensions
            internal const float folderIconWidth = 64.0f;
            internal const float folderIconHeight = 64.0f;

            //small folder icon dimensions
            internal const float folderIconWidthSmall = 16.0f;
            internal const float folderIconHeightSmall = 16.0f;
        }
    }


    [System.Serializable]
    public struct FolderData
    {
        [SerializeField, Directory, Tooltip("Relative path from Assets directory.")]
        private string path;
        [SerializeField]
        private Texture icon;
        [SerializeField]
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