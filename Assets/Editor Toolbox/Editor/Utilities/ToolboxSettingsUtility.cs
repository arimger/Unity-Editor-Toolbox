using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    internal static class ToolboxSettingsUtility
    {
        [InitializeOnLoadMethod]
        internal static void InitializeSettings()
        {
            const string versionFileName = "Version.txt";

            var guids = AssetDatabase.FindAssets("t:" + nameof(ToolboxEditorSettings));
            if (guids == null || guids.Length == 0) return;
            var settingsPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            Settings = AssetDatabase.LoadAssetAtPath<ToolboxEditorSettings>(settingsPath);
            var versionPath = settingsPath.Replace(Settings.name + ".asset", versionFileName);
            Version = AssetDatabase.LoadAssetAtPath<TextAsset>(versionPath).text;

            if (Settings == null)
            {
                Debug.LogWarning("ToolboxEditorSettings not found. Cannot initialize Toolbox core functionalities. " +
                                 "You can create new settings file using CreateAsset menu -> Create -> Toolbox Editor -> Settings.");
                return;
            }

            ToolboxDrawerUtility.InitializeDrawers(Settings);
            ToolboxFolderUtility.InitializeProject(Settings);
        }


        internal static void ReimportSettings()
        {
            var guids = AssetDatabase.FindAssets("ToolboxSettingsUtility");
            if (guids == null || guids.Length == 0) return;
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            AssetDatabase.ImportAsset(path);
        }


        internal static ToolboxEditorSettings Settings { get; private set; }

        internal static string Version { get; private set; } = "1.0.0";
    }
}