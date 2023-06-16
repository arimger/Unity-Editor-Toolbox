using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.SceneView
{
    public class ToolboxEditorSceneViewObjectSelector : EditorWindow
    {
        private List<GameObject> gameObjects;
        private List<string> gameObjectPaths;

        private const float sizeXPadding = 2f;
        private const float sizeYPadding = 2f;
        private const float buttonYSpacing = 0.0f;
        private const float buttonYSize = 20f;
        private const float sizeXOffset = -30f;

        private GameObject highlightedObject;
        private Vector2 size;
        private Vector2 buttonSize;
        private static readonly Color selectionColor = new Color(0.50f, 0.70f, 1.00f);

        private int shiftMinSelectionId = -1;
        private int shiftMaxSelectionId = -1;
        private int shiftLastId = -1;

        public static void Show(List<GameObject> gameObjects, Vector2 position)
        {
            var rect = new Rect(position, Vector2.one);

            var window = CreateInstance<ToolboxEditorSceneViewObjectSelector>();
            window.wantsMouseMove = true;
            window.wantsMouseEnterLeaveWindow = true;
            window.gameObjects = gameObjects;
            window.InitializeGameObjectPaths();
            window.CalculateSize();
            window.ShowAsDropDown(rect, window.size);
        }

        private Vector2 CalculateSize()
        {
            size = Vector2.zero;

            foreach (var go in gameObjects)
            {
                GUIContent content = EditorGUIUtility.ObjectContent(go, typeof(GameObject));
                Vector2 currentSize = Style.buttonStyle.CalcSize(content);
                if (currentSize.x > size.x)
                {
                    size.x = currentSize.x;
                }
            }

            //This is needed because CalcSize calculates content drawing with icon at full size.
            size.x += sizeXOffset;

            buttonSize.x = size.x;
            buttonSize.y = buttonYSize;

            size.y = gameObjects.Count * buttonYSize + sizeYPadding * 2.0f + buttonYSpacing * gameObjects.Count - 1;
            size.x += sizeXPadding * 2.0f;

            return size;
        }

        private void InitializeGameObjectPaths()
        {
            gameObjectPaths = new List<string>();
            var pathStack = new Stack<string>();

            for (int i = 0; i < gameObjects.Count; i++)
            {
                pathStack.Clear();
                Transform transform = gameObjects[i].transform;
                pathStack.Push(transform.gameObject.name);

                while (transform.parent != null)
                {
                    transform = transform.parent;
                    pathStack.Push(transform.gameObject.name);
                }

                string path = string.Join("/", pathStack.ToArray());
                gameObjectPaths.Add(path);
            }
        }

        private void OnGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            switch (Event.current.type)
            {
                case EventType.MouseMove:
                {
                    OnGUIMouseMove();
                    break;
                }
                case EventType.MouseLeaveWindow: 
                {
                    OnGUIMouseLeave();
                    break;
                }
                default:
                {
                    OnGUINormal();
                    break;
                }
            }
        }

        private void OnGUINormal()
        {
            Rect rect = new Rect(sizeXPadding, sizeYPadding, buttonSize.x, buttonSize.y);

            for (int i = 0; i < gameObjects.Count; i++)
            {
                var gameObject = gameObjects[i];

                if(gameObject == null)
                {
                    //Can happen when something removes the gameobject during the window display.
                    continue;
                }

                var content = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject));

                bool objectSelected = Selection.Contains(gameObject);

                if (objectSelected)
                {
                    GUI.backgroundColor = selectionColor;
                }

                if (GUI.Button(rect, content, Style.buttonStyle))
                {
                    GameObjectButtonPress(i);
                }

                GUI.backgroundColor = Color.white;

                rect.y += buttonYSize + buttonYSpacing;
            }
        }

        private void OnGUIMouseMove()
        {
            var rect = new Rect(sizeXPadding, sizeYPadding, buttonSize.x, buttonSize.y);

            for (int i = 0; i < gameObjects.Count; i++)
            {
                var gameObject = gameObjects[i];

                if (gameObject == null)
                {
                    //Can happen when something removes the gameobject during the window display.
                    continue;
                }

                var content = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject));

                GUI.Button(rect, content, Style.buttonStyle);

                if (rect.Contains(Event.current.mousePosition))
                {
                    HighlightedObject = gameObject;
                }

                rect.y += buttonYSize + buttonYSpacing;
            }
        }

        private void GameObjectButtonPress(int id)
        {
            SelectObject(id, Event.current.control, Event.current.shift);

            if (Event.current.control || Event.current.shift)
            {
                return;
            }

            Close();
        }

        private void UpdateShiftSelectionIDs(int id)
        {
            if(shiftLastId == -1)
            {
                shiftLastId = id;
            }

            if(shiftMinSelectionId == -1)
            {
                shiftMinSelectionId = id;
            }

            if (shiftMaxSelectionId == -1)
            {
                shiftMaxSelectionId = id;
            }

            if(id < shiftMinSelectionId)
            {
                shiftMinSelectionId = id;
            }
            else if(id >= shiftMaxSelectionId)
            {
                shiftMaxSelectionId = id;
            }
            else if(id > shiftMinSelectionId)
            {
                //ID is between min and max.
                if(shiftLastId < id)
                {
                    shiftMaxSelectionId = id;
                }
                else
                {
                    shiftMinSelectionId = id;
                }
            }

            shiftLastId = id;
        }

        private void SelectObject(int id, bool control, bool shift)
        {
            var gameObject = gameObjects[id];

            if (shift)
            {
                UpdateShiftSelectionIDs(id);
                SelectObjects(shiftMinSelectionId, shiftMaxSelectionId);
            }
            else if (control)
            {
                UpdateShiftSelectionIDs(id);

                if (Selection.Contains(gameObject))
                {
                    //Deselect
                    RemoveObjectFromSelection(gameObject);
                }
                else
                {
                    //Select
                    AddObjectToSelection(gameObject);
                }
            }
            else
            {
                Selection.objects = new Object[] { gameObject };
            }
        }

        private void SelectObjects(int minID, int maxID)
        {
            var size = maxID - minID + 1;
            var newSelection = new Object[size];

            int index = 0;

            for (int i = minID; i <= maxID; i++)
            {
                newSelection[index] = gameObjects[i];
                index++;
            }

            Selection.objects = newSelection;
        }

        private void AddObjectToSelection(GameObject gameObject)
        {
            var currentSelection = Selection.objects;
            var newSelection = new Object[currentSelection.Length + 1];

            currentSelection.CopyTo(newSelection, 0);
            newSelection[newSelection.Length - 1] = gameObject;

            Selection.objects = newSelection;
        }

        private void RemoveObjectFromSelection(GameObject gameObject)
        {
            var currentSelection = Selection.objects;
            var newSelection = new Object[currentSelection.Length - 1];

            var index = 0;

            for (int i = 0; i < currentSelection.Length; i++)
            {
                if (currentSelection[i] == gameObject)
                {
                    continue;
                }

                newSelection[index] = currentSelection[i];
                index++;
            }

            Selection.objects = newSelection;
        }

        private void OnGUIMouseLeave()
        {
            HighlightedObject = null;
        }

        private GameObject HighlightedObject
        {
            set
            {
                if (highlightedObject == value)
                {
                    return;
                }

                highlightedObject = value;
                UnityEditor.SceneView.RepaintAll();

                if (highlightedObject != null)
                {
                    EditorGUIUtility.PingObject(highlightedObject);
                }
            }
        }

        private static class Style
        {
            internal static readonly GUIStyle buttonStyle;

            static Style()
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.alignment = TextAnchor.MiddleLeft;
            }
        }
    }
}