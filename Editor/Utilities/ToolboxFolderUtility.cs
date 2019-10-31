using System.Collections.Generic;

namespace Toolbox.Editor
{
    internal static class ToolboxFolderUtility
    {
        private readonly static Dictionary<string, FolderData> foldersData = new Dictionary<string, FolderData>();


        internal static void InitializeProject(ToolboxEditorSettings settings)
        {
            for (var i = 0; i < settings.CustomFoldersCount; i++)
            {
                var customFolder = settings.GetCustomFolderAt(i);
                foldersData.Add(customFolder.Path, customFolder);
            }
        }


        internal static bool IsCustomFolder(string path)
        {
            return foldersData.ContainsKey(path);
        }

        internal static bool TryGetFolderData(string path, out FolderData data)
        {
            return foldersData.TryGetValue(path, out data);
        }

        internal static FolderData GetFolderData(string path)
        {
            TryGetFolderData(path, out FolderData data);
            return data;
        }


        internal static bool ToolboxFoldersAllowed => ToolboxSettingsUtility.ToolboxFoldersAllowed;
    }
}