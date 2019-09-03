using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    /// <summary>
    /// Class responsible for folder custom icons drawing.
    /// </summary>
    [InitializeOnLoad]
    public static class ProjectEditorWindow
    {
        static ProjectEditorWindow()
        {
            EditorApplication.projectWindowItemOnGUI += OnItemCallback;
        }

        private static void OnItemCallback(string guid, Rect rect)
        {
            //EditorGUI.DrawRect(rect, Color.black);
        }
    }
}