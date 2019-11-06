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

        private ReorderableList targetTypeDrawerHandlersList;


        private void OnEnable()
        {
            hierarchySettingsEnabled = EditorPrefs.GetBool("ToolboxEditorSettings.hierarchySettingsEnabled", false);
            projectSettingsEnabled = EditorPrefs.GetBool("ToolboxEditorSettings.projectSettingsEnabled", false);
            drawersSettingsEnabled = EditorPrefs.GetBool("ToolboxEditorSettings.drawersSettingsEnabled", false);

            useToolboxDrawersProperty = serializedObject.FindProperty("useToolboxDrawers");
            useToolboxFoldersProperty = serializedObject.FindProperty("useToolboxFolders");
            useToolboxHierarchyProperty = serializedObject.FindProperty("useToolboxHierarchy");

            customFoldersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("customFolders"));
            customFoldersList.HasHeader = false;

            areaDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("areaDrawerHandlers"));
            areaDrawerHandlersList.HasHeader = false;

            propertyDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("propertyDrawerHandlers"));
            propertyDrawerHandlersList.HasHeader = false;

            conditionDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("conditionDrawerHandlers"));
            conditionDrawerHandlersList.HasHeader = false;
  
            collectionDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("collectionDrawerHandlers"));
            collectionDrawerHandlersList.HasHeader = false;

            targetTypeDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("targetTypeDrawerHandlers"));
            targetTypeDrawerHandlersList.HasHeader = false;
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool("ToolboxEditorSettings.hierarchySettingsEnabled", hierarchySettingsEnabled);
            EditorPrefs.SetBool("ToolboxEditorSettings.projectSettingsEnabled", projectSettingsEnabled);
            EditorPrefs.SetBool("ToolboxEditorSettings.drawersSettingsEnabled", drawersSettingsEnabled);
        }


        private bool DrawListFoldout(ReorderableList list)
        {
            return DrawListFoldout(list, Style.drawerListFoldoutStyle);
        }

        private bool DrawListFoldout(ReorderableList list, GUIStyle style)
        {
            return DrawListFoldout(list, style, list.List.displayName);
        }

        private bool DrawListFoldout(ReorderableList list, GUIStyle style, string label)
        {
            return list.List.isExpanded = EditorGUILayout.Foldout(list.List.isExpanded, label, true, style);
        }


        public override void OnInspectorGUI()
        {
            const float lineTickiness = 1.0f;

            EditorGUILayout.HelpBox("To approve all changes press the \"Apply\" button.", MessageType.Info);

            serializedObject.Update();

            if (hierarchySettingsEnabled = ToolboxEditorGui.DrawLayoutHeaderFoldout(hierarchySettingsEnabled, Style.hierarchySettingsContent, true, Style.settingsFoldoutStyle))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(useToolboxHierarchyProperty);

                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
            else
            {
                GUILayout.Space(-lineTickiness);
            }

            if (projectSettingsEnabled = ToolboxEditorGui.DrawLayoutHeaderFoldout(projectSettingsEnabled, Style.projectSettingsContent, true, Style.settingsFoldoutStyle))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(useToolboxFoldersProperty);

                EditorGUILayout.Space();
                ToolboxEditorGui.DrawLayoutLine(lineTickiness);

                EditorGUI.BeginDisabledGroup(!useToolboxFoldersProperty.boolValue);
                if (DrawListFoldout(customFoldersList, Style.folderListFoldoutStyle))
                {
                    customFoldersList.DoLayoutList();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
            else
            {
                GUILayout.Space(-lineTickiness);
            }

            if (drawersSettingsEnabled = ToolboxEditorGui.DrawLayoutHeaderFoldout(drawersSettingsEnabled, Style.drawersSettingsContent, true, Style.settingsFoldoutStyle))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(useToolboxDrawersProperty);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Attribute-based", Style.smallHeaderStyle);

                ToolboxEditorGui.DrawLayoutLine(lineTickiness);

                EditorGUI.BeginDisabledGroup(!useToolboxDrawersProperty.boolValue);
                if (DrawListFoldout(areaDrawerHandlersList, Style.drawerListFoldoutStyle, "Decorator Drawers[Area]"))
                {
                    areaDrawerHandlersList.DoLayoutList();
                }

                if (DrawListFoldout(propertyDrawerHandlersList, Style.drawerListFoldoutStyle, "Property Drawers"))
                {
                    propertyDrawerHandlersList.DoLayoutList();
                }

                if (DrawListFoldout(collectionDrawerHandlersList, Style.drawerListFoldoutStyle, "Collection Drawers"))
                {
                    collectionDrawerHandlersList.DoLayoutList();
                }

                if (DrawListFoldout(conditionDrawerHandlersList, Style.drawerListFoldoutStyle, "Condition Drawers"))
                {
                    conditionDrawerHandlersList.DoLayoutList();
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Type-based", Style.smallHeaderStyle);

                ToolboxEditorGui.DrawLayoutLine(lineTickiness);

                if (DrawListFoldout(targetTypeDrawerHandlersList, Style.drawerListFoldoutStyle, "Target Type Drawers"))
                {
                    targetTypeDrawerHandlersList.DoLayoutList();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
            else
            {
                GUILayout.Space(-lineTickiness);
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button(Style.applyButtonContent, Style.buttonOptions))
            {
                ToolboxSettingsUtility.ReimportSettings();
            }
        }


        internal static class Style
        {
            internal static GUIStyle smallHeaderStyle;
            internal static GUIStyle settingsFoldoutStyle;
            internal static GUIStyle drawerListFoldoutStyle;
            internal static GUIStyle folderListFoldoutStyle;

            internal static GUIContent hierarchySettingsContent = new GUIContent("Hierarchy Settings");
            internal static GUIContent projectSettingsContent = new GUIContent("Project Settings");
            internal static GUIContent drawersSettingsContent = new GUIContent("Drawers Settings");
            internal static GUIContent applyButtonContent = new GUIContent("Apply", "Apply changes");

            internal static GUILayoutOption[] buttonOptions = new GUILayoutOption[]
            {
                GUILayout.Width(80)
            };

            static Style()
            {
                smallHeaderStyle = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 10
                };
                settingsFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 11
                };
                drawerListFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontSize = 10
                };
                folderListFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 10
                };
            }
        }
    }
}