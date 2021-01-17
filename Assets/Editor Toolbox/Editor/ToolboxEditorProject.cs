using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Folders;

    /// <summary>
    /// Static GUI representation for the Project Overlay.
    /// </summary>
    [InitializeOnLoad]
    public static class ToolboxEditorProject
    {
        static ToolboxEditorProject()
        {
            EditorApplication.projectWindowItemOnGUI -= OnItemCallback;
            EditorApplication.projectWindowItemOnGUI += OnItemCallback;
        }


        private readonly static Dictionary<string, FolderData> pathBasedFoldersData = new Dictionary<string, FolderData>();
        private readonly static Dictionary<string, FolderData> nameBasedFoldersData = new Dictionary<string, FolderData>();


        /// <summary>
        /// Draws icons and additional tooltips for matched assets.
        /// </summary>
        private static void OnItemCallback(string guid, Rect rect)
        {
            if (!IsOverlayAllowed)
            {
                return;
            }

            //try to get path to the asset using defined GUID
            var path = AssetDatabase.GUIDToAssetPath(guid);

            //try to determine if the found path has own data
            if (TryGetFolderData(path, out var data))
            {
                if (TryGetFolderIcon(data, rect, out var icon, out var iconRect))
                {
                    ToolboxEditorGui.DrawTexture(iconRect, icon);
                }

                ToolboxEditorGui.DrawTooltip(rect, data.Tooltip);
            }
        }


        /// <summary>
        /// Tries to retrive <see cref="FolderData"/> associated to given path.
        /// </summary>
        private static bool TryGetFolderData(string path, out FolderData data)
        {
            return pathBasedFoldersData.TryGetValue(path, out data) || nameBasedFoldersData.TryGetValue(Path.GetFileName(path), out data);
        }

        /// <summary>
        /// Tries to retrive proper icon for given <see cref="FolderData"/> and <see cref="Rect"/>.
        /// </summary>
        private static bool TryGetFolderIcon(FolderData data, Rect labelRect, out Texture icon, out Rect rect)
        {
            var isSmallIcon = labelRect.width > labelRect.height;
            icon = isSmallIcon ? data.SmallIcon : data.LargeIcon;

            //skip rect-related calculations if there is no icon
            if (!icon)
            {
                rect = labelRect;
                return false;
            }

            rect = isSmallIcon ? GetSmallIconRect(labelRect) : GetLargeIconRect(labelRect, true);
            return true;
        }


        /// <summary>
        /// Creates a custom folder using given data.
        /// </summary>
        internal static void CreateCustomFolder(FolderData data)
        {
            switch (data.DataType)
            {
                case FolderDataType.Path:
                    pathBasedFoldersData[data.Path] = data;
                    return;
                case FolderDataType.Name:
                    nameBasedFoldersData[data.Name] = data;
                    return;
            }
        }

        /// <summary>
        /// Removes a custom folder using given data.
        /// </summary>
        internal static bool RemoveCustomFolder(FolderData data)
        {
            switch (data.DataType)
            {
                case FolderDataType.Path:
                    return pathBasedFoldersData.Remove(data.Path);
                case FolderDataType.Name:
                    return nameBasedFoldersData.Remove(data.Name);
            }

            return false;
        }

        /// <summary>
        /// Removes all custom folders.
        /// </summary>
        internal static void ClearCustomFolders()
        {
            pathBasedFoldersData.Clear();
            nameBasedFoldersData.Clear();
        }


        internal static Rect GetLargeIconRect(Rect folderIconRect)
        {
            return GetLargeIconRect(folderIconRect, false);
        }

        internal static Rect GetLargeIconRect(Rect folderIconRect, bool clearLabel)
        {
            if (clearLabel)
            {
                folderIconRect.height -= EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing * 2;
            }

            //NOTE: in older versions of the Unity Editor icons are not scaling properly 
#if !UNITY_2019_3_OR_NEWER
            var w = Mathf.Min(folderIconRect.width, Defaults.maxFolderWidth);
            var h = Mathf.Min(folderIconRect.height, Defaults.maxFolderHeight);

            folderIconRect.x += (folderIconRect.width - w) / 2;
            folderIconRect.y += (folderIconRect.height - h) / 2;
            folderIconRect.width = w;
            folderIconRect.height = h;
#endif

            //calculate only base icon dimensions as:
            // - icon width without offset 
            // - icon height without offset
            var iconPlaceWidth = folderIconRect.width - folderIconRect.width
                                 * Defaults.folderWidthOffsetRatio;
            var iconPlaceHeight = folderIconRect.height - folderIconRect.height
                                  * Defaults.folderHeightOffsetRatio;

            var centerX = folderIconRect.xMin + folderIconRect.width / 2 - iconPlaceWidth / 2;
            var centerY = folderIconRect.yMin + folderIconRect.height / 2 - iconPlaceHeight / 2;

            //prepare final rect for the 'large' icon
            folderIconRect = new Rect(centerX, centerY, iconPlaceWidth, iconPlaceHeight);

            folderIconRect.x += (folderIconRect.width - folderIconRect.width * LargeIconScale) / 2;
            folderIconRect.y += (folderIconRect.height - folderIconRect.height * LargeIconScale) / 2;
            //adjust rect to the scale
            folderIconRect.width *= LargeIconScale;
            folderIconRect.height *= LargeIconScale;
            //adjust rect to the padding
            folderIconRect.x += folderIconRect.width * LargeIconPaddingRatio.x;
            folderIconRect.y += folderIconRect.height * LargeIconPaddingRatio.y;

            return folderIconRect;
        }

        internal static Rect GetSmallIconRect(Rect folderIconRect)
        {
            //prepare final rect for the 'small' icon
            folderIconRect = new Rect(folderIconRect.xMin, folderIconRect.y, Defaults.minFolderWidth, Defaults.minFolderHeight);

            folderIconRect.x += (folderIconRect.width - folderIconRect.width * SmallIconScale) / 2;
            folderIconRect.y += (folderIconRect.height - folderIconRect.height * SmallIconScale) / 2;
            //adjust rect to the scale
            folderIconRect.width *= SmallIconScale;
            folderIconRect.height *= SmallIconScale;
            //adjust rect to the padding
            folderIconRect.x += folderIconRect.width * SmallIconPaddingRatio.x;
            folderIconRect.y += folderIconRect.height * SmallIconPaddingRatio.y;

            return folderIconRect;
        }

        internal static void RepaintProjectOverlay() => EditorApplication.RepaintProjectWindow();


        /// <summary>
        /// Determines if <see cref="ToolboxEditorProject"/> can create an additional overlay on the Project Window.
        /// </summary>
        internal static bool IsOverlayAllowed { get; set; } = false;

        /// <summary>
        /// Scale ratio for the large icons.
        /// </summary>
        internal static float LargeIconScale { get; set; } = 1.0f;
        /// <summary>
        /// Scale ratio for the small icons.
        /// </summary>
        internal static float SmallIconScale { get; set; } = 1.0f;

        /// <summary>
        /// Padding ratio for the large icons.
        /// </summary>
        internal static Vector2 LargeIconPaddingRatio { get; set; } = new Vector2(0, 0);
        /// <summary>
        /// Padding ratio for the small icons.
        /// </summary>
        internal static Vector2 SmallIconPaddingRatio { get; set; } = new Vector2(0, 0);


        internal static class Defaults
        {
            /// <summary>
            /// Value based on clear space ratio between folder icon and width.
            /// </summary>
            internal const float folderWidthOffsetRatio = 0.25f;
            /// <summary>
            /// Value based on clear space ratio between folder icon and height.
            /// </summary>
            internal const float folderHeightOffsetRatio = 0.375f;

#if !UNITY_2019_3_OR_NEWER
            /// <summary>
            /// Value used in versions before 2019.3+ to determine maximal width of the folder icon.
            /// </summary>
            internal const float maxFolderWidth = 64.0f;
            /// <summary>
            /// Value used in versions before 2019.3+ to determine maximal height of the folder icon.
            /// </summary>
            internal const float maxFolderHeight = 64.0f;
#endif
            /// <summary>
            /// Minimal possible width of the folder icon.
            /// </summary>
            internal const float minFolderWidth = 16.0f;
            /// <summary>
            /// Minimal possible height of the folder icon.
            /// </summary>
            internal const float minFolderHeight = 16.0f;
        }
    }
}