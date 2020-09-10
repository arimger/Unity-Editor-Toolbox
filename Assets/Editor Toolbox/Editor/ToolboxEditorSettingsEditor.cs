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
        private SerializedProperty useToolboxFoldersProperty;
        private SerializedProperty useToolboxDrawersProperty;
        private SerializedProperty drawHorizontalLinesProperty;
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
            useToolboxDrawersProperty = serializedObject.FindProperty("useToolboxDrawers");
            useToolboxFoldersProperty = serializedObject.FindProperty("useToolboxFolders");
            drawHorizontalLinesProperty = serializedObject.FindProperty("drawHorizontalLines");
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
            serializedObject.Update();

            //forcing validation is usefull when target field is edited directly
            //NOTE: check InspectorUtility for more details
            var forceValidation = false;

            //handle hierarchy settings section
            if (hierarchySettingsEnabled = ToolboxEditorGui.DrawLayoutHeaderFoldout(hierarchySettingsEnabled, Style.hierarchySettingsContent, true, Style.bigSectionFoldoutStyle))
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(useToolboxHierarchyProperty);
                EditorGUILayout.Space();

                EditorGUI.BeginDisabledGroup(!useToolboxHierarchyProperty.boolValue);
                if (ToolboxEditorGui.DrawListFoldout(rowDataItemsList, Style.normalListFoldoutStyle))
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
                    //hierarchy settings changed
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
                    currentTarget.ResetIconProperties();
                    forceValidation = true;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                //draw custom icons list
                if (ToolboxEditorGui.DrawListFoldout(customFoldersList, Style.normalListFoldoutStyle))
                {
                    customFoldersList.DoLayoutList();
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
                if (EditorGUI.EndChangeCheck())
                {
                    //project settings changed
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

                if (ToolboxEditorGui.DrawDrawerList(decoratorDrawerHandlersList, "Decorator Drawers", assignButtonLabel, Style.drawerListFoldoutStyle))
                {
                    currentTarget.SetAllPossibleDecoratorDrawers();
                    forceValidation = true;
                }

                if (ToolboxEditorGui.DrawDrawerList(conditionDrawerHandlersList, "Condition Drawers", assignButtonLabel, Style.drawerListFoldoutStyle))
                {
                    currentTarget.SetAllPossibleConditionDrawers();
                    forceValidation = true;
                }

                if (ToolboxEditorGui.DrawDrawerList(selfPropertyDrawerHandlersList, "Self Property Drawers", assignButtonLabel, Style.drawerListFoldoutStyle))
                {
                    currentTarget.SetAllPossibleSelfPropertyDrawers();
                    forceValidation = true;
                }

                if (ToolboxEditorGui.DrawDrawerList(listPropertyDrawerHandlersList, "List Property Drawers", assignButtonLabel, Style.drawerListFoldoutStyle))
                {
                    currentTarget.SetAllPossibleListPropertyDrawers();
                    forceValidation = true;
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Type-based", Style.smallHeaderStyle);

                if (ToolboxEditorGui.DrawDrawerList(targetTypeDrawerHandlersList, "Target Type Drawers", assignButtonLabel, Style.drawerListFoldoutStyle))
                {
                    currentTarget.SetAllPossibleTargetTypeDrawers();
                    forceValidation = true;
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
                if (EditorGUI.EndChangeCheck())
                {
                    //inspectors settings changed
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                GUILayout.Space(-Style.spacing / 2);
            }

            if (currentTarget.NeedsUpdate || forceValidation)
            {
                currentTarget.ForceUpdate();
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

            internal static readonly GUIContent hierarchySettingsContent = new GUIContent("Hierarchy Settings",
                EditorGUIUtility.IconContent("UnityEditor.HierarchyWindow").image);
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