using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    [CanEditMultipleObjects, CustomEditor(typeof(ComponentEditorSettings), true, isFallback = false)]
    public class ComponentEditorSettingsEditor : ComponentEditor
    {
        private SerializedProperty useToolboxDrawersProperty;

        private ReorderableList areaDrawerHandlersList;
        private ReorderableList groupDrawerHandlersList;
        private ReorderableList propertyDrawerHandlersList;
        private ReorderableList conditionDrawerHandlersList;


        protected override void OnEnable()
        {
            useToolboxDrawersProperty = serializedObject.FindProperty("useToolboxDrawers");

            areaDrawerHandlersList = ToolboxEditorUtility.CreateBoxedList(serializedObject.FindProperty("areaDrawerHandlers"));
            groupDrawerHandlersList = ToolboxEditorUtility.CreateBoxedList(serializedObject.FindProperty("groupDrawerHandlers"));
            propertyDrawerHandlersList = ToolboxEditorUtility.CreateBoxedList(serializedObject.FindProperty("propertyDrawerHandlers"));
            conditionDrawerHandlersList = ToolboxEditorUtility.CreateBoxedList(serializedObject.FindProperty("conditionDrawerHandlers"));
        }

        protected override void OnDisable()
        { }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(useToolboxDrawersProperty);
            if (useToolboxDrawersProperty.boolValue)
            {
                EditorGUILayout.HelpBox("Select all wanted drawers and press \"Apply\" button.", MessageType.Info);
                areaDrawerHandlersList.DoLayoutList();    
                EditorGUILayout.Separator();
                groupDrawerHandlersList.DoLayoutList();
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
                ComponentEditorUtility.ReimportEditor();
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