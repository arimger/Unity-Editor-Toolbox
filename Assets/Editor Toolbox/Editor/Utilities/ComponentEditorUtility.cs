using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

//TODO: handling preset drawers;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    internal static class ComponentEditorUtility
    {
        internal static readonly ComponentEditorSettings settings;

        static ComponentEditorUtility()
        {
            var guids = AssetDatabase.FindAssets("t:ComponentEditorSettings");
            if (guids == null || guids[0] == null) return;
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            settings = AssetDatabase.LoadAssetAtPath(path, typeof(ComponentEditorSettings)) as ComponentEditorSettings;
        }

        internal static void ReimportEditor()
        {
            var guids = AssetDatabase.FindAssets("ComponentEditor");
            if (guids == null || guids[0] == null) return;
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            AssetDatabase.ImportAsset(path);
        }
    }
}