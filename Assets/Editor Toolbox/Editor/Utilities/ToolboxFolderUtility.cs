using UnityEngine;

namespace Toolbox.Editor
{
    internal static class ToolboxFolderUtility
    {
        internal static bool IsCustomFolder(string path)
        {
            return ToolboxEditorUtility.IsCustomFolder(path);
        }

        internal static bool TryGetFolderData(string path, out FolderData data)
        {
            return ToolboxEditorUtility.TryGetFolderData(path, out data);
        }

        internal static FolderData GetFolderData(string path)
        {
            ToolboxEditorUtility.TryGetFolderData(path, out FolderData data);
            return data;
        }


        internal static bool ToolboxFoldersAllowed => ToolboxEditorUtility.ToolboxFoldersAllowed;
    }
}