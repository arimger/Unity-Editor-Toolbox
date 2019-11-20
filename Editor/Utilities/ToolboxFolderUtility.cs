using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Toolbox.Editor
{
    internal static class ToolboxFolderUtility
    {
        /// <summary>
        /// All custom folders mapped with own path relative to Asset directory.
        /// </summary>
        private readonly static Dictionary<string, FolderData> pathBasedFoldersData = new Dictionary<string, FolderData>();
        /// <summary>
        /// All custom folders mapped with name.
        /// </summary>
        private readonly static Dictionary<string, FolderData> nameBasedFoldersData = new Dictionary<string, FolderData>();


        /// <summary>
        /// Settings provided to handle custom folders.
        /// </summary>
        private static IToolboxProjectSettings settings;


        internal static void InitializeProject(IToolboxProjectSettings settings)
        {
            ToolboxFolderUtility.settings = settings;

            for (var i = 0; i < settings.CustomFoldersCount; i++)
            {
                var folderData = settings.GetCustomFolderAt(i);

                switch (folderData.Type)
                {
                    case FolderDataType.Path: pathBasedFoldersData.Add(folderData.Path, folderData);
                        break;
                    case FolderDataType.Name: nameBasedFoldersData.Add(folderData.Name, folderData);
                        break;
                }
            }
        }


        internal static bool IsCustomFolder(string path)
        {
            return pathBasedFoldersData.ContainsKey(path) ||
                   nameBasedFoldersData.ContainsKey(Path.GetFileName(path));
        }

        internal static bool TryGetFolderData(string path, out FolderData data)
        {
            return pathBasedFoldersData.TryGetValue(path, out data) ||
                   nameBasedFoldersData.TryGetValue(Path.GetFileName(path), out data);
        }

        internal static FolderData GetFolderData(string path)
        {
            TryGetFolderData(path, out FolderData data);
            return data;
        }


        internal static bool ToolboxFoldersAllowed => settings != null ? settings.UseToolboxProject : false;
    }
}