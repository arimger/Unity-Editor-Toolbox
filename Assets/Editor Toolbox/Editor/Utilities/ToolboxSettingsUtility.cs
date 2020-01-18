using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;

    [InitializeOnLoad]
    internal static class ToolboxSettingsUtility
    {
        private const string settingsType = nameof(ToolboxEditorSettings);

        private static string settingsGuid;
        private static string settingsPath;

        private static Editor globalSettingsEditor;


        [InitializeOnLoadMethod]
        internal static void InitializeSettings()
        {
            var guids = AssetDatabase.FindAssets("t:" + settingsType);

            InitializeSettings(guids.Length > 0 ? guids[0] : null);
        }

        internal static void InitializeSettings(ToolboxEditorSettings settings)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(settings, out string guid, out long id);

            InitializeSettings(guid);
        }

        internal static void InitializeSettings(string assetGuid)
        {
            settingsGuid = assetGuid;
            settingsPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            //try to get proper settings asset from provided guid
            var settings = AssetDatabase.LoadAssetAtPath<ToolboxEditorSettings>(settingsPath);
            if (settings == null)
            {
                ToolboxEditorLog.KitInitializationWarning(settingsType);
                return;
            }

            Settings = settings;
            Settings.AddOnSettingsUpdatedListener(() =>
            {
                //perform separated data models update
                ToolboxDrawerUtility.PerformData();
                ToolboxProjectUtility.PerformData();
                ToolboxHierarchyUtility.PerformData();

                //perform additional repaint to update GUI
                ToolboxEditorProject.RepaintProjectOverlay();
                ToolboxEditorHierarchy.RepaintHierarchyOverlay();
            });

            //initialize core functionalities
            ToolboxDrawerUtility.PerformData(Settings);
            ToolboxProjectUtility.PerformData(Settings);
            ToolboxHierarchyUtility.PerformData(Settings);
        }

        internal static void ReimportSettings()
        {
            //find and re-import this script file
            var guids = AssetDatabase.FindAssets(nameof(ToolboxSettingsUtility));
            if (guids == null || guids.Length == 0) return;
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);

            AssetDatabase.ImportAsset(path);
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

                    if (GUILayout.Button("Try to find settings file"))
                    {
                        InitializeSettings();
                        InitializeProvider();
                    }

                    if (GUILayout.Button("Create settings new file"))
                    {
                        var settingsInstance = ScriptableObject.CreateInstance(settingsType);
                        var path = "Assets/" + settingsType + ".asset";

                        AssetDatabase.CreateAsset(settingsInstance, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        Debug.Log("Created settings file at - " + path);

                        InitializeSettings();
                        InitializeProvider();
                    }

                    return;
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Settings file location - " + settingsPath);
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
    }
}