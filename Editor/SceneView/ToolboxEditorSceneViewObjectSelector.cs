using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.SceneView
{
    public class ToolboxEditorSceneViewObjectSelector : EditorWindow
    {
        private static readonly Color selectionColor = new Color(0.50f, 0.70f, 1.00f);
        private static readonly Color highlightWireColor = Color.yellow;

        private const float sizeXPadding = 2f;
        private const float sizeYPadding = 2f;
        private const float buttonYSpacing = 0.0f;
        private const float buttonYSize = 20f;
        private const float sizeXOffset = -30f;
        private const float indentWidth = 12f;

        private readonly List<Renderer> highlightedRenderers = new List<Renderer>();

        private List<DisplayEntry> displayEntries;
        private GameObject highlightedObject;
        private Vector2 size;
        private Vector2 buttonSize;

        private int shiftMinSelectionId = -1;
        private int shiftMaxSelectionId = -1;
        private int shiftLastId = -1;

        public static void Show(List<GameObject> gameObjects, Vector2 position)
        {
            var rect = new Rect(position, Vector2.one);

            var window = CreateInstance<ToolboxEditorSceneViewObjectSelector>();
            window.wantsMouseMove = true;
            window.wantsMouseEnterLeaveWindow = true;
            window.displayEntries = window.BuildDisplayEntries(gameObjects);
            window.CalculateSize();
            window.ShowAsDropDown(rect, window.size);
        }

        private void OnEnable()
        {
#if UNITY_2019_1_OR_NEWER
            UnityEditor.SceneView.duringSceneGui += OnSceneViewGui;
#endif
        }

        private void OnDisable()
        {
#if UNITY_2019_1_OR_NEWER
            UnityEditor.SceneView.duringSceneGui -= OnSceneViewGui;
#endif
            highlightedRenderers.Clear();
            highlightedObject = null;
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
                    OnGuiMouseMove();
                    break;
                case EventType.MouseLeaveWindow:
                    OnGuiMouseLeave();
                    break;
                default:
                    OnGuiNormal();
                    break;
            }
        }

        private void OnGuiMouseLeave()
        {
            HighlightedObject = null;
        }

        private void OnGuiMouseMove()
        {
            var rect = new Rect(sizeXPadding, sizeYPadding, buttonSize.x, buttonSize.y);
            for (var i = 0; i < displayEntries.Count; i++)
            {
                var entry = displayEntries[i];
                var gameObject = entry.GameObject;
                if (gameObject == null)
                {
                    //Can happen when something removes the gameobject during the window display.
                    continue;
                }

                var indentOffset = entry.Depth * indentWidth;
                var drawRect = new Rect(rect);
                drawRect.x += indentOffset;
                drawRect.width -= indentOffset;

                var content = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject));
                GUI.Button(drawRect, content, Style.buttonStyle);
                if (drawRect.Contains(Event.current.mousePosition))
                {
                    HighlightedObject = gameObject;
                }

                rect.y += buttonYSize + buttonYSpacing;
            }
        }

        private void OnGuiNormal()
        {
            var rect = new Rect(sizeXPadding, sizeYPadding, buttonSize.x, buttonSize.y);
            for (var i = 0; i < displayEntries.Count; i++)
            {
                var entry = displayEntries[i];
                var gameObject = entry.GameObject;
                if (gameObject == null)
                {
                    //Can happen when something removes the gameobject during the window display.
                    continue;
                }

                var indentOffset = entry.Depth * indentWidth;
                var drawRect = new Rect(rect);
                drawRect.x += indentOffset;
                drawRect.width -= indentOffset;

                var content = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject));
                var objectSelected = Selection.Contains(gameObject);
                if (objectSelected)
                {
                    GUI.backgroundColor = selectionColor;
                }

                if (GUI.Button(drawRect, content, Style.buttonStyle))
                {
                    GameObjectButtonPress(i);
                }

                GUI.backgroundColor = Color.white;
                rect.y += buttonYSize + buttonYSpacing;
            }
        }

        private Vector2 CalculateSize()
        {
            size = Vector2.zero;

            var maxIndent = 0f;
            foreach (var entry in displayEntries)
            {
                var content = EditorGUIUtility.ObjectContent(entry.GameObject, typeof(GameObject));
                var currentSize = Style.buttonStyle.CalcSize(content);
                if (currentSize.x > size.x)
                {
                    size.x = currentSize.x;
                }

                var currentIndent = entry.Depth * indentWidth;
                if (currentIndent > maxIndent)
                {
                    maxIndent = currentIndent;
                }
            }

            size.x += maxIndent;
            //This is needed because CalcSize calculates content drawing with icon at full size.
            size.x += sizeXOffset;

            buttonSize.x = size.x;
            buttonSize.y = buttonYSize;

            size.y = displayEntries.Count * buttonYSize + sizeYPadding * 2.0f + buttonYSpacing * displayEntries.Count - 1;
            size.x += sizeXPadding * 2.0f;

            return size;
        }

        private List<DisplayEntry> BuildDisplayEntries(List<GameObject> objectsUnderCursor)
        {
            var entries = new List<DisplayEntry>();

            if (objectsUnderCursor == null || objectsUnderCursor.Count == 0)
            {
                return entries;
            }

            var underCursorSet = new HashSet<Transform>();
            foreach (var gameObject in objectsUnderCursor)
            {
                if (gameObject == null)
                {
                    continue;
                }

                underCursorSet.Add(gameObject.transform);
            }

            var relevantTransforms = new HashSet<Transform>();
            foreach (var transform in underCursorSet)
            {
                var current = transform;
                while (current != null && relevantTransforms.Add(current))
                {
                    current = current.parent;
                }
            }

            var orderedRoots = new List<Transform>();
            foreach (var gameObject in objectsUnderCursor)
            {
                if (gameObject == null)
                {
                    continue;
                }

                var top = gameObject.transform;
                while (top.parent != null && relevantTransforms.Contains(top.parent))
                {
                    top = top.parent;
                }

                if (!orderedRoots.Contains(top))
                {
                    orderedRoots.Add(top);
                }
            }

            foreach (var root in orderedRoots)
            {
                AppendHierarchy(root, 0, relevantTransforms, entries);
            }

            return entries;
        }

        private void AppendHierarchy(Transform current, int depth, HashSet<Transform> relevantTransforms, List<DisplayEntry> entries)
        {
            entries.Add(new DisplayEntry(current.gameObject, depth));

            for (var i = 0; i < current.childCount; i++)
            {
                var child = current.GetChild(i);
                if (!relevantTransforms.Contains(child))
                {
                    continue;
                }

                AppendHierarchy(child, depth + 1, relevantTransforms, entries);
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
            if (shiftLastId == -1)
            {
                shiftLastId = id;
            }

            if (shiftMinSelectionId == -1)
            {
                shiftMinSelectionId = id;
            }

            if (shiftMaxSelectionId == -1)
            {
                shiftMaxSelectionId = id;
            }

            if (id < shiftMinSelectionId)
            {
                shiftMinSelectionId = id;
            }
            else if (id >= shiftMaxSelectionId)
            {
                shiftMaxSelectionId = id;
            }
            else if (id > shiftMinSelectionId)
            {
                //ID is between min and max.
                if (shiftLastId < id)
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
            var gameObject = displayEntries[id].GameObject;

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
                    RemoveSelectedObject(gameObject);
                }
                else
                {
                    AppendSelectedObject(gameObject);
                }
            }
            else
            {
                Selection.objects = new Object[]
                {
                    gameObject
                };
            }
        }

        private void SelectObjects(int minId, int maxId)
        {
            var size = maxId - minId + 1;
            var newSelection = new Object[size];

            var index = 0;

            for (var i = minId; i <= maxId; i++)
            {
                newSelection[index] = displayEntries[i].GameObject;
                index++;
            }

            Selection.objects = newSelection;
        }

        private void AppendSelectedObject(GameObject gameObject)
        {
            var currentSelection = Selection.objects;
            var newSelection = new Object[currentSelection.Length + 1];

            currentSelection.CopyTo(newSelection, 0);
            newSelection[newSelection.Length - 1] = gameObject;

            Selection.objects = newSelection;
        }

        private void RemoveSelectedObject(GameObject gameObject)
        {
            var currentSelection = Selection.objects;
            var newSelection = new Object[currentSelection.Length - 1];

            var index = 0;

            for (var i = 0; i < currentSelection.Length; i++)
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

        private void UpdateHighlightedRenderers()
        {
            highlightedRenderers.Clear();

            if (highlightedObject == null)
            {
                return;
            }

            highlightedRenderers.AddRange(highlightedObject.GetComponentsInChildren<Renderer>(true));
        }

#if UNITY_2019_1_OR_NEWER
        private void OnSceneViewGui(UnityEditor.SceneView sceneView)
        {
            if (highlightedRenderers.Count == 0 ||
                Event.current.type != EventType.Repaint)
            {
                return;
            }

            // Unity 6.1+ provides Handles.DrawOutline which also highlights children in one call.
#if UNITY_6000_1_OR_NEWER
            Handles.DrawOutline(highlightedRenderers.ToArray(), highlightWireColor, highlightWireColor, 0.5f);
#else
            using (new Handles.DrawingScope(highlightWireColor))
            {
                foreach (var renderer in highlightedRenderers)
                {
                    if (renderer == null)
                    {
                        continue;
                    }

                    var bounds = renderer.bounds;
                    Handles.DrawWireCube(bounds.center, bounds.size);
                }
            }
#endif
        }
#endif

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

                UpdateHighlightedRenderers();
            }
        }

        private readonly struct DisplayEntry
        {
            internal DisplayEntry(GameObject gameObject, int depth)
            {
                GameObject = gameObject;
                Depth = depth;
            }

            internal GameObject GameObject { get; }
            internal int Depth { get; }
        }

        private static class Style
        {
            internal static readonly GUIStyle buttonStyle;

            static Style()
            {
                buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleLeft
                };
            }
        }
    }
}