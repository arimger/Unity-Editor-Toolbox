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
        private GUIStyle buttonStyle;
        private Color selectionColor = new Color(0.50f, 0.70f, 1.00f);

        private int shiftMinSelectionID = -1;
        private int shiftMaxSelectionID = -1;
        private int shiftLastID = -1;

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

            size = Vector2.zero;

            foreach (GameObject go in gameObjects)
            {
                GUIContent content = EditorGUIUtility.ObjectContent(go, typeof(GameObject));
                Vector2 currentSize = buttonStyle.CalcSize(content);
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

            if (Event.current.type == EventType.Layout)
            {
                return;
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
            Rect rect = new Rect(sizeXPadding, sizeYPadding, buttonSize.x, buttonSize.y);

            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];

                if(gameObject == null)
                {
                    //Can happen when something removes the gameobject during the window display.
                    continue;
                }

                GUIContent content = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject));

                bool objectSelected = Selection.Contains(gameObject);

                if (objectSelected)
                {
                    GUI.backgroundColor = selectionColor;
                }

                if (GUI.Button(rect, content, buttonStyle))
                {
                    GameObjectButtonPress(i);
                }

                GUI.backgroundColor = Color.white;

                rect.y += buttonYSize + buttonYSpacing;
            }
        }

        private void OnGUI_MouseMove()
        {
            Rect rect = new Rect(sizeXPadding, sizeYPadding, buttonSize.x, buttonSize.y);

            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];

                if (gameObject == null)
                {
                    //Can happen when something removes the gameobject during the window display.
                    continue;
                }

                GUIContent content = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject));

                GUI.Button(rect, content, buttonStyle);

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
            if(shiftLastID == -1)
            {
                shiftLastID = id;
            }

            if(shiftMinSelectionID == -1)
            {
                shiftMinSelectionID = id;
            }

            if (shiftMaxSelectionID == -1)
            {
                shiftMaxSelectionID = id;
            }

            if(id < shiftMinSelectionID)
            {
                shiftMinSelectionID = id;
            }
            else if(id >= shiftMaxSelectionID)
            {
                shiftMaxSelectionID = id;
            }
            else if(id > shiftMinSelectionID)
            {
                //ID is between min and max.
                if(shiftLastID < id)
                {
                    shiftMaxSelectionID = id;
                }
                else
                {
                    shiftMinSelectionID = id;
                }
            }

            shiftLastID = id;
        }

        private void SelectObject(int id, bool control, bool shift)
        {
            GameObject gameObject = gameObjects[id];

            if (shift)
            {
                UpdateShiftSelectionIDs(id);
                SelectObjects(shiftMinSelectionID, shiftMaxSelectionID);
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
            int size = maxID - minID + 1;
            Object[] newSelection = new Object[size];

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
            Object[] currentSelection = Selection.objects;
            Object[] newSelection = new Object[currentSelection.Length + 1];

            currentSelection.CopyTo(newSelection, 0);
            newSelection[newSelection.Length - 1] = gameObject;

            Selection.objects = newSelection;
        }

        private void RemoveObjectFromSelection(GameObject gameObject)
        {
            Object[] currentSelection = Selection.objects;
            Object[] newSelection = new Object[currentSelection.Length - 1];

            int index = 0;

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

        private void OnGUI_MouseLeave()
        {
            HighlightedObject = null;
        }
    }
}