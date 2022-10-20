using System;
using System.Reflection;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Toolbox.Editor.Windows
{
    public class ScriptableObjectCreationWindow : EditorWindow
    {
        private SearchField searchField;


        [MenuItem("Assets/Create/ScriptableObject Creation Window", priority = 5)]
        internal static void Initialize()
        {
            var window = GetWindow<ScriptableObjectCreationWindow>();
            window.titleContent = new GUIContent("ScriptableObject Creation Window");
            window.Show();
        }


        private void OnEnable()
        {
            searchField = new SearchField();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    DrawSearchPanel();
                }

                using (new EditorGUILayout.VerticalScope())
                {
                    DrawCreatePanel();
                }
            }
        }

        private void DrawSearchPanel()
        {
            var rect = GUILayoutUtility.GetRect(100.0f, 16.0f);
            searchField.OnGUI(rect, string.Empty);
        }

        private void DrawCreatePanel()
        {
            if (GUILayout.Button("Create"))
            {
                Debug.Log(GetActiveFolderPath());
            }
        }

        private static string GetActiveFolderPath()
        {
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            string pathToCurrentFolder = obj.ToString();
            return pathToCurrentFolder;
        }
    }
}