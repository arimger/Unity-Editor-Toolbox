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


        private static readonly float labelHeight = EditorGUIUtility.singleLineHeight;
        private static readonly float labelSpacing = EditorGUIUtility.standardVerticalSpacing;

        /// <summary>
        /// Value based on clear space ratio between folder icon and width.
        /// </summary>
        internal const float usualFolderWidthOffsetRatio = 0.25f;
        /// <summary>
        /// Value based on clear space ratio between folder icon and height.
        /// </summary>
        internal const float usualFolderHeightOffsetRatio = 0.375f;

        /// <summary>
        /// Value used in versions before 2019.3+ to determine maximal width of folder icon.
        /// </summary>
        internal const float maximalFolderWidth = 64.0f;
        /// <summary>
        /// Value used in versions before 2019.3+ to determine maximal height of folder icon.
        /// </summary>
        internal const float maximalFolderHeight = 64.0f;
        /// <summary>
        /// Minimal possible width of folder icon.
        /// </summary>
        internal const float minimalFolderWidth = 16.0f;
        /// <summary>
        /// Minimal possible height of folder icon.
        /// </summary>
        internal const float minimalFolderHeight = 16.0f;

        internal const float minimalFolderIconWidth = 10.0f;
        internal const float minimalFolderIconHeight = 10.0f;


        private const float smallFolderWidthPaddingRatioDefault = 0.0f;
        private const float smallFolderHeightPaddingRatioDefault = 0.0f;

        private const float usualFolderWidthPaddingRatioDefault = 0.0f;
        private const float usualFolderHeightPaddingRatioDefault = 0.15f;
        private const float usualFolderIconPlaceToIconRatioDefault = 0.8f;


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
                rect = GetSmallIconRect(rect);
            }
            else
            {
                icon = data.Icon;
                rect = GetUsualIconRect(rect, true);
            }

            if (icon == null) return;

            //finally, draw retrieved icon
            GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit, true);
        }


        internal static Rect GetUsualIconRect(Rect folderIconRect)
        {
            return GetUsualIconRect(folderIconRect, false);
        }

        internal static Rect GetUsualIconRect(Rect folderIconRect, bool clearLabel)
        {
            if (clearLabel)
            {
                folderIconRect.height -= labelHeight - labelSpacing;
            }

            //NOTE: in older versions of Unity folder icon is not scaling properly 
#if !UNITY_2019_3_OR_NEWER
            var width = Mathf.Min(folderIconRect.width, maximalFolderWidth);
            var height = Mathf.Min(folderIconRect.height, maximalFolderHeight);
            folderIconRect.x += (folderIconRect.width - width) / 2;
            folderIconRect.y += (folderIconRect.height - height) / 2;
            folderIconRect.width = width;
            folderIconRect.height = height;
#endif

            //calculate only base icon dimensions as:
            // - icon width without offset 
            // - icon height without offset
            var iconPlaceWidth = folderIconRect.width - folderIconRect.width * usualFolderWidthOffsetRatio;
            var iconPlaceHeight = folderIconRect.height - folderIconRect.height * usualFolderHeightOffsetRatio;
            var centerX = folderIconRect.xMin + folderIconRect.width / 2 - iconPlaceWidth / 2;
            var centerY = folderIconRect.yMin + folderIconRect.height / 2 - iconPlaceHeight / 2;

            folderIconRect = new Rect(centerX, centerY, iconPlaceWidth, iconPlaceHeight);

            folderIconRect.x += (folderIconRect.width - folderIconRect.width * UsualIconScale) / 2;
            folderIconRect.y += (folderIconRect.height - folderIconRect.height * UsualIconScale) / 2;
            folderIconRect.width *= UsualIconScale;
            folderIconRect.height *= UsualIconScale;
            folderIconRect.x += folderIconRect.width * UsualIconXPaddingRatio;
            folderIconRect.y += folderIconRect.height * UsualIconYPaddingRatio;

            return folderIconRect;
        }

        internal static Rect GetSmallIconRect(Rect folderIconRect)
        {
            folderIconRect = new Rect(folderIconRect.xMin, folderIconRect.y, minimalFolderIconWidth, minimalFolderIconHeight);

            folderIconRect.x = folderIconRect.xMin + folderIconRect.width / 2;
            folderIconRect.y = folderIconRect.yMin + folderIconRect.height / 2;
            folderIconRect.x += folderIconRect.width * SmallIconXPaddingRatio;
            folderIconRect.y += folderIconRect.height * SmallIconYPaddingRatio;

            return folderIconRect;
        }

        internal static void ResetIconProperties()
        {
            UsualIconScale = usualFolderIconPlaceToIconRatioDefault;

            UsualIconXPaddingRatio = usualFolderWidthPaddingRatioDefault;
            UsualIconYPaddingRatio = usualFolderHeightPaddingRatioDefault;
            SmallIconXPaddingRatio = smallFolderWidthPaddingRatioDefault;
            SmallIconYPaddingRatio = smallFolderHeightPaddingRatioDefault;
        }


        internal static float UsualIconScale { get; set; } = usualFolderIconPlaceToIconRatioDefault;

        internal static float UsualIconXPaddingRatio { get; set; } = usualFolderWidthPaddingRatioDefault;
        internal static float UsualIconYPaddingRatio { get; set; } = usualFolderHeightPaddingRatioDefault;
        internal static float SmallIconXPaddingRatio { get; set; } = smallFolderWidthPaddingRatioDefault;
        internal static float SmallIconYPaddingRatio { get; set; } = smallFolderHeightPaddingRatioDefault;
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