using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.SceneView;

    [InitializeOnLoad]
    public static class ToolboxEditorSceneView
    {
        static ToolboxEditorSceneView()
        {
            UpdateEventCallback();
        }

        private static List<GameObject> GetObjectsUnderCursor()
        {
            List<GameObject> hitObjects = new List<GameObject>();
            GameObject[] hitObjectsArray;

            int maxIterations = 50;
            for (int i = 0; i < maxIterations; i++)
            {
                hitObjectsArray = hitObjects.ToArray();

                GameObject go = HandleUtility.PickGameObject(Event.current.mousePosition, true, hitObjectsArray);
                if (go == null)
                {
                    break;
                }

                hitObjects.Add(go);
            }

            return hitObjects;
        }

        private static void UpdateEventCallback()
        {
            UnityEditor.SceneView.duringSceneGui -= SceneViewDuringSceneGUI;

            if (UseToolboxSceneView)
            {
                UnityEditor.SceneView.duringSceneGui += SceneViewDuringSceneGUI;
            }
        }

        private static void SceneViewDuringSceneGUI(UnityEditor.SceneView sceneView)
        {
            if (Event.current.type != EventType.KeyDown
                || Event.current.keyCode != SelectorKey)
            {
                return;
            }

            List<GameObject> objectsUnderCursor = GetObjectsUnderCursor();
            if (objectsUnderCursor.Count > 0)
            {
                ToolboxEditorSceneViewObjectSelector.Show(objectsUnderCursor, Event.current.mousePosition + sceneView.position.position);
            }
        }

        internal static void UpdateSettings(IToolboxSceneViewSettings settings)
        {
            if (settings == null)
            {
                UseToolboxSceneView = true;
                SelectorKey = KeyCode.LeftControl;
            }
            else
            {
                UseToolboxSceneView = settings.UseToolboxSceneView;
                SelectorKey = settings.SelectorKey;
            }

            UpdateEventCallback();
        }

        /// <summary>
        /// Should the scene view be used.
        /// </summary>
        private static bool UseToolboxSceneView { get; set; } = true;

        /// <summary>
        /// Which key should activate the scene view.
        /// </summary>
        private static KeyCode SelectorKey { get; set; } = KeyCode.LeftControl;
    }
}
