using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    [CanEditMultipleObjects, CustomEditor(typeof(ComponentEditorSettings), true, isFallback = false)]
    public class ComponentEditorSettingsEditor : ComponentEditor
    {
        private SerializedProperty useOrderedDrawersProperty;

        private ReorderableList presetHandlersList;
        private ReorderableList propertyHandlersList;


        protected override void OnEnable()
        {
            useOrderedDrawersProperty = serializedObject.FindProperty("useOrderedDrawers");

            presetHandlersList = ToolboxEditorUtility.CreateBoxedList(serializedObject.FindProperty("presetHandlers"));
            propertyHandlersList = ToolboxEditorUtility.CreateBoxedList(serializedObject.FindProperty("propertyHandlers"));
        }

        protected override void OnDisable()
        { }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(useOrderedDrawersProperty);
            if (useOrderedDrawersProperty.boolValue)
            {
                EditorGUILayout.HelpBox("Select all wanted property drawers in specific order. " +
                        "Remember that they will be drawn in provided way, " +
                        "if any ordered drawer interrupts this cycle, " +
                        "then each next one will be ignored.", MessageType.Info);
                presetHandlersList.DoLayoutList();
                EditorGUILayout.Space();
                propertyHandlersList.DoLayoutList();
            }
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button(Style.buttonContent, Style.buttonOptions))
            {
                ComponentEditorUtility.ReimportEditor();
            }
        }


        internal static class Style
        {
            internal static GUIContent buttonContent;

            internal static GUILayoutOption[] buttonOptions;

            static Style()
            {
                buttonContent = new GUIContent("Apply", "Apply all changes");

                buttonOptions = new GUILayoutOption[]
                {
                    GUILayout.Width(80)
                };
            }
        }
    }
}