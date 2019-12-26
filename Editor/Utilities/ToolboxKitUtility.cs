using System.IO;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    internal static class ToolboxKitUtility
    {
        private static readonly string toolkitName = "Editor Toolbox";


        static ToolboxKitUtility()
        {
            //TODO:
            var directories = Directory.GetDirectories(Application.dataPath, toolkitName, SearchOption.AllDirectories);
            if (directories == null || directories.Length == 0) return;

            var toolkitDirectoryPath = directories[0].Replace('\\', '/').Replace(Application.dataPath, "Assets");
            var toolkitVersionAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(toolkitDirectoryPath + "/Version.txt");
            if (toolkitVersionAsset)
            {
                Version = toolkitVersionAsset.text;
            }
        }


        internal static string Version
        {
            get; private set;
        }
    }
}
