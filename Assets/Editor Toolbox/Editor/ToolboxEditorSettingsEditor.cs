using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    [CustomEditor(typeof(ToolboxEditorSettings), true, isFallback = false)]
    [CanEditMultipleObjects]
    public class ToolboxEditorSettingsEditor : ToolboxEditor
    {
        private bool hierarchySettingsEnabled;
        private bool projectSettingsEnabled;
        private bool drawersSettingsEnabled;

        private SerializedProperty useToolboxDrawersProperty;
        private SerializedProperty useToolboxFoldersProperty;
        private SerializedProperty useToolboxHierarchyProperty;

        private ReorderableList customFoldersList;

        private ReorderableList areaDrawerHandlersList;
        private ReorderableList groupDrawerHandlersList;
        private ReorderableList propertyDrawerHandlersList;
        private ReorderableList conditionDrawerHandlersList;


        protected override void OnEnable()
        {
            hierarchySettingsEnabled = EditorPrefs.GetBool("ToolboxEditorSettings.hierarchySettingsEnabled", false);
            projectSettingsEnabled = EditorPrefs.GetBool("ToolboxEditorSettings.projectSettingsEnabled", false);
            drawersSettingsEnabled = EditorPrefs.GetBool("ToolboxEditorSettings.drawersSettingsEnabled", false);

            useToolboxDrawersProperty = serializedObject.FindProperty("useToolboxDrawers");
            useToolboxFoldersProperty = serializedObject.FindProperty("useToolboxFolders");
            useToolboxHierarchyProperty = serializedObject.FindProperty("useToolboxHierarchy");

            customFoldersList = ToolboxEditorGui.CreateBoxedList(serializedObject.FindProperty("customFolders"));

            areaDrawerHandlersList = ToolboxEditorGui.CreateBoxedList(serializedObject.FindProperty("areaDrawerHandlers"));
            groupDrawerHandlersList = ToolboxEditorGui.CreateBoxedList(serializedObject.FindProperty("groupDrawerHandlers"));
            propertyDrawerHandlersList = ToolboxEditorGui.CreateBoxedList(serializedObject.FindProperty("propertyDrawerHandlers"));
            conditionDrawerHandlersList = ToolboxEditorGui.CreateBoxedList(serializedObject.FindProperty("conditionDrawerHandlers"));
        }

        protected override void OnDisable()
        {
            EditorPrefs.SetBool("ToolboxEditorSettings.hierarchySettingsEnabled", hierarchySettingsEnabled);
            EditorPrefs.SetBool("ToolboxEditorSettings.projectSettingsEnabled", projectSettingsEnabled);
            EditorPrefs.SetBool("ToolboxEditorSettings.drawersSettingsEnabled", drawersSettingsEnabled);
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (hierarchySettingsEnabled = EditorGUILayout.Foldout(hierarchySettingsEnabled, Style.hierarchySettingsContent, true, Style.foldoutStyle))
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(useToolboxHierarchyProperty);
            }

            ToolboxEditorGui.DrawLayoutLine();

            if (projectSettingsEnabled = EditorGUILayout.Foldout(projectSettingsEnabled, Style.projectSettingsContent, true, Style.foldoutStyle))
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(useToolboxFoldersProperty);
                EditorGUI.BeginDisabledGroup(!useToolboxFoldersProperty.boolValue);
                customFoldersList.DoLayoutList();
                EditorGUI.EndDisabledGroup();
            }

            ToolboxEditorGui.DrawLayoutLine();

            if (drawersSettingsEnabled = EditorGUILayout.Foldout(drawersSettingsEnabled, Style.drawersSettingsContent, true, Style.foldoutStyle))
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(useToolboxDrawersProperty);

                EditorGUI.BeginDisabledGroup(!useToolboxDrawersProperty.boolValue);
                EditorGUILayout.HelpBox("Select all wanted drawers and press \"Apply\" button.", MessageType.Info);
                areaDrawerHandlersList.DoLayoutList();
                EditorGUILayout.Separator();
                EditorGUILayout.HelpBox("Deprecated.", MessageType.Warning);
                EditorGUI.BeginDisabledGroup(true);
                groupDrawerHandlersList.DoLayoutList();
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Separator();
                propertyDrawerHandlersList.DoLayoutList();
                EditorGUILayout.Separator();
                conditionDrawerHandlersList.DoLayoutList();
                EditorGUI.EndDisabledGroup();
            }

            ToolboxEditorGui.DrawLayoutLine();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button(Style.buttonContent, Style.buttonOptions))
            {
                ToolboxEditorUtility.ReimportEditor();
            }
        }


        internal static class Style
        {
            internal static GUIStyle foldoutStyle;

            internal static GUIContent hierarchySettingsContent = new GUIContent("Hierarchy Settings");
            internal static GUIContent projectSettingsContent = new GUIContent("Project Settings");
            internal static GUIContent drawersSettingsContent = new GUIContent("Drawers Settings");
            internal static GUIContent buttonContent = new GUIContent("Apply", "Apply changes");

            internal static GUILayoutOption[] buttonOptions = new GUILayoutOption[]
            {
                GUILayout.Width(80)
            };

            static Style()
            {
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 11
                };
            }
        }
    }
}