using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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
            var guids = AssetDatabase.FindAssets("t:ToolboxEditorSettings");
            if (guids == null || guids.Length == 0) return;
            var path = AssetDatabase.GUIDToAssetPath(guids.First());
            Settings = AssetDatabase.LoadAssetAtPath(path, typeof(ToolboxEditorSettings)) as ToolboxEditorSettings;

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
            var path = AssetDatabase.GUIDToAssetPath(guids.First());
            AssetDatabase.ImportAsset(path);
        }


        internal static ToolboxEditorSettings Settings { get; private set; }
    }
}