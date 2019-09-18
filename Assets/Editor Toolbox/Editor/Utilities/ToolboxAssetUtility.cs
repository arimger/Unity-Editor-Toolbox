using System.IO;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public static class ToolboxAssetUtility
    {
        [InitializeOnLoadMethod]
        internal static void InitializeUtility()
        {
            var dirs = Directory.GetDirectories(Application.dataPath, "Editor Toolbox", SearchOption.AllDirectories);
            if (dirs == null || dirs.Length == 0) return;

            ResourcesPath = dirs[0].Replace('\\', '/') + "/Editor Resources";
            ResourcesPathRelative = ResourcesPath.Replace(Application.dataPath, "Assets");
        }


        internal static T LoadEditorAsset<T>(string assetName) where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath(ResourcesPathRelative + "/" + assetName, typeof(T)) as T;
        }


        internal static string ResourcesPath { get; private set; }

        internal static string ResourcesPathRelative { get; private set; }
    }
}