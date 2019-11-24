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
            const string settingsFileExt = ".asset";
            const string versionFileName = "Version.txt";

            //find all possible settings files in whole project solution
            var guids = AssetDatabase.FindAssets("t:" + nameof(ToolboxEditorSettings));

            if (guids == null || guids.Length == 0) return;

            //try to get proper path for first settings file
            var settingsFilePath = AssetDatabase.GUIDToAssetPath(guids[0]);
            Settings = AssetDatabase.LoadAssetAtPath<ToolboxEditorSettings>(settingsFilePath);

            if (Settings == null)
            {
                Debug.LogWarning("ToolboxEditorSettings not found. Cannot initialize Toolbox core functionalities. " +
                                 "You can create new settings file using CreateAsset menu -> Create -> Toolbox Editor -> Settings.");
                return;
            }

            //try to get version file path, it should be in same location as settings file
            var versionFilePath = settingsFilePath.Replace(Settings.name + settingsFileExt, versionFileName);
            Version = AssetDatabase.LoadAssetAtPath<TextAsset>(versionFilePath)?.text;

            //initialize core functionalities
            ToolboxDrawerUtility.InitializeDrawers(Settings);
            ToolboxFolderUtility.InitializeProject(Settings);
        }

        internal static void ReimportSettings()
        {
            //find and re-import this script file
            var guids = AssetDatabase.FindAssets(nameof(ToolboxSettingsUtility));
            if (guids == null || guids.Length == 0) return;
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);

            AssetDatabase.ImportAsset(path);
        }


        internal static ToolboxEditorSettings Settings { get; private set; }

        internal static string Version { get; private set; }
    }
}