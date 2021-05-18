using System;

using UnityEngine;
using UnityEngine.Serialization;

namespace Toolbox.Editor.Folders
{
    public enum FolderDataType
    {
        Path,
        Name
    }

    public enum FolderIconType
    {
        Custom,
        Editor
    }

    [Serializable]
    public struct FolderData
    {
        //NOTE: all additional attributes are overrided by the custom FolderDataDrawer

        [SerializeField, FormerlySerializedAs("type")]
        private FolderDataType dataType;

        [SerializeField, HideIf(nameof(dataType), FolderDataType.Name)]
        private string name;
        [SerializeField, HideIf(nameof(dataType), FolderDataType.Path), Directory]
        [Tooltip("Relative path from Assets directory.")]
        private string path;

        [SerializeField, TextArea(minLines: 0, maxLines: 3)]
        [Tooltip("Will create additional tooltip for custom folders. Leave empty to ignore.")]
        private string tooltip;

        [SerializeField]
        private FolderIconType iconType;

        [SerializeField, HideIf(nameof(iconType), FolderIconType.Editor), FormerlySerializedAs("icon")]
        private Texture largeIcon;
        [SerializeField, HideIf(nameof(iconType), FolderIconType.Editor)]
        private Texture smallIcon;

        [SerializeField, HideIf(nameof(iconType), FolderIconType.Custom)]
        private string iconName;


        public FolderDataType DataType
        {
            get => dataType;
            set => dataType = value;
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

        public FolderIconType IconType
        {
            get => iconType;
            set => iconType = value;
        }

        public Texture LargeIcon
        {
            get => largeIcon;
            set => largeIcon = value;
        }

        public Texture SmallIcon
        {
            get => smallIcon;
            set => smallIcon = value;
        }

        public string IconName
        {
            get => iconName;
            set => iconName = value;
        }
    }
}