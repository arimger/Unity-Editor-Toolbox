using System.Collections;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Management
{
    using Toolbox.Editor.Drawers;
    using Editor = UnityEditor.Editor;

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
            ManageSceneViewCore(Settings);
        }

        private static void ManageInspectorCore(IToolboxInspectorSettings settings)
        {
            //setup all available drawers using the internal module
            ToolboxDrawersManager.UpdateDrawers(settings);
        }

        private static void ManageProjectCore(IToolboxProjectSettings settings)
        {
            if (settings == null)
            {
                ToolboxEditorProject.IsOverlayAllowed = false;
                ToolboxEditorProject.RepaintProjectOverlay();
                return;
            }

            //enable/disable the core GUI function
            ToolboxEditorProject.IsOverlayAllowed = settings.UseToolboxProject;

            ToolboxEditorProject.LargeIconScale = settings.LargeIconScale;
            ToolboxEditorProject.SmallIconScale = settings.SmallIconScale;
            ToolboxEditorProject.LargeIconPaddingRatio = settings.LargeIconPadding;
            ToolboxEditorProject.SmallIconPaddingRatio = settings.SmallIconPadding;

            ToolboxEditorProject.ClearCustomFolders();

            //create custom folders using stored data
            for (var i = 0; i < settings.CustomFolders.Count; i++)
            {
                ToolboxEditorProject.CreateCustomFolder(settings.CustomFolders[i]);
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
            ToolboxEditorHierarchy.ShowSelectionsCount = settings.ShowSelectionsCount;
            ToolboxEditorHierarchy.DrawSeparationLines = true;

            ToolboxEditorHierarchy.RemovePropertyLabels();

            //create custom drawers using stored data
            for (var i = 0; i < settings.RowDataTypes.Count; i++)
            {
                ToolboxEditorHierarchy.CreatePropertyLabels(settings.RowDataTypes[i]);
            }

            ToolboxEditorHierarchy.RepaintHierarchyOverlay();
        }

        private static void ManageSceneViewCore(IToolboxSceneViewSettings settings)
        {
            ToolboxEditorSceneView.UpdateSettings(settings);
        }

        private static string GetSettingsFileGuid()
        {
            var guids = AssetDatabase.FindAssets($"t:{settingsType}");
            string targetGuid = null;
            //try to find a settings file in a non-package directory
            for (var i = guids.Length - 1; i >= 0; i--)
            {
                var guid = guids[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                targetGuid = guid;
                if (!IsDefaultSettingsPath(path))
                {
                    break;
                }
            }

            return targetGuid;
        }

        private static bool IsDefaultSettingsPath(string path)
        {
            return path.StartsWith("Packages");
        }

        [InitializeOnLoadMethod]
        internal static void InitializeToolbox()
        {
            IsInitializing = true;
            if (TryInitializeSettings())
            {
                IsInitializing = false;
                IsInitialized = true;
                return;
            }

            EditorCoroutineUtility.StartCoroutineOwnerless(InitializeSettingsAsync());
            IEnumerator InitializeSettingsAsync()
            {
                yield return null;
                yield return new WaitWhile(() => EditorApplication.isUpdating);
                if (TryInitializeSettings())
                {
                    IsInitialized = true;
                }
                else
                {
                    IsInitialized = false;
                    ToolboxEditorLog.KitInitializationMessage();
                }

                IsInitializing = false;
            }
        }

        internal static bool TryInitializeSettings()
        {
            var settingsGuid = GetSettingsFileGuid();
            return TryInitializeSettings(settingsGuid);
        }

        internal static bool TryInitializeSettings(string settingsGuid)
        {
            SettingsGuid = settingsGuid;
            SettingsPath = AssetDatabase.GUIDToAssetPath(settingsGuid);

            //try to get proper settings asset from the provided guid
            if (Settings = AssetDatabase.LoadAssetAtPath<ToolboxEditorSettings>(SettingsPath))
            {
                //TODO: instead of relying on validation events let's prepare appropriate initialize/deinitialize process for all sub-systems

                //subscribe to all related events
                Settings.OnHierarchySettingsChanged += ManageHierarchyCore;
                Settings.OnProjectSettingsChanged += ManageProjectCore;
                Settings.OnInspectorSettingsChanged += ManageInspectorCore;
                Settings.OnSceneViewSettingsChanged += ManageSceneViewCore;
                //initialize core functionalities
                Settings.Validate(true);
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
            if (guids == null || guids.Length == 0)
            {
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            AssetDatabase.ImportAsset(path);
        }

        [SettingsProvider]
        internal static SettingsProvider SettingsProvider()
        {
            var provider = new SettingsProvider("Project/Editor Toolbox", SettingsScope.Project);

            void ReintializeProvider()
            {
                InitializeToolbox();

                //rebuild the settings provider right after initialization
                provider.OnDeactivate();
                provider.OnActivate(string.Empty, null);
            }

            provider.guiHandler = (searchContext) =>
            {
                if (globalSettingsEditor == null || globalSettingsEditor.serializedObject.targetObject == null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(string.Format("Cannot find {0} file located in this Project", settingsType));
                    EditorGUILayout.Space();

                    if (GUILayout.Button("Create a new settings file"))
                    {
                        var settingsInstance = ScriptableObject.CreateInstance<ToolboxEditorSettings>();
                        if (settingsInstance == null)
                        {
                            ToolboxEditorLog.LogError($"Something went wrong. Cannot create a new Settings file.");
                            return;
                        }

                        var locationPath = EditorUtility.OpenFolderPanel("New Settings file location", "Assets", "");
                        //validate returned path and create relative one if possible
                        if (string.IsNullOrEmpty(locationPath))
                        {
                            return;
                        }

                        var assetName = string.Format("{0}.asset", settingsType);
                        var relativePath = Path.Combine(FileUtil.GetProjectRelativePath(locationPath), assetName);

                        AssetDatabase.CreateAsset(settingsInstance, relativePath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        ToolboxEditorLog.LogInfo($"Created a settings file at - {relativePath}");

                        ReintializeProvider();
                    }

                    return;
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Settings file location - {SettingsPath}");
                if (IsDefaultSettingsPath(SettingsPath))
                {
                    EditorGUILayout.HelpBox("The currently displayed settings file is the default one, located in the packages directory. " +
                        "This means all changes can be easily lost. Create a new file for better maintainability.", MessageType.Info);
                }

                EditorGUILayout.Space();

                globalSettingsEditor.OnInspectorGUI();
            };
            provider.activateHandler = (searchContext, elements) =>
            {
                globalSettingsEditor = Editor.CreateEditor(Settings);
            };
            provider.deactivateHandler = () =>
            {
                Object.DestroyImmediate(globalSettingsEditor);
            };
            provider.titleBarGuiHandler = () =>
            {
                if (GUILayout.Button(new GUIContent("Refresh", "Try to find a settings file in the main (Assets) directory")))
                {
                    ReintializeProvider();
                }
            };

            return provider;
        }

        internal static bool IsInitialized { get; private set; }
        internal static bool IsInitializing { get; private set; }
        internal static ToolboxEditorSettings Settings { get; private set; }
        internal static string SettingsPath { get; private set; }
        internal static string SettingsGuid { get; private set; }
    }
}