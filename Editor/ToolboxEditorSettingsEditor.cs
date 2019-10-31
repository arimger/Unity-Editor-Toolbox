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
        private ReorderableList propertyDrawerHandlersList;
        private ReorderableList conditionDrawerHandlersList;
        private ReorderableList collectionDrawerHandlersList;

        private void OnEnable()
        {
            hierarchySettingsEnabled = EditorPrefs.GetBool("ToolboxEditorSettings.hierarchySettingsEnabled", false);
            projectSettingsEnabled = EditorPrefs.GetBool("ToolboxEditorSettings.projectSettingsEnabled", false);
            drawersSettingsEnabled = EditorPrefs.GetBool("ToolboxEditorSettings.drawersSettingsEnabled", false);

            useToolboxDrawersProperty = serializedObject.FindProperty("useToolboxDrawers");
            useToolboxFoldersProperty = serializedObject.FindProperty("useToolboxFolders");
            useToolboxHierarchyProperty = serializedObject.FindProperty("useToolboxHierarchy");

            customFoldersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("customFolders"));

            areaDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("areaDrawerHandlers"));
            propertyDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("propertyDrawerHandlers"));
            conditionDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("conditionDrawerHandlers"));
            collectionDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("collectionDrawerHandlers"));
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool("ToolboxEditorSettings.hierarchySettingsEnabled", hierarchySettingsEnabled);
            EditorPrefs.SetBool("ToolboxEditorSettings.projectSettingsEnabled", projectSettingsEnabled);
            EditorPrefs.SetBool("ToolboxEditorSettings.drawersSettingsEnabled", drawersSettingsEnabled);
        }


        public override void OnInspectorGUI()
        {
            const float lineTickiness = 1.0f;

            EditorGUILayout.HelpBox("To approve all changes press the \"Apply\" button below.", MessageType.Info);

            serializedObject.Update();

            if (hierarchySettingsEnabled = EditorGUILayout.Foldout(hierarchySettingsEnabled, Style.hierarchySettingsContent, true, Style.foldoutStyle))
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(useToolboxHierarchyProperty);
            }
            else
            {
                ToolboxEditorGui.DrawLayoutLine(lineTickiness);
            }

            if (projectSettingsEnabled = EditorGUILayout.Foldout(projectSettingsEnabled, Style.projectSettingsContent, true, Style.foldoutStyle))
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(useToolboxFoldersProperty);
                EditorGUI.BeginDisabledGroup(!useToolboxFoldersProperty.boolValue);
                customFoldersList.DoLayoutList();
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                ToolboxEditorGui.DrawLayoutLine(lineTickiness);
            }
        
            if (drawersSettingsEnabled = EditorGUILayout.Foldout(drawersSettingsEnabled, Style.drawersSettingsContent, true, Style.foldoutStyle))
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(useToolboxDrawersProperty);

                EditorGUI.BeginDisabledGroup(!useToolboxDrawersProperty.boolValue);
                areaDrawerHandlersList.DoLayoutList();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                propertyDrawerHandlersList.DoLayoutList();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                collectionDrawerHandlersList.DoLayoutList();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                conditionDrawerHandlersList.DoLayoutList();
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                ToolboxEditorGui.DrawLayoutLine(lineTickiness);
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button(Style.buttonContent, Style.buttonOptions))
            {
                ToolboxSettingsUtility.ReimportEditor();
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