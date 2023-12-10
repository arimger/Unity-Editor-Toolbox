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
            const int maxIterations = 50;

            var mousePosition = Event.current.mousePosition;
            var hitObjects = new List<GameObject>();
            GameObject[] hitObjectsArray;
            for (var i = 0; i < maxIterations; i++)
            {
                hitObjectsArray = hitObjects.ToArray();

                var go = HandleUtility.PickGameObject(mousePosition, false, hitObjectsArray);
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
#if UNITY_2019_1_OR_NEWER
            UnityEditor.SceneView.duringSceneGui -= SceneViewDuringSceneGui;

            if (UseToolboxSceneView)
            {
                UnityEditor.SceneView.duringSceneGui += SceneViewDuringSceneGui;
            }
#endif
        }

        private static void SceneViewDuringSceneGui(UnityEditor.SceneView sceneView)
        {
            if (Event.current.type != EventType.KeyDown ||
                Event.current.keyCode != SelectorKey)
            {
                return;
            }

            var objectsUnderCursor = GetObjectsUnderCursor();
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
