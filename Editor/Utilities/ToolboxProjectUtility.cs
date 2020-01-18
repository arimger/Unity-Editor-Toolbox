using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace Toolbox.Editor
{
    /// <summary>
    /// Utility class to handle Project Window related data.
    /// </summary>
    internal static class ToolboxProjectUtility
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
        /// Settings provided to handle Project Window overlay.
        /// </summary>
        private static IToolboxProjectSettings settings;


        private static void CreateCustomFolders(IToolboxProjectSettings settings)
        {
            ClearCustomFolders();

            for (var i = 0; i < settings.CustomFoldersCount; i++)
            {
                var folderData = settings.GetCustomFolderAt(i);

                switch (folderData.Type)
                {
                    case FolderDataType.Path:
                        if (pathBasedFoldersData.ContainsKey(folderData.Path))
                        {
                            continue;
                        }
                        pathBasedFoldersData.Add(folderData.Path, folderData);
                        break;
                    case FolderDataType.Name:
                        if (nameBasedFoldersData.ContainsKey(folderData.Name))
                        {
                            continue;
                        }
                        nameBasedFoldersData.Add(folderData.Name, folderData);
                        break;
                }
            }
        }

        private static void ClearCustomFolders()
        {
            pathBasedFoldersData.Clear();
            nameBasedFoldersData.Clear();
        }


        internal static void PerformData()
        {
            if (settings == null)
            {
                return;              
            }

            PerformData(settings);
        }

        internal static void PerformData(IToolboxProjectSettings settings)
        {
            ToolboxProjectUtility.settings = settings;

            CreateCustomFolders(settings);

            ToolboxProjectAllowed = settings.UseToolboxProject;

            LargeIconScale = settings.LargeIconScale;
            SmallIconScale = settings.SmallIconScale;
            LargeIconPaddingRatio = settings.LargeIconPadding;
            SmallIconPaddingRatio = settings.SmallIconPadding;
        }


        /// <summary>
        /// Checks if provided path has associated custom <see cref="FolderData"/>.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static bool IsCustomFolder(string path)
        {
            return pathBasedFoldersData.ContainsKey(path) ||
                   nameBasedFoldersData.ContainsKey(Path.GetFileName(path));
        }

        /// <summary>
        /// Tries to retrieve custom <see cref="FolderData"/> for provided path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static bool TryGetFolderData(string path, out FolderData data)
        {
            return pathBasedFoldersData.TryGetValue(path, out data) ||
                   nameBasedFoldersData.TryGetValue(Path.GetFileName(path), out data);
        }

        /// <summary>
        /// Returns custom <see cref="FolderData"/> associated to provided path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static FolderData GetFolderData(string path)
        {
            TryGetFolderData(path, out FolderData data);
            return data;
        }


        /// <summary>
        /// Returns true if Project overlay is expected by the user.
        /// </summary>
        internal static bool ToolboxProjectAllowed { get; private set; }

        /// <summary>
        /// Scale ratio for large icons.
        /// </summary>
        internal static float LargeIconScale { get; set; } = 1.0f;
        /// <summary>
        /// Scale ratio for small icons.
        /// </summary>
        internal static float SmallIconScale { get; set; } = 1.0f;

        /// <summary>
        /// Padding ratio for large icons.
        /// </summary>
        internal static Vector2 LargeIconPaddingRatio { get; set; } = new Vector2(0, 0);
        /// <summary>
        /// Padding ratio for small icons.
        /// </summary>
        internal static Vector2 SmallIconPaddingRatio { get; set; } = new Vector2(0, 0);
    }
}