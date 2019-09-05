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

        internal static readonly List<OrderedDrawerBase> propertyDrawers = new List<OrderedDrawerBase>();

        static ComponentEditorUtility()
        {
            var guids = AssetDatabase.FindAssets("t:ComponentEditorSettings");
            if (guids == null || guids[0] == null) return;
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            settings = AssetDatabase.LoadAssetAtPath(path, typeof(ComponentEditorSettings)) as ComponentEditorSettings;

            if (settings == null) return;
            //create all needed property drawer instances and store them in list
            for (var i = 0; i < settings.PropertyDrawersCount; i++)
            {
                var type = settings.GetPropertyDrawerTypeAt(i);
                if (type == null) continue;
                propertyDrawers.Add(Activator.CreateInstance(type) as OrderedDrawerBase);
            }
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