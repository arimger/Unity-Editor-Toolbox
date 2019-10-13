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

            //adjust rect for icon
            if (rect.width > rect.height)
            {
                rect.x += rect.width * Style.xToWidthRatioSmall;
                rect.y += rect.height * Style.yToHeightRatioSmall;
                rect.width = Style.iconWidthSmall;
                rect.height = Style.iconHeightSmall;

                if (data.SmallIcon == null) return;
                
                GUI.DrawTexture(rect, data.SmallIcon);
            }
            else
            {
                rect.x += rect.width * Style.xToWidthRatio;
                rect.y += rect.height * Style.yToHeightRatio;
                rect.width = Style.iconWidth;
                rect.height = Style.iconHeight;

                if (data.Icon == null) return;

                GUI.DrawTexture(rect, data.Icon);
            }
        }


        internal static class Style
        {
            internal static readonly float xToWidthRatio = 0.25f;
            internal static readonly float xToWidthRatioSmall = 0.025f;
            internal static readonly float yToHeightRatio = 0.3f;
            internal static readonly float yToHeightRatioSmall = 0.45f;
            internal static readonly float iconWidth = 28.0f;
            internal static readonly float iconWidthSmall = 7.0f;
            internal static readonly float iconHeight = 28.0f;
            internal static readonly float iconHeightSmall = 7.0f;
        }
    }


    [System.Serializable]
    public struct FolderData
    {
        [SerializeField, Directory, Tooltip("Relative path from Assets directory.")]
        private string path;
        [SerializeField, AssetPreview(28.0f, 28.0f)]
        private Texture icon;
        [SerializeField, AssetPreview(7.0f, 7.0f)]
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