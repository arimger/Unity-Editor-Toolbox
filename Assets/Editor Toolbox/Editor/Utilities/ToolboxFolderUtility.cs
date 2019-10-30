namespace Toolbox.Editor
{
    internal static class ToolboxFolderUtility
    {
        internal static bool IsCustomFolder(string path)
        {
            return ToolboxSettingsUtility.IsCustomFolder(path);
        }

        internal static bool TryGetFolderData(string path, out FolderData data)
        {
            return ToolboxSettingsUtility.TryGetFolderData(path, out data);
        }

        internal static FolderData GetFolderData(string path)
        {
            ToolboxSettingsUtility.TryGetFolderData(path, out FolderData data);
            return data;
        }


        internal static bool ToolboxFoldersAllowed => ToolboxSettingsUtility.ToolboxFoldersAllowed;
    }
}