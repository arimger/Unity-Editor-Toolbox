using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Core
{
    using Editor = UnityEditor.Editor;

    internal class ToolboxSettingsProvider
    {
        private static Editor settingsEditor;

        [SettingsProvider]
        internal static SettingsProvider GetProvider()
        {
            var provider = new SettingsProvider("Project/Editor Toolbox v2", SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    if (settingsEditor == null || settingsEditor.serializedObject.targetObject == null)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField(string.Format("Cannot access settings file"));
                        EditorGUILayout.Space();
                        return;
                    }

                    using (var scope = new EditorGUI.ChangeCheckScope())
                    {
                        settingsEditor.OnInspectorGUI();
                        if (scope.changed)
                        {
                            ToolboxSettings.instance.Save();
                        }
                    }
                },
                activateHandler = (searchContext, elements) =>
                {
                    var instance = ToolboxSettings.instance;
                    settingsEditor = Editor.CreateEditor(instance);
                },
                deactivateHandler = () =>
                {
                    if (settingsEditor == null)
                    {
                        return;
                    }

                    Object.DestroyImmediate(settingsEditor);
                }
            };

            return provider;
        }
    }
}