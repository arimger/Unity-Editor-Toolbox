using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

using static Toolbox.Editor.ToolboxEditorGui;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    [CustomEditor(typeof(ToolboxEditorSettings), true, isFallback = false)]
    [CanEditMultipleObjects]
    internal class ToolboxEditorSettingsEditor : ToolboxEditor
    {
        private ToolboxEditorSettings currentTarget;

        private AnimBool hierarchyAnimBool;
        private AnimBool projectAnimBool;
        private AnimBool inspectorAnimBool;

        private int enabledToShowDrawerType;

        private SerializedProperty useToolboxHierarchyProperty;
        private SerializedProperty drawHorizontalLinesProperty;
        private SerializedProperty showSelectionsCountProperty;
        private SerializedProperty useToolboxFoldersProperty;
        private SerializedProperty useToolboxDrawersProperty;
        private SerializedProperty largeIconScaleProperty;
        private SerializedProperty smallIconScaleProperty;
        private SerializedProperty largeIconPaddingProperty;
        private SerializedProperty smallIconPaddingProperty;

        private ReorderableListBase rowDataItemsList;
        private ReorderableListBase customFoldersList;

        private ReorderableListBase[] drawerHandlersLists;

        private string[] drawerHandlersLongNames;
        private string[] drawerHandlersShortNames;
        private string[] drawerHandlersInfoLabels;


        private void OnEnable()
        {
            currentTarget = target as ToolboxEditorSettings;

            //internal properties cached by 'EditorPrefs'
            hierarchyAnimBool = new AnimBool(EditorPrefs.GetBool(string.Format("{0}.HierarchyEnabled", nameof(ToolboxEditorSettings)), false));
            projectAnimBool = new AnimBool(EditorPrefs.GetBool(string.Format("{0}.ProjectEnabled", nameof(ToolboxEditorSettings)), false));
            inspectorAnimBool = new AnimBool(EditorPrefs.GetBool(string.Format("{0}.InspectorEnabled", nameof(ToolboxEditorSettings)), false));

            enabledToShowDrawerType = EditorPrefs.GetInt(string.Format("{0}.PickedDrawerType", nameof(ToolboxEditorSettings)), 0);

            var repaintAction = new UnityAction(() =>
            {
                //NOTE: we have to repaint our editor and already focused (probably ProjectSettings) window
                Repaint();
                EditorWindow.focusedWindow.Repaint();
            });

            hierarchyAnimBool.valueChanged.AddListener(repaintAction);
            projectAnimBool.valueChanged.AddListener(repaintAction);
            inspectorAnimBool.valueChanged.AddListener(repaintAction);

            //hierarchy-related properties
            useToolboxHierarchyProperty = serializedObject.FindProperty("useToolboxHierarchy");
            drawHorizontalLinesProperty = serializedObject.FindProperty("drawHorizontalLines");
            showSelectionsCountProperty = serializedObject.FindProperty("showSelectionsCount");
#if UNITY_2019_3_OR_NEWER
            rowDataItemsList = CreateClearList(serializedObject.FindProperty("rowDataTypes"), hasHeader: false, elementLabel: "Position");
#else
            rowDataItemsList = CreateLinedList(serializedObject.FindProperty("rowDataTypes"), hasHeader: false, elementLabel: "Position");
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

            drawerHandlersLists = new ReorderableListBase[5];
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
            EditorPrefs.SetBool(string.Format("{0}.HierarchyEnabled", nameof(ToolboxEditorSettings)), hierarchyAnimBool.target);
            EditorPrefs.SetBool(string.Format("{0}.ProjectEnabled", nameof(ToolboxEditorSettings)), projectAnimBool.target);
            EditorPrefs.SetBool(string.Format("{0}.InspectorEnabled", nameof(ToolboxEditorSettings)), inspectorAnimBool.target);
            EditorPrefs.SetInt(string.Format("{0}.PickedDrawerType", nameof(ToolboxEditorSettings)), enabledToShowDrawerType);
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
            EditorGUILayout.BeginVertical(Style.layoutsGroupStyle);
#endif
            EditorGUILayout.LabelField("Row Data Types");
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.EndVertical();
#endif
            {
#if UNITY_2019_3_OR_NEWER
                EditorGUILayout.BeginVertical(Style.layoutsGroupStyle);
#endif
                rowDataItemsList.DoList();
#if UNITY_2019_3_OR_NEWER
                EditorGUILayout.EndVertical();
#endif
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(drawHorizontalLinesProperty);
            EditorGUILayout.PropertyField(showSelectionsCountProperty);
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
            EditorGUILayout.BeginVertical(Style.layoutsGroupStyle);
#endif
            EditorGUILayout.LabelField("Custom Folders");
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.EndVertical();
#endif
            {
#if UNITY_2019_3_OR_NEWER
                EditorGUILayout.BeginVertical(Style.layoutsGroupStyle);
#endif
                customFoldersList.DoList();
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
            EditorGUILayout.BeginVertical(Style.layoutsGroupStyle);
#endif
#if UNITY_2019_3_OR_NEWER
            enabledToShowDrawerType = GUILayout.Toolbar(enabledToShowDrawerType, drawerHandlersShortNames, EditorStyles.toolbarButton);
#else
            enabledToShowDrawerType = GUILayout.Toolbar(enabledToShowDrawerType, drawerHandlersShortNames);
#endif
            EditorGUI.BeginDisabledGroup(!useToolboxDrawersProperty.boolValue);

            var longName = drawerHandlersLongNames[enabledToShowDrawerType];
            var tooltip = drawerHandlersInfoLabels[enabledToShowDrawerType];
            var content = new GUIContent(longName + " Drawers", tooltip);
            EditorGUILayout.LabelField(content, Style.drawersHeaderStyle);

#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.EndVertical();
#endif
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.BeginVertical(Style.layoutsGroupStyle);
#endif
            drawerHandlersLists[enabledToShowDrawerType].DoList();
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.EndVertical();
#endif
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.BeginVertical(Style.layoutsGroupStyle);
#endif
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Style.clearAllContent, EditorStyles.miniButtonLeft))
            {
                ClearAllDrawers();
                validateInspector = true;
            }

            if (GUILayout.Button(Style.overrideContent, EditorStyles.miniButtonMid))
            {
                ApplyAllDrawers();
                validateInspector = true;
            }

            if (GUILayout.Button(Style.validateContent, EditorStyles.miniButtonRight))
            {
                ValidateDrawers();
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

        private void ApplyAllDrawers()
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

            EditorUtility.SetDirty(target);
        }

        private void ClearAllDrawers()
        {
            Undo.RecordObject(target, "Clear Drawers");
            switch (enabledToShowDrawerType)
            {
                case 0:
                    currentTarget.DecoratorDrawerHandlers.Clear();
                    break;
                case 1:
                    currentTarget.ConditionDrawerHandlers.Clear();
                    break;
                case 2:
                    currentTarget.SelfPropertyDrawerHandlers.Clear();
                    break;
                case 3:
                    currentTarget.ListPropertyDrawerHandlers.Clear();
                    break;
                case 4:
                    currentTarget.TargetTypeDrawerHandlers.Clear();
                    break;
            }

            EditorUtility.SetDirty(target);
        }

        private void ValidateDrawers()
        {
            ToolboxEditorLog.LogInfo("Function not implemented.");
        }


        public override void DrawCustomInspector()
        {
            serializedObject.Update();
            //handle hierarchy settings section
            hierarchyAnimBool.target = DrawHeaderFoldout(hierarchyAnimBool.target, Style.hierarchySettingsContent, true, Style.sectionHeaderStyle, Style.headersGroupStyle);
            using (var group = new EditorGUILayout.FadeGroupScope(hierarchyAnimBool.faded))
            {
                if (group.visible)
                {
                    DrawHierarchySettings();
                }
            }

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            //handle project settings section
            projectAnimBool.target = DrawHeaderFoldout(projectAnimBool.target, Style.projectSettingsContent, true, Style.sectionHeaderStyle, Style.headersGroupStyle);
            using (var group = new EditorGUILayout.FadeGroupScope(projectAnimBool.faded))
            {
                if (group.visible)
                {
                    DrawProjectSettings();
                }
            }

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            //handle inspector settings section
            inspectorAnimBool.target = DrawHeaderFoldout(inspectorAnimBool.target, Style.inspectorSettingsContent, true, Style.sectionHeaderStyle, Style.headersGroupStyle);
            using (var group = new EditorGUILayout.FadeGroupScope(inspectorAnimBool.faded))
            {
                if (group.visible)
                {
                    DrawInspectorSettings();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }


        internal static class Style
        {
            internal static readonly GUIStyle headersGroupStyle;
            internal static readonly GUIStyle layoutsGroupStyle;
            internal static readonly GUIStyle drawersHeaderStyle;
            internal static readonly GUIStyle sectionHeaderStyle;

            internal static readonly GUIContent clearAllContent = new GUIContent("Clear");
            internal static readonly GUIContent overrideContent = new GUIContent("Assign all possible");
            internal static readonly GUIContent validateContent = new GUIContent("Validate");

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
                headersGroupStyle = new GUIStyle(EditorStyles.helpBox);
#if UNITY_2019_3_OR_NEWER
                layoutsGroupStyle = new GUIStyle(EditorStyles.helpBox);
#else
                layoutsGroupStyle = new GUIStyle();
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