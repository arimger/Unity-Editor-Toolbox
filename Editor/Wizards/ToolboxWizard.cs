using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Wizards
{
    using Editor = UnityEditor.Editor;

    public class ToolboxWizard : EditorWindow
    {
        private static readonly string[] propertiesToIgnore = new string[]
        {
            PropertyUtility.Defaults.scriptPropertyName,
            "m_SerializedDataModeController"
        };

        private Editor targetEditor;

        private Vector2 scrollPosition;

        protected virtual void OnEnable()
        {
            if (targetEditor != null)
            {
                ReinitEditor(targetEditor);
            }
        }

        protected virtual void OnDestroy()
        {
            DestroyImmediate(targetEditor);
        }

        private void OnGUI()
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollView.scrollPosition;
                EditorGUI.BeginChangeCheck();
                OnWizardGui();
                if (EditorGUI.EndChangeCheck())
                {
                    OnWizardUpdate();
                }
            }

            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    HandleOtherButtons();
                    GUI.enabled = IsValid;
                    if (HandleCreateButton())
                    {
                        OnWizardCreate();
                        if (CloseOnCreate)
                        {
                            Close();
                            GUIUtility.ExitGUI();
                        }
                    }

                    GUI.enabled = true;
                }

                GUILayout.Space(5);
            }
        }

        private void CreateEditor()
        {
            if (targetEditor != null)
            {
                return;
            }

            targetEditor = Editor.CreateEditor(this);
            ReinitEditor(targetEditor);
            OnWizardUpdate();
        }

        private void ReinitEditor(Editor editor)
        {
            if (editor == null)
            {
                return;
            }

            editor.hideFlags = HideFlags.HideAndDontSave;
            if (editor is ToolboxEditor toolboxEditor)
            {
                foreach (var property in propertiesToIgnore)
                {
                    toolboxEditor.IgnoreProperty(property);
                }
            }
        }

        protected virtual void OnWizardCreate()
        { }

        protected virtual void OnWizardUpdate()
        { }

        protected virtual void OnWizardGui()
        {
            CreateEditor();
            targetEditor.OnInspectorGUI();
        }

        protected virtual bool HandleCreateButton()
        {
            return GUILayout.Button("Create", GUILayout.MinWidth(100));
        }

        protected virtual void HandleOtherButtons()
        { }

        public static T DisplayWizard<T>(string title) where T : ToolboxWizard
        {
            return GetWindow<T>(true, title);
        }

        protected virtual bool IsValid { get; set; } = true;

        protected virtual bool CloseOnCreate => true;
    }
}