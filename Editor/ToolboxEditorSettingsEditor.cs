using UnityEditor;
using UnityEngine;
using static Toolbox.Editor.ToolboxEditorGui;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    [CustomEditor(typeof(ToolboxEditorSettings), true, isFallback = false)]
    [CanEditMultipleObjects, InitializeOnLoad]
    public class ToolboxEditorSettingsEditor : ToolboxEditor
    {
        private ToolboxEditorSettings currentTarget;

        private bool inspectorSettingsEnabled;
        private bool projectSettingsEnabled;
        private bool hierarchySettingsEnabled;

        private int enabledToShowDrawerType;

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

        private ReorderableList[] drawerHandlersLists;

        private string[] drawerHandlersLongNames;
        private string[] drawerHandlersShortNames;
        private string[] drawerHandlersInfoLabels;


        private void OnEnable()
        {
            currentTarget = target as ToolboxEditorSettings;

            //internal properties cached by 'EditorPrefs'
            hierarchySettingsEnabled = EditorPrefs.GetBool(nameof(ToolboxEditorSettings) + ".HierarchyEnabled", false);
            projectSettingsEnabled = EditorPrefs.GetBool(nameof(ToolboxEditorSettings) + ".ProjectEnabled", false);
            inspectorSettingsEnabled = EditorPrefs.GetBool(nameof(ToolboxEditorSettings) + ".InspectorEnabled", false);

            enabledToShowDrawerType = EditorPrefs.GetInt(nameof(ToolboxEditorSettings) + ".PickedDrawerType", 0);

            //hierarchy-related properties
            useToolboxHierarchyProperty = serializedObject.FindProperty("useToolboxHierarchy");
            drawHorizontalLinesProperty = serializedObject.FindProperty("drawHorizontalLines");
#if UNITY_2019_3_OR_NEWER
			rowDataItemsList = CreateClearList(serializedObject.FindProperty("rowDataItems"), hasHeader: false, elementLabel: "Position");
#else
            rowDataItemsList = CreateLinedList(serializedObject.FindProperty("rowDataItems"), hasHeader: false, elementLabel: "Position");
#endif
            //project-related properties
            useToolboxFoldersProperty = serializedObject.FindProperty("useToolboxFolders");

            largeIconScaleProperty = serializedObject.FindProperty("largeIconScale");
            smallIconScaleProperty = serializedObject.FindProperty("smallIconScale");
            largeIconPaddingProperty = serializedObject.FindProperty("largeIconPadding");
            smallIconPaddingProperty = serializedObject.FindProperty("smallIconPadding");
#if UNITY_2019_3_OR_NEWER
			customFoldersList = CreateClearList(serializedObject.FindProperty("customFolders"), hasHeader: false);
#else
            customFoldersList = CreateLinedList(serializedObject.FindProperty("customFolders"), hasHeader: false);
#endif
            //inspector-related properties
            useToolboxDrawersProperty = serializedObject.FindProperty("useToolboxDrawers");

            drawerHandlersLists = new ReorderableList[5];
#if UNITY_2019_3_OR_NEWER
            drawerHandlersLists[0] = CreateClearList(serializedObject.FindProperty("decoratorDrawerHandlers"), hasHeader: false);
            drawerHandlersLists[1] = CreateClearList(serializedObject.FindProperty("conditionDrawerHandlers"), hasHeader: false);
            drawerHandlersLists[2] = CreateClearList(serializedObject.FindProperty("selfPropertyDrawerHandlers"), hasHeader: false);
            drawerHandlersLists[3] = CreateClearList(serializedObject.FindProperty("listPropertyDrawerHandlers"), hasHeader: false);
            drawerHandlersLists[4] = CreateClearList(serializedObject.FindProperty("targetTypeDrawerHandlers"), hasHeader: false);
#else
            drawerHandlersLists[0] = CreateLinedList(serializedObject.FindProperty("decoratorDrawerHandlers"), hasHeader: false);
            drawerHandlersLists[1] = CreateLinedList(serializedObject.FindProperty("conditionDrawerHandlers"), hasHeader: false);
            drawerHandlersLists[2] = CreateLinedList(serializedObject.FindProperty("selfPropertyDrawerHandlers"), hasHeader: false);
            drawerHandlersLists[3] = CreateLinedList(serializedObject.FindProperty("listPropertyDrawerHandlers"), hasHeader: false);
            drawerHandlersLists[4] = CreateLinedList(serializedObject.FindProperty("targetTypeDrawerHandlers"), hasHeader: false);
#endif
            drawerHandlersLongNames = new[]
            {
                "Decorator",
                "Condition",
                "Property (Self)",
                "Property (List)",
                "Target Type"
            };
            drawerHandlersShortNames = new[]
            {
                "Decorator",
                "Condition",
                "Prop. Self",
                "Prop. List",
                "Type"
            };
            drawerHandlersInfoLabels = new[]
            {
                "",
                "",
                "",
                "",
                ""
            };
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool(nameof(ToolboxEditorSettings) + ".HierarchyEnabled", hierarchySettingsEnabled);
            EditorPrefs.SetBool(nameof(ToolboxEditorSettings) + ".ProjectEnabled", projectSettingsEnabled);
            EditorPrefs.SetBool(nameof(ToolboxEditorSettings) + ".InspectorEnabled", inspectorSettingsEnabled);

            EditorPrefs.SetInt(nameof(ToolboxEditorSettings) + ".PickedDrawerType", enabledToShowDrawerType);
        }

        private void DrawHierarchySettings()
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(useToolboxHierarchyProperty);
            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!useToolboxHierarchyProperty.boolValue);
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.BeginVertical(Style.groupStyle);
#endif
            EditorGUILayout.LabelField("Row Data Items");
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.EndVertical();
#endif
            {
#if UNITY_2019_3_OR_NEWER
                EditorGUILayout.BeginVertical(Style.groupStyle);
#endif
                rowDataItemsList.DoLayoutList();
#if UNITY_2019_3_OR_NEWER
                EditorGUILayout.EndVertical();
#endif
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

        private void DrawProjectSettings()
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
            if (GUILayout.Button("Reset", EditorStyles.miniButton, Style.resetButtonOptions))
            {
                Undo.RecordObject(target, "Resetted icon properties");
                currentTarget.ResetIconRectProperties();
                currentTarget.ValidateProjectSettings();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.BeginVertical(Style.groupStyle);
#endif
            EditorGUILayout.LabelField("Custom Folders");
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.EndVertical();
#endif
            {
#if UNITY_2019_3_OR_NEWER
                EditorGUILayout.BeginVertical(Style.groupStyle);
#endif
                customFoldersList.DoLayoutList();
#if UNITY_2019_3_OR_NEWER
                EditorGUILayout.EndVertical();
#endif
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            if (EditorGUI.EndChangeCheck())
            {
                currentTarget.SetProjectSettingsDirty();
            }
            EditorGUI.indentLevel--;
        }

        private void DrawInspectorSettings()
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(useToolboxDrawersProperty);
            EditorGUILayout.Space();

            var validateInspector = false;

#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.BeginVertical(Style.groupStyle);
#endif
#if UNITY_2019_3_OR_NEWER
            enabledToShowDrawerType = GUILayout.Toolbar(enabledToShowDrawerType, drawerHandlersShortNames, EditorStyles.toolbarButton);
#else
            enabledToShowDrawerType = GUILayout.Toolbar(enabledToShowDrawerType, drawerHandlersShortNames);
#endif
            EditorGUI.BeginDisabledGroup(!useToolboxDrawersProperty.boolValue);

            var longName = drawerHandlersLongNames[enabledToShowDrawerType];

            EditorGUILayout.LabelField(longName + " Drawers", Style.drawersHeaderStyle);
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.EndVertical();
#endif
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.BeginVertical(Style.groupStyle);
#endif
            drawerHandlersLists[enabledToShowDrawerType].DoLayoutList();
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.EndVertical();
#endif
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.BeginVertical(Style.groupStyle);
#endif
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Style.clearListContent, EditorStyles.miniButtonLeft))
            {
                Undo.RecordObject(target, "Clear Drawers");
                switch (enabledToShowDrawerType)
                {
                    case 0:
                        currentTarget.ClearDecoratorDrawerHandlers();
                        break;
                    case 1:
                        currentTarget.ClearConditionDrawerHandlers();
                        break;
                    case 2:
                        currentTarget.ClearSelfPropertyDrawerHandlers();
                        break;
                    case 3:
                        currentTarget.ClearListPropertyDrawerHandlers();
                        break;
                    case 4:
                        currentTarget.ClearTargetTypeDrawerHandlers();
                        break;
                }
                validateInspector = true;
            }

            if (GUILayout.Button(Style.assignAllContent, EditorStyles.miniButtonMid))
            {
                Undo.RecordObject(target, "Assign Drawers");
                switch (enabledToShowDrawerType)
                {
                    case 0:
                        currentTarget.SetAllPossibleDecoratorDrawers();
                        break;
                    case 1:
                        currentTarget.SetAllPossibleConditionDrawers();
                        break;
                    case 2:
                        currentTarget.SetAllPossibleSelfPropertyDrawers();
                        break;
                    case 3:
                        currentTarget.SetAllPossibleListPropertyDrawers();
                        break;
                    case 4:
                        currentTarget.SetAllPossibleTargetTypeDrawers();
                        break;
                }
                validateInspector = true;
            }

            if (GUILayout.Button(Style.validListContent, EditorStyles.miniButtonRight))
            {
                ToolboxEditorLog.LogMessage("Function not implemented, force recompilation to validate drawers assignment.");
            }
            EditorGUILayout.EndHorizontal();
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.EndVertical();
#endif
            EditorGUILayout.Space();

            if (validateInspector)
            {
                currentTarget.ValidateInspectorSettings();
            }

            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
            {
                currentTarget.SetInspectorSettingsDirty();
            }
            EditorGUI.indentLevel--;
        }


        public override void DrawCustomInspector()
        {
            serializedObject.Update();

            //handle hierarchy settings section
            if (hierarchySettingsEnabled = DrawLayoutHeaderFoldout(hierarchySettingsEnabled, Style.hierarchySettingsContent, true, Style.sectionHeaderStyle))
            {
                DrawHierarchySettings();
            }

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            //handle project settings section
            if (projectSettingsEnabled = DrawLayoutHeaderFoldout(projectSettingsEnabled, Style.projectSettingsContent, true, Style.sectionHeaderStyle))
            {
                DrawProjectSettings();
            }

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            //handle inspector settings section
            if (inspectorSettingsEnabled = DrawLayoutHeaderFoldout(inspectorSettingsEnabled, Style.inspectorSettingsContent, true, Style.sectionHeaderStyle))
            {
                DrawInspectorSettings();
            }

            serializedObject.ApplyModifiedProperties();
        }


        internal static class Style
        {
            internal static readonly GUIStyle groupStyle;
            internal static readonly GUIStyle drawersHeaderStyle;
            internal static readonly GUIStyle sectionHeaderStyle;

            internal static readonly GUIContent clearListContent = new GUIContent("Clear");
            internal static readonly GUIContent assignAllContent = new GUIContent("Assign all possible");
            internal static readonly GUIContent validListContent = new GUIContent("Validate");

#if UNITY_2019_3_OR_NEWER
            internal static readonly GUIContent hierarchySettingsContent = new GUIContent("Hierarchy",
                EditorGUIUtility.IconContent("UnityEditor.SceneHierarchyWindow").image);
#else
            internal static readonly GUIContent hierarchySettingsContent = new GUIContent("Hierarchy",
                EditorGUIUtility.IconContent("UnityEditor.HierarchyWindow").image);
#endif
            internal static readonly GUIContent projectSettingsContent = new GUIContent("Project",
                EditorGUIUtility.IconContent("Project").image);
            internal static readonly GUIContent inspectorSettingsContent = new GUIContent("Inspector",
                EditorGUIUtility.IconContent("UnityEditor.InspectorWindow").image);

            internal static readonly GUILayoutOption[] resetButtonOptions = new GUILayoutOption[]
            {
                GUILayout.Width(50)
            };

            static Style()
            {
#if UNITY_2019_3_OR_NEWER
                groupStyle = new GUIStyle(EditorStyles.helpBox);
#else
                groupStyle = new GUIStyle();
#endif
                drawersHeaderStyle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleCenter
                };
                sectionHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleLeft
                };
            }
        }
    }
}