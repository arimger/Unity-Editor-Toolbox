using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    [CustomEditor(typeof(ToolboxEditorSettings), true, isFallback = false)]
    [CanEditMultipleObjects]
    public class ToolboxEditorSettingsEditor : ToolboxEditor
    {
        private SerializedProperty useToolboxDrawersProperty;
        private SerializedProperty useToolboxHierarchyProperty;

        private ReorderableList areaDrawerHandlersList;
        private ReorderableList groupDrawerHandlersList;
        private ReorderableList propertyDrawerHandlersList;
        private ReorderableList conditionDrawerHandlersList;


        protected override void OnEnable()
        {       
            useToolboxDrawersProperty = serializedObject.FindProperty("useToolboxDrawers");
            useToolboxHierarchyProperty = serializedObject.FindProperty("useToolboxHierarchy");

            areaDrawerHandlersList = ToolboxEditorGui.CreateBoxedList(serializedObject.FindProperty("areaDrawerHandlers"));
            groupDrawerHandlersList = ToolboxEditorGui.CreateBoxedList(serializedObject.FindProperty("groupDrawerHandlers"));
            propertyDrawerHandlersList = ToolboxEditorGui.CreateBoxedList(serializedObject.FindProperty("propertyDrawerHandlers"));
            conditionDrawerHandlersList = ToolboxEditorGui.CreateBoxedList(serializedObject.FindProperty("conditionDrawerHandlers"));
        }

        protected override void OnDisable()
        { }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(useToolboxHierarchyProperty);
            EditorGUILayout.PropertyField(useToolboxDrawersProperty);
            if (useToolboxDrawersProperty.boolValue)
            {
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
            }
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
            internal static GUIContent buttonContent;

            internal static GUILayoutOption[] buttonOptions;

            static Style()
            {
                buttonContent = new GUIContent("Apply", "Apply changes");

                buttonOptions = new GUILayoutOption[]
                {
                    GUILayout.Width(80)
                };
            }
        }
    }
}