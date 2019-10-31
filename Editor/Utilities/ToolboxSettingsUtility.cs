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
        private static ToolboxEditorSettings settings;


        [InitializeOnLoadMethod]
        internal static void InitializeSettings()
        {
            var guids = AssetDatabase.FindAssets("t:ToolboxEditorSettings");
            if (guids == null || guids.Length == 0) return;
            var path = AssetDatabase.GUIDToAssetPath(guids.First());
            settings = AssetDatabase.LoadAssetAtPath(path, typeof(ToolboxEditorSettings)) as ToolboxEditorSettings;

            if (!settings)
            {
                Debug.LogWarning("ToolboxEditorSettings not found. Cannot initialize Toolbox core functionalities. " +
                                 "You can create new settings file using CreateAsset menu -> Create -> Toolbox Editor -> Settings.");
                return;
            }

            ToolboxDrawerUtility.InitializeDrawers(settings);
            ToolboxFolderUtility.InitializeProject(settings);
        }


        internal static void ReimportEditor()
        {
            var guids = AssetDatabase.FindAssets("ToolboxSettingsUtility");
            if (guids == null || guids.Length == 0) return;
            var path = AssetDatabase.GUIDToAssetPath(guids.First());
            AssetDatabase.ImportAsset(path);
        }


        internal static bool ToolboxDrawersAllowed => settings?.UseToolboxDrawers ?? false;

        internal static bool ToolboxFoldersAllowed => settings?.UseToolboxProject ?? false;

        internal static bool ToolboxHierarchyAllowed => settings?.UseToolboxHierarchy ?? false;
    }
}