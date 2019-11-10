using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    [CustomEditor(typeof(ToolboxEditorSettings), true, isFallback = false)]
    [CanEditMultipleObjects]
    public class ToolboxEditorSettingsEditor : ToolboxEditor
    {
        private bool drawersSettingsEnabled;
        private bool projectSettingsEnabled;
        private bool hierarchySettingsEnabled;

        private SerializedProperty useToolboxDrawersProperty;
        private SerializedProperty useToolboxFoldersProperty;
        private SerializedProperty useToolboxHierarchyProperty;

        private ReorderableList customFoldersList;

        private ReorderableList decoratorDrawerHandlersList;
        private ReorderableList propertyDrawerHandlersList;
        private ReorderableList collectionDrawerHandlersList;
        private ReorderableList conditionDrawerHandlersList;

        private ReorderableList targetTypeDrawerHandlersList;

        private ToolboxEditorSettings currentTarget;


        private void OnEnable()
        {
            drawersSettingsEnabled = EditorPrefs.GetBool("ToolboxEditorSettings.drawersSettingsEnabled", false);
            projectSettingsEnabled = EditorPrefs.GetBool("ToolboxEditorSettings.projectSettingsEnabled", false);
            hierarchySettingsEnabled = EditorPrefs.GetBool("ToolboxEditorSettings.hierarchySettingsEnabled", false);

            useToolboxDrawersProperty = serializedObject.FindProperty("useToolboxDrawers");
            useToolboxFoldersProperty = serializedObject.FindProperty("useToolboxFolders");
            useToolboxHierarchyProperty = serializedObject.FindProperty("useToolboxHierarchy");

            customFoldersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("customFolders"));
            customFoldersList.HasHeader = false;

            decoratorDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("decoratorDrawerHandlers"));
            decoratorDrawerHandlersList.HasHeader = false;

            propertyDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("propertyDrawerHandlers"));
            propertyDrawerHandlersList.HasHeader = false;

            conditionDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("conditionDrawerHandlers"));
            conditionDrawerHandlersList.HasHeader = false;
  
            collectionDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("collectionDrawerHandlers"));
            collectionDrawerHandlersList.HasHeader = false;

            targetTypeDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("targetTypeDrawerHandlers"));
            targetTypeDrawerHandlersList.HasHeader = false;

            currentTarget = target as ToolboxEditorSettings;
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool("ToolboxEditorSettings.drawersSettingsEnabled", drawersSettingsEnabled);
            EditorPrefs.SetBool("ToolboxEditorSettings.projectSettingsEnabled", projectSettingsEnabled);
            EditorPrefs.SetBool("ToolboxEditorSettings.hierarchySettingsEnabled", hierarchySettingsEnabled);
        }


        private bool DrawDrawerList(ReorderableList list, string titleLabel, string assignButtonLabel)
        {
            EditorGUILayout.BeginHorizontal();

            var expanded = DrawListFoldout(list, Style.drawerListFoldoutStyle, titleLabel);
            GUILayout.FlexibleSpace();
            var pressed = GUILayout.Button(assignButtonLabel, Style.smallButtonStyle);
 
            EditorGUILayout.EndHorizontal();

            if (expanded)
            {
                list.DoLayoutList();
            }

            return pressed;
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

                if (DrawDrawerList(decoratorDrawerHandlersList, "Decorator Drawers", "Assign all possible"))
                {
                    currentTarget.SetAllPossibleDecoratorDrawers();
                }

                if (DrawDrawerList(propertyDrawerHandlersList, "Property Drawers", "Assign all possible"))
                {
                    currentTarget.SetAllPossiblePropertyDrawers();
                }

                if (DrawDrawerList(collectionDrawerHandlersList, "Collection Drawers", "Assign all possible"))
                {
                    currentTarget.SetAllPossibleCollectionDrawers();
                }

                if (DrawDrawerList(conditionDrawerHandlersList, "Condition Drawers", "Assign all possible"))
                {
                    currentTarget.SetAllPossibleConditionDrawers();
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Type-based", Style.smallHeaderStyle);

                ToolboxEditorGui.DrawLayoutLine(lineTickiness);

                if (DrawDrawerList(targetTypeDrawerHandlersList, "Target Type Drawers", "Assign all possible"))
                {
                    currentTarget.SetAllPossibleTargetTypeDrawers();
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
            internal static GUIStyle smallButtonStyle;
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
                smallButtonStyle = new GUIStyle(EditorStyles.miniButton);              
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