using System;

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

            if (!string.IsNullOrEmpty(data.Tooltip))
            {
                //create additional tooltip to mark custom folder
                EditorGUI.LabelField(rect, new GUIContent(string.Empty, data.Tooltip));
            }

            Texture icon;
            //adjust rect for icon depending on current view and handled texture
            if (rect.width > rect.height)
            {
                icon = data.SmallIcon;

                //determinate exact folder icon rect
                rect = new Rect(rect.xMin, rect.y, Style.folderIconWidthSmall, Style.folderIconHeightSmall);
                
                rect.x += Style.padding;
                rect.y += rect.height * Style.yToHeightRatioSmall;
                rect.width = Style.iconWidthSmall;
                rect.height = Style.iconHeightSmall;
            }
            else
            {
                icon = data.Icon;

                var offset = Mathf.Max(rect.height - Style.labelHeight - Style.folderIconHeight, 0);
                var iconHeight       = rect.height - Style.labelHeight - offset;

                //determinate exact folder icon rect
                rect = new Rect(rect.x, rect.yMin + offset / 2 + Style.spacing, rect.width, iconHeight);

                //calculate multipliers
                var widthRatio = iconHeight / Style.folderIconWidth;
                var heightRatio = iconHeight / Style.folderIconHeight;

                //set final rect
                rect.x += rect.width * Style.xToWidthRatio;
                rect.y += rect.height * Style.yToHeightRatio;
                rect.width = Style.iconWidth * widthRatio;
                rect.height = Style.iconHeight * heightRatio;         
            }

            if (icon == null) return;

            //finally, draw retrieved icon
            GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit, true);

        }

        //TODO: refactor needed
        internal static class Style
        {
            internal const float spacing = 2.0f;
            internal const float padding = 5.5f;
            internal const float labelHeight = 16.0f;

            //x ratio will determinate OX position of icon
            internal const float xToWidthRatio = 0.45f;
            [System.Obsolete("Use padding instead.")]
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

    public enum FolderDataType
    {
        Path,
        Name
    }

    [Serializable]
    public struct FolderData
    {
        [SerializeField]
        private FolderDataType type;

        [SerializeField, HideIf(nameof(type), FolderDataType.Name)]
        private string name;
        [SerializeField, HideIf(nameof(type), FolderDataType.Path), Directory, Tooltip("Relative path from Assets directory.")]
        private string path;

        [SerializeField, Tooltip("Will create additional tooltip for custom folders. Leave empty to ignore.")]
        private string tooltip;

        [SerializeField]
        private Texture icon;
        [SerializeField]
        private Texture smallIcon;


        public FolderDataType Type
        {
            get => type;
            set => type = value;
        }

        public string Path
        {
            get => "Assets/" + path;
            set => path = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Tooltip
        {
            get => tooltip;
            set => tooltip = value;
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