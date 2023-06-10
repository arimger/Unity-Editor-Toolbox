using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.SceneView
{
    public class ToolboxEditorSceneViewObjectSelector : EditorWindow
    {
        private List<GameObject> gameObjects;
        private List<string> gameObjectPaths;

        private const float sizeXPadding = 8f;
        private const float sizeYPadding = 4f;
        private const float sizeYspacing = 2.0f;

        private GameObject highlightedObject;
        private Vector2 size;
        private GUIStyle buttonStyle;

        private GameObject HighlightedObject
        {
            set
            {
                if (highlightedObject == value)
                {
                    return;
                }

                highlightedObject = value;
                Selection.activeGameObject = value;
                UnityEditor.SceneView.RepaintAll();
            }
        }

        public static void Show(List<GameObject> gameObjects, Vector2 position)
        {
            Rect rect = new Rect(position, Vector2.one);

            ToolboxEditorSceneViewObjectSelector window = CreateInstance<ToolboxEditorSceneViewObjectSelector>();
            window.wantsMouseMove = true;
            window.wantsMouseEnterLeaveWindow = true;
            window.gameObjects = gameObjects;
            window.InitializeGameobjectPaths();
            window.CalculateSize();
            window.ShowAsDropDown(rect, window.size);
        }

        private void InitializeStyle()
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleLeft;
        }

        private Vector2 CalculateSize()
        {
            if (buttonStyle == null)
            {
                InitializeStyle();
            }

            GUIContent content = new GUIContent();

            size = Vector2.zero;

            foreach (string path in gameObjectPaths)
            {
                content.text = path;
                Vector2 currentSize = buttonStyle.CalcSize(content);
                if (currentSize.x > size.x)
                {
                    size.x = currentSize.x;
                }

                size.y += currentSize.y + sizeYspacing;
            }

            size.x += sizeXPadding;
            size.y += sizeYPadding;

            return size;
        }

        private void InitializeGameobjectPaths()
        {
            gameObjectPaths = new List<string>();
            Stack<string> pathStack = new Stack<string>();

            for (int i = 0; i < gameObjects.Count; i++)
            {
                pathStack.Clear();
                Transform transf = gameObjects[i].transform;
                pathStack.Push(transf.gameObject.name);

                while (transf.parent != null)
                {
                    transf = transf.parent;
                    pathStack.Push(transf.gameObject.name);
                }

                string path = string.Join("/", pathStack.ToArray());
                gameObjectPaths.Add(path);
            }
        }

        private void OnGUI()
        {
            if (buttonStyle == null)
            {
                InitializeStyle();
            }

            //Debug.Log($"Event : {Event.current.type}");

            if (Event.current.type == EventType.MouseMove)
            {
                OnGUI_MouseMove();
            }
            else if (Event.current.type == EventType.MouseLeaveWindow)
            {
                OnGUI_MouseLeave();
            }
            else
            {
                OnGUI_Normal();
            }
        }

        private void OnGUI_Normal()
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];
                if (GUILayout.Button(gameObjectPaths[i], buttonStyle))
                {
                    Selection.activeGameObject = gameObject;
                    Event.current.Use();
                    Close();
                }
            }
        }

        private void OnGUI_MouseMove()
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];
                if (GUILayout.Button(gameObjectPaths[i], buttonStyle))
                {
                    Selection.activeGameObject = gameObject;
                    Event.current.Use();
                    Close();
                }

                Rect lastRect = GUILayoutUtility.GetLastRect();

                if (lastRect.Contains(Event.current.mousePosition))
                {
                    HighlightedObject = gameObject;
                }
            }
        }

        private void OnGUI_MouseLeave()
        {
            HighlightedObject = null;
        }
    }
}