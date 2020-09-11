using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    [CustomEditor(typeof(ToolboxEditorSettings), true, isFallback = false)]
    [CanEditMultipleObjects, InitializeOnLoad]
    public class ToolboxEditorSettingsEditor : ToolboxEditor
    {
        private bool inspectorSettingsEnabled;
        private bool projectSettingsEnabled;
        private bool hierarchySettingsEnabled;

        private SerializedProperty useToolboxHierarchyProperty;
        private SerializedProperty drawHorizontalLinesProperty;
        private SerializedProperty useToolboxFoldersProperty;
        private SerializedProperty useToolboxDrawersProperty;
        private SerializedProperty largeIconScaleProperty;
        private SerializedProperty smallIconScaleProperty;
        private SerializedProperty largeIconPaddingProperty;
        private SerializedProperty smallIconPaddingProperty;

        private ReorderableList rowDataItemsList;
        private ReorderableList customFoldersList;

        private ReorderableList decoratorDrawerHandlersList;
        private ReorderableList conditionDrawerHandlersList;
        private ReorderableList selfPropertyDrawerHandlersList;
        private ReorderableList listPropertyDrawerHandlersList;
        private ReorderableList targetTypeDrawerHandlersList;

        private ToolboxEditorSettings currentTarget;


        private void OnEnable()
        {
            hierarchySettingsEnabled = EditorPrefs.GetBool(nameof(ToolboxEditorSettings) + ".HierarchyEnabled", false);
            projectSettingsEnabled = EditorPrefs.GetBool(nameof(ToolboxEditorSettings) + ".ProjectEnabled", false);
            inspectorSettingsEnabled = EditorPrefs.GetBool(nameof(ToolboxEditorSettings) + ".InspectorEnabled", false);

            useToolboxHierarchyProperty = serializedObject.FindProperty("useToolboxHierarchy");
            drawHorizontalLinesProperty = serializedObject.FindProperty("drawHorizontalLines");
            useToolboxDrawersProperty = serializedObject.FindProperty("useToolboxDrawers");
            useToolboxFoldersProperty = serializedObject.FindProperty("useToolboxFolders");
            largeIconScaleProperty = serializedObject.FindProperty("largeIconScale");
            smallIconScaleProperty = serializedObject.FindProperty("smallIconScale");
            largeIconPaddingProperty = serializedObject.FindProperty("largeIconPadding");
            smallIconPaddingProperty = serializedObject.FindProperty("smallIconPadding");

            rowDataItemsList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("rowDataItems"));
            customFoldersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("customFolders"));

            decoratorDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("decoratorDrawerHandlers"));
            conditionDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("conditionDrawerHandlers"));
            selfPropertyDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("selfPropertyDrawerHandlers"));
            listPropertyDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("listPropertyDrawerHandlers"));
            targetTypeDrawerHandlersList = ToolboxEditorGui.CreateLinedList(serializedObject.FindProperty("targetTypeDrawerHandlers"));

            currentTarget = target as ToolboxEditorSettings;
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool(nameof(ToolboxEditorSettings) + ".HierarchyEnabled", hierarchySettingsEnabled);
            EditorPrefs.SetBool(nameof(ToolboxEditorSettings) + ".ProjectEnabled", projectSettingsEnabled);
            EditorPrefs.SetBool(nameof(ToolboxEditorSettings) + ".InspectorEnabled", inspectorSettingsEnabled);
        }


        /// <summary>
        /// Draws all needed inspector controls.
        /// </summary>
        public override void DrawCustomInspector()
        {
            EditorGUIUtility.labelWidth = 0.0f;
            EditorGUIUtility.fieldWidth = 0.0f;

            serializedObject.Update();

            //handle hierarchy settings section
            if (hierarchySettingsEnabled = ToolboxEditorGui.DrawLayoutHeaderFoldout(hierarchySettingsEnabled, Style.hierarchySettingsContent, true, Style.bigSectionFoldoutStyle))
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(useToolboxHierarchyProperty);
                EditorGUILayout.Space();

                EditorGUI.BeginDisabledGroup(!useToolboxHierarchyProperty.boolValue);
                if (ToolboxEditorGui.DoListFoldout(rowDataItemsList, Style.normalListFoldoutStyle))
                {
                    rowDataItemsList.ElementLabel = "Position";
                    rowDataItemsList.DoLayoutList();
                }
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(drawHorizontalLinesProperty);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
                if (EditorGUI.EndChangeCheck())
                {
                    currentTarget.SetHierarchySettingsDirty();
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                GUILayout.Space(-Style.spacing / 2);
            }

            //handle project settings section (focused on customized folder icons)
            if (projectSettingsEnabled = ToolboxEditorGui.DrawLayoutHeaderFoldout(projectSettingsEnabled, Style.projectSettingsContent, true, Style.bigSectionFoldoutStyle))
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(useToolboxFoldersProperty);
                EditorGUILayout.Space();

                EditorGUI.BeginDisabledGroup(!useToolboxFoldersProperty.boolValue);

                EditorGUILayout.LabelField("Large Icon Properties");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(largeIconScaleProperty, new GUIContent("Scale"), false);

                var x = 0.0f;
                var y = 0.0f;

                const float minPadding = -1.5f;
                const float maxPadding = +1.5f;

                EditorGUI.BeginChangeCheck();
                x = EditorGUILayout.Slider(new GUIContent("X"), largeIconPaddingProperty.vector2Value.x, minPadding, maxPadding);
                y = EditorGUILayout.Slider(new GUIContent("Y"), largeIconPaddingProperty.vector2Value.y, minPadding, maxPadding);
                if (EditorGUI.EndChangeCheck())
                {
                    largeIconPaddingProperty.vector2Value = new Vector2(x, y);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Small Icon Properties");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(smallIconScaleProperty, new GUIContent("Scale"), false);
                EditorGUI.BeginChangeCheck();
                x = EditorGUILayout.Slider(new GUIContent("X"), smallIconPaddingProperty.vector2Value.x, minPadding, maxPadding);
                y = EditorGUILayout.Slider(new GUIContent("Y"), smallIconPaddingProperty.vector2Value.y, minPadding, maxPadding);
                if (EditorGUI.EndChangeCheck())
                {
                    smallIconPaddingProperty.vector2Value = new Vector2(x, y);
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Reset", Style.smallButtonStyle, Style.resetButtonOptions))
                {
                    Undo.RecordObject(target, "Resetted icon properties");
                    currentTarget.ResetIconsRectProperties();
                    currentTarget.ValidateProjectSettings();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                //draw custom icons list
                if (ToolboxEditorGui.DoListFoldout(customFoldersList, Style.normalListFoldoutStyle))
                {
                    customFoldersList.DoLayoutList();
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
                if (EditorGUI.EndChangeCheck())
                {
                    currentTarget.SetProjectSettingsDirty();
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                GUILayout.Space(-Style.spacing / 2);
            }

            //handle drawers settings section
            if (inspectorSettingsEnabled = ToolboxEditorGui.DrawLayoutHeaderFoldout(inspectorSettingsEnabled, Style.inspectorSettingsContent, true, Style.bigSectionFoldoutStyle))
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(useToolboxDrawersProperty);
                EditorGUILayout.Space();

                EditorGUI.BeginDisabledGroup(!useToolboxDrawersProperty.boolValue);

                EditorGUILayout.LabelField("Attribute-based", Style.smallHeaderStyle);

                const string assignButtonLabel = "Assign all possible";

                var validateInspector = false;
                if (ToolboxEditorGui.DoDrawerList(decoratorDrawerHandlersList, "Decorator Drawers", assignButtonLabel, Style.drawerListFoldoutStyle))
                {
                    Undo.RecordObject(target, "Assignment of multiple decorator drawers");
                    currentTarget.SetAllPossibleDecoratorDrawers();
                    validateInspector = true;
                }

                if (ToolboxEditorGui.DoDrawerList(conditionDrawerHandlersList, "Condition Drawers", assignButtonLabel, Style.drawerListFoldoutStyle))
                {
                    Undo.RecordObject(target, "Assignment of multiple condition drawers");
                    currentTarget.SetAllPossibleConditionDrawers();
                    validateInspector = true;
                }

                if (ToolboxEditorGui.DoDrawerList(selfPropertyDrawerHandlersList, "Property Drawers (Self)", assignButtonLabel, Style.drawerListFoldoutStyle))
                {
                    Undo.RecordObject(target, "Assignment of multiple self property drawers");
                    currentTarget.SetAllPossibleSelfPropertyDrawers();
                    validateInspector = true;
                }

                if (ToolboxEditorGui.DoDrawerList(listPropertyDrawerHandlersList, "Property Drawers (List)", assignButtonLabel, Style.drawerListFoldoutStyle))
                {
                    Undo.RecordObject(target, "Assignment of multiple list property drawers");
                    currentTarget.SetAllPossibleListPropertyDrawers();
                    validateInspector = true;
                }

                if (validateInspector)
                {
                    currentTarget.ValidateInspectorSettings();
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Type-based", Style.smallHeaderStyle);

                if (ToolboxEditorGui.DoDrawerList(targetTypeDrawerHandlersList, "Target Type Drawers", assignButtonLabel, Style.drawerListFoldoutStyle))
                {
                    currentTarget.SetAllPossibleTargetTypeDrawers();
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
                if (EditorGUI.EndChangeCheck())
                {
                    currentTarget.SetInspectorSettingsDirty();
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                GUILayout.Space(-Style.spacing / 2);
            }

            serializedObject.ApplyModifiedProperties();
        }


        internal static class Style
        {
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;

            internal static readonly GUIStyle smallButtonStyle;
            internal static readonly GUIStyle smallHeaderStyle;
            internal static readonly GUIStyle usualHeaderStyle;
            internal static readonly GUIStyle bigSectionFoldoutStyle;
            internal static readonly GUIStyle drawerListFoldoutStyle;
            internal static readonly GUIStyle normalListFoldoutStyle;

#if UNITY_2019_3_OR_NEWER
            internal static readonly GUIContent hierarchySettingsContent = new GUIContent("Hierarchy Settings",
                EditorGUIUtility.IconContent("UnityEditor.SceneHierarchyWindow").image);
#else
            internal static readonly GUIContent hierarchySettingsContent = new GUIContent("Hierarchy Settings",
                EditorGUIUtility.IconContent("UnityEditor.HierarchyWindow").image);
#endif
            internal static readonly GUIContent projectSettingsContent = new GUIContent("Project Settings",
                EditorGUIUtility.IconContent("Project").image);
            internal static readonly GUIContent inspectorSettingsContent = new GUIContent("Inspector Settings",
                EditorGUIUtility.IconContent("UnityEditor.InspectorWindow").image);

            internal static readonly GUILayoutOption[] resetButtonOptions = new GUILayoutOption[]
            {
                GUILayout.Width(50)
            };

            static Style()
            {
                smallButtonStyle = new GUIStyle(EditorStyles.miniButton);
                smallHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 10
                };
                usualHeaderStyle = new GUIStyle(EditorStyles.boldLabel);

                bigSectionFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
#if UNITY_2019_3_OR_NEWER
                    contentOffset = new Vector2(0, 0),
#else
                    contentOffset = new Vector2(0, -spacing),
#endif
                    fontSize = 11
                };
                drawerListFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontSize = 10
                };
                normalListFoldoutStyle = new GUIStyle(EditorStyles.foldout);
            }
        }
    }
}