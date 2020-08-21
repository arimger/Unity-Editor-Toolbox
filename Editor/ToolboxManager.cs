using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;

    [InitializeOnLoad]
    internal static class ToolboxManager
    {
        private const string settingsType = nameof(ToolboxEditorSettings);

        private static Editor globalSettingsEditor;


        [InitializeOnLoadMethod]
        internal static bool InitializeSettings()
        {
            var guids = AssetDatabase.FindAssets("t:" + settingsType);

            if (InitializeSettings(guids.Length > 0 ? guids[0] : null))
            {
                return true;
            }
            else
            {
                ToolboxEditorLog.KitInitializationWarning(settingsType);
                return false;
            }
        }

        internal static bool InitializeSettings(ToolboxEditorSettings settings)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(settings, out string guid, out long id);

            return InitializeSettings(guid);
        }

        internal static bool InitializeSettings(string assetGuid)
        {
            SettingsGuid = assetGuid;
            SettingsPath = AssetDatabase.GUIDToAssetPath(assetGuid);

            //try to get proper settings asset from the provided guid
            if (Settings = AssetDatabase.LoadAssetAtPath<ToolboxEditorSettings>(SettingsPath))
            {
                Settings.AddOnSettingsUpdatedListener(() =>
                {
                    //update associated utitilies after validation 
                    ForceSettingsUpdate();
                });
                //initialize core functionalities
                Settings.OnValidate();
                return true;
            }
            else
            {
                ForceSettingsUpdate();
                return false;
            }
        }

        internal static void ReimportSettings()
        {
            //find and re-import this script file
            var guids = AssetDatabase.FindAssets(nameof(ToolboxManager));
            if (guids == null || guids.Length == 0) return;
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);

            AssetDatabase.ImportAsset(path);
        }

        private static void ForceSettingsUpdate()
        {
            //perform separated data models update
            ToolboxDrawerUtility.PerformData(Settings);
            ToolboxProjectUtility.PerformData(Settings);
            ToolboxHierarchyUtility.PerformData(Settings);

            //perform additional repaint to update GUI
            ToolboxEditorProject.RepaintProjectOverlay();
            ToolboxEditorHierarchy.RepaintHierarchyOverlay();
        }


        [SettingsProvider]
        internal static SettingsProvider SettingsProvider()
        {
            var provider = new SettingsProvider("Project/Editor Toolbox", SettingsScope.Project);
            provider.guiHandler = (searchContext) =>
            {
                if (globalSettingsEditor == null || globalSettingsEditor.serializedObject.targetObject == null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Cannot find " + settingsType + " file located in this Project");
                    EditorGUILayout.Space();

                    void InitializeProvider()
                    {
                        provider.OnDeactivate();
                        provider.OnActivate("", null);
                    }

                    if (GUILayout.Button("Try to find the settings file"))
                    {
                        InitializeSettings();
                        InitializeProvider();
                    }

                    if (GUILayout.Button("Create the new settings file"))
                    {
                        var settingsInstance = ScriptableObject.CreateInstance(settingsType);

                        var directoryPath = EditorUtility.OpenFolderPanel("New Settings file location", "Assets", "");                      
                        var settingsPath = directoryPath.Substring(directoryPath.IndexOf("Assets/")) + "/" + settingsType + ".asset";

                        AssetDatabase.CreateAsset(settingsInstance, settingsPath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        ToolboxEditorLog.LogMessage("Created the settings file at - " + settingsPath);

                        InitializeSettings();
                        InitializeProvider();
                    }

                    return;
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Settings file location - " + SettingsPath);
                EditorGUILayout.Space();

                globalSettingsEditor.serializedObject.Update();
                globalSettingsEditor.OnInspectorGUI();
                globalSettingsEditor.serializedObject.ApplyModifiedProperties();
            };
            provider.activateHandler = (searchContext, elements) =>
            {
                globalSettingsEditor = Editor.CreateEditor(Settings);
            };
            provider.deactivateHandler = () =>
            {
                Object.DestroyImmediate(globalSettingsEditor);
            };

            return provider;
        }


        internal static ToolboxEditorSettings Settings { get; private set; }

        internal static string SettingsPath { get; private set; }
        internal static string SettingsGuid { get; private set; }
    }
}