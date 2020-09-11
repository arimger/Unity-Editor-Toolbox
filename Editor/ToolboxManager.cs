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


        private static void ForceModulesUpdate()
        {
            //perform separated modules update
            ManageInspectorCore(Settings);
            ManageProjectCore(Settings);
            ManageHierarchyCore(Settings);
        }

        private static void ManageInspectorCore(IToolboxInspectorSettings settings)
        {
            //setup all available drawers using the internal module
            ToolboxDrawerModule.UpdateDrawers(settings);
        }

        private static void ManageProjectCore(IToolboxProjectSettings settings)
        {
            if (settings == null)
            {
                ToolboxEditorProject.IsOverlayAllowed = false;
                ToolboxEditorProject.RepaintProjectOverlay();
                return;
            }

            var validateData = !IsInitialized;

            //enable/disable the core GUI function
            ToolboxEditorProject.IsOverlayAllowed = settings.UseToolboxProject;

            ToolboxEditorProject.LargeIconScale = settings.LargeIconScale;
            ToolboxEditorProject.SmallIconScale = settings.SmallIconScale;
            ToolboxEditorProject.LargeIconPaddingRatio = settings.LargeIconPadding;
            ToolboxEditorProject.SmallIconPaddingRatio = settings.SmallIconPadding;

            ToolboxEditorProject.RemoveCustomFolders();

            //create custom folders using stored data
            for (var i = 0; i < settings.CustomFoldersCount; i++)
            {
                ToolboxEditorProject.CreateCustomFolder(settings.GetCustomFolderAt(i));
            }

            ToolboxEditorProject.RepaintProjectOverlay();
        }

        private static void ManageHierarchyCore(IToolboxHierarchySettings settings)
        {
            if (settings == null)
            {
                ToolboxEditorHierarchy.IsOverlayAllowed = false;
                ToolboxEditorHierarchy.RepaintHierarchyOverlay();
                return;
            }

            //enable/disable the core GUI function
            ToolboxEditorHierarchy.IsOverlayAllowed = settings.UseToolboxHierarchy;

            ToolboxEditorHierarchy.DrawHorizontalLines = settings.DrawHorizontalLines;
            ToolboxEditorHierarchy.DrawSeparationLines = true;

            ToolboxEditorHierarchy.RemoveAllowedHierarchyContentCallbacks();

            //create custom drawers using stored data
            for (var i = 0; i < settings.RowDataItemsCount; i++)
            {
                ToolboxEditorHierarchy.CreateAllowedHierarchyContentCallbacks(settings.GetRowDataItemAt(i));
            }

            ToolboxEditorHierarchy.RepaintHierarchyOverlay();
        }


        [InitializeOnLoadMethod]
        internal static bool InitializeSettings()
        {
            var guids = AssetDatabase.FindAssets("t:" + settingsType);

            if (InitializeSettings(guids.Length > 0 ? guids[0] : null))
            {
                IsInitialized = true;
                return true;
            }
            else
            {
                ToolboxEditorLog.KitInitializationMessage();
                return false;
            }
        }

        internal static bool InitializeSettings(string settingsGuid)
        {
            SettingsGuid = settingsGuid;
            SettingsPath = AssetDatabase.GUIDToAssetPath(settingsGuid);

            //try to get proper settings asset from the provided guid
            if (Settings = AssetDatabase.LoadAssetAtPath<ToolboxEditorSettings>(SettingsPath))
            {
                //subscribe to all related events
                Settings.OnHierarchySettingsChanged += () => ManageHierarchyCore(Settings);
                Settings.OnProjectSettingsChanged += () => ManageProjectCore(Settings);
                Settings.OnInspectorSettingsChanged += () => ManageInspectorCore(Settings);
                //initialize core functionalities
                Settings.Validate();
                return true;
            }
            else
            {
                ForceModulesUpdate();
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


        internal static bool IsInitialized { get; private set; }

        internal static ToolboxEditorSettings Settings { get; private set; }

        internal static string SettingsPath { get; private set; }
        internal static string SettingsGuid { get; private set; }
    }
}