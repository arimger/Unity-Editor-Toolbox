using System;
using System.Collections;
using System.Collections.Generic;

namespace Toolbox.Editor
{
    internal static class ToolboxFolderUtility
    {
        /// <summary>
        /// All custom folders mapped with own path relative to Asset directory.
        /// </summary>
        private readonly static Dictionary<string, FolderData> foldersData = new Dictionary<string, FolderData>();


        /// <summary>
        /// Settings provided to handle custom folders.
        /// </summary>
        private static IToolboxProjectSettings settings;


        internal static void InitializeProject(IToolboxProjectSettings settings)
        {
            ToolboxFolderUtility.settings = settings;

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


        internal static bool ToolboxFoldersAllowed => settings != null ? settings.UseToolboxProject : false;
    }
}