using UnityEngine;

namespace Toolbox.Editor
{
    internal static class ToolboxFolderUtility
    {
        internal static bool IsCustomFolder(string path)
        {
            return ToolboxEditorUtility.IsCustomFolder(path);
        }

        internal static bool TryGetFolderIcon(string path, out Texture icon)
        {
            return ToolboxEditorUtility.TryGetFolderIcon(path, out icon);
        }

        internal static Texture GetFolderIcon(string path)
        {
            ToolboxEditorUtility.TryGetFolderIcon(path, out Texture icon);
            return icon;
        }


        internal static bool ToolboxFoldersAllowed => ToolboxEditorUtility.ToolboxProjectAllowed;
    }


    [System.Serializable]
    public struct FolderIcon
    {
        [SerializeField, Directory, Tooltip("Relative path from Assets directory.")]
        private string path;
        [SerializeField, AssetPreview]
        private Texture icon;


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
    }
}