using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Hierarchy;

    public enum HierarchyObjectDataType
    {
        Icon,
        Toggle,
        Tag,
        Layer,
        Script
    }

    /// <summary>
    /// Static GUI representation for the Hierarchy Overlay.
    /// </summary>
    [InitializeOnLoad]
    public static class ToolboxEditorHierarchy
    {
        /// <summary>
        /// Possible types of the GameObject label.
        /// </summary>
        internal enum LabelType
        {
            Empty,
            Header,
            Default
        }


        static ToolboxEditorHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnItemCallback;
            EditorApplication.hierarchyWindowItemOnGUI += OnItemCallback;
        }


        /// <summary>
        /// 
        /// </summary>
        private static readonly List<HierarchyDataDrawer> allowedDrawers = new List<HierarchyDataDrawer>();

        /// <summary>
        /// Index of current (last processed) GameObject within the Hierarchy Window.
        /// </summary>
        private static int currentItemIndex = 0;


        /// <summary>
        /// Tries to display item label in the Hierarchy Window.
        /// </summary>
        private static void OnItemCallback(int instanceId, Rect rect)
        {
            if (!IsOverlayAllowed)
            {
                return;
            }

            //use Unity's internal method to determinate the proper GameObject instance
            var gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (gameObject)
            {
                //NOTE: the first item can be used to draw single options for the whole hierarchy
                if (IsFirstGameObject(gameObject))
                {
                    //reset items index
                    currentItemIndex = 0;
                }

                var type = GetLabelType(gameObject, out var label);
                //draw label using one of the possible forms
                switch (type)
                {
                    case LabelType.Empty:
                        DrawEmptyItemLabel(rect, gameObject, label, currentItemIndex);
                        break;
                    case LabelType.Header:
                        DrawHeaderItemLabel(rect, gameObject, label, currentItemIndex);
                        break;
                    case LabelType.Default:
                        DrawDefaultItemLabel(rect, gameObject, label, currentItemIndex);
                        break;
                }

                //increment index after drawing
                currentItemIndex++;
            }
            else
            {
                DrawSceneHeaderLabel(rect);
            }
        }


        /// <summary>
        /// Creates optional information about selected objects using the <see cref="Selection"/> class.
        /// </summary>
        private static void DrawSceneHeaderLabel(Rect rect)
        {
            if (!ShowSelectionsCount)
            {
                return;
            }

            //validate selected objects
            var count = Selection.gameObjects?.Length ?? 0;
            if (count == 0)
            {
                return;
            }

            //draw dedicated label field
            EditorGUI.LabelField(rect, count.ToString(), Style.selectLabelStyle);
        }

        /// <summary>
        /// Draws item in the completely raw way.
        /// </summary>
        private static void DrawEmptyItemLabel(Rect rect, GameObject gameObject, string label, int index = 0)
        {
            //just keep internal label
        }

        /// <summary>
        /// Draws GameObject's as header. Creates separation lines and a proper background.
        /// </summary>
        private static void DrawHeaderItemLabel(Rect rect, GameObject gameObject, string label, int index = 0)
        {
            //repaint background on proper event
            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(rect, false, false, false, false);
            }

            if (index > 0)
            {
                EditorGUI.DrawRect(new Rect(rect.x, rect.y - Style.lineWidth, rect.width, Style.lineWidth), Style.lineColor);
            }

            EditorGUI.DrawRect(new Rect(rect.xMax, rect.y, Style.lineWidth, rect.height), Style.lineColor);
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.y, Style.lineWidth, rect.height), Style.lineColor);

            //try to retrive a content for the provided GameObject
            var iconContent = EditorGuiUtility.GetObjectContent(gameObject, typeof(GameObject));
            //prepare content for the associated (fixed) label
            var itemContent = new GUIContent(label, iconContent.image);

            //draw a custom label field for the provided GameObject
            EditorGUI.LabelField(rect, itemContent, Style.headerLabelStyle);

            EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - Style.lineWidth, rect.width, Style.lineWidth), Style.lineColor);
        }

        /// <summary>
        /// Creates separation lines and content based on the <see cref="allowedDrawContentCallbacks"/> collection.
        /// </summary>
        private static void DrawDefaultItemLabel(Rect rect, GameObject gameObject, string label, int index = 0)
        {
            var contentRect = rect;
            var drawersCount = allowedDrawers.Count;

            //determine if there is anything to draw
            if (drawersCount > 0)
            {
                var availableRect = contentRect;
                EditorGUI.DrawRect(new Rect(contentRect.xMax, rect.y, Style.lineWidth, rect.height), Style.lineColor);

                //draw first the drawer element in a proper rect
                //we have to adjust a given rect to our purpose
                contentRect = ApplyDataDrawer(allowedDrawers[0], gameObject, availableRect);
                availableRect.xMax -= contentRect.width;

                EditorGUI.DrawRect(new Rect(contentRect.xMin, rect.y, Style.lineWidth, rect.height), Style.lineColor);

                //draw each needed element content stored in the drawers collection
                for (var i = 1; i < drawersCount; i++)
                {
                    contentRect = ApplyDataDrawer(allowedDrawers[i], gameObject, availableRect);
                    availableRect.xMax -= contentRect.width;

                    EditorGUI.DrawRect(new Rect(contentRect.xMin, rect.y, Style.lineWidth, rect.height), Style.lineColor);
                }
            }

            //finally adjust available rect to the created content
            if (contentRect.xMin < rect.xMin)
            {
                rect.xMin = contentRect.xMin;
            }

            //draw a horiozntal line but only if it is expected
            if (DrawHorizontalLines)
            {
                EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - Style.lineWidth, rect.width, Style.lineWidth), Style.lineColor);
            }

            contentRect.xMax = contentRect.xMin;
            contentRect.xMin = rect.xMin;

            //create an empty label field which will serve as a tooltip
            EditorGUI.LabelField(contentRect, new GUIContent(string.Empty, label));
        }


        /// <summary>
        /// 
        /// </summary>
        private static Rect ApplyDataDrawer(HierarchyDataDrawer dataDrawer, GameObject target, Rect availableRect)
        {
            dataDrawer.Prepare(target, availableRect);
            var rect = availableRect;
            rect.xMin = rect.xMax - dataDrawer.GetWidth();

            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(rect, false, false, false, false);
            }

            dataDrawer.OnGui(rect);
            return rect;
        }


        /// <summary>
        /// Determines if the provided GameObject is first element inside hierarchy.
        /// </summary>
        private static bool IsFirstGameObject(GameObject gameObject)
        {
            return gameObject.transform.parent == null && gameObject.transform.GetSiblingIndex() == 0;
        }

        /// <summary>
        /// Determines valid <see cref="LabelType"/> using GameObject's name property.
        /// </summary>
        private static LabelType GetLabelType(GameObject gameObject, out string label)
        {
            var type = LabelType.Default;
            var name = gameObject.name;
            if (name.StartsWith("#") && name.Length > 1)
            {
                //try to find an associated command
                switch (name[1])
                {
                    case 'e':
                        type = LabelType.Empty;
                        break;
                    case 'h':
                        type = LabelType.Header;
                        break;
                }
                //remove unnecessary part of the name
                name = name.Remove(0, 2);
            }

            label = name;
            //return last cached type
            return type;
        }


        [MenuItem("GameObject/Editor Toolbox/Hierarchy Header", false, 10)]
        private static void CreateHeaderObject(MenuCommand menuCommand)
        {
            var parentGameObject = menuCommand.context as GameObject;
            var headerGameObject = new GameObject();

            //hide the redundant transform component
            headerGameObject.transform.hideFlags = HideFlags.HideInInspector;
            //set proper essentials
            headerGameObject.name = "#hHeader";
            headerGameObject.layer = 0;
            headerGameObject.tag = "EditorOnly";
            headerGameObject.isStatic = false;

            //ensure it gets reparented if this was a context click(otherwise does nothing) and fix name
            GameObjectUtility.SetParentAndAlign(headerGameObject, parentGameObject);

            //register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(headerGameObject, "Create a Header");

            //set proper selection
            Selection.activeObject = headerGameObject;
        }
        [Obsolete]
        private static void HandleHeaderObject(UnityEditor.Editor editor)
        {
            if (editor.targets.Length > 1)
            {
                return;
            }

            if (editor.target.name.StartsWith("#h"))
            {
                var target = editor.target as GameObject;
                EditorGUILayout.LabelField("Hierachy Header Object", Style.remarkLabelStyle);
                editor.serializedObject.Update();
                target.tag = "EditorOnly";
                editor.serializedObject.ApplyModifiedProperties();
            }
        }
        [Obsolete]
        private static void HandleHeaderObject(GameObject gameObject)
        {
            throw new NotImplementedException();
        }


        internal static void CreateAllowedHierarchyContentCallbacks(params HierarchyObjectDataType[] items)
        {
            foreach (var item in items)
            {
                HierarchyDataDrawer dataDrawer = null;
                switch (item)
                {
                    case HierarchyObjectDataType.Icon:
                        dataDrawer = new HierarchyIconDrawer();
                        break;
                    case HierarchyObjectDataType.Toggle:
                        dataDrawer = new HierarchyToggleDrawer();
                        break;
                    case HierarchyObjectDataType.Tag:
                        dataDrawer = new HierarchyTagDrawer();
                        break;
                    case HierarchyObjectDataType.Layer:
                        dataDrawer = new HierarchyLayerDrawer();
                        break;
                    case HierarchyObjectDataType.Script:
                        dataDrawer = new HierarchyScriptDrawer();
                        break;
                }

                allowedDrawers.Add(dataDrawer);
            }
        }

        internal static void RemoveAllowedHierarchyContentCallbacks()
        {
            allowedDrawers.Clear();
        }

        internal static void RepaintHierarchyOverlay() => EditorApplication.RepaintHierarchyWindow();


        /// <summary>
        /// Determines if <see cref="ToolboxEditorHierarchy"/> can create an additional overlay on the Hierarchy Window.
        /// </summary>
        internal static bool IsOverlayAllowed { get; set; } = false;

        internal static bool DrawHorizontalLines { get; set; } = true;
        internal static bool ShowSelectionsCount { get; set; } = true;
        internal static bool DrawSeparationLines { get; set; } = true;


        internal static class Style
        {
            internal static readonly float lineWidth = 1.0f;
#if UNITY_2019_3_OR_NEWER
            internal static readonly Color lineColor = EditorGUIUtility.isProSkin ? new Color(0.15f, 0.15f, 0.15f) : new Color(0.69f, 0.69f, 0.69f);
#else
            internal static readonly Color lineColor = EditorGUIUtility.isProSkin ? new Color(0.15f, 0.15f, 0.15f) : new Color(0.59f, 0.59f, 0.59f);
#endif
#if UNITY_2019_3_OR_NEWER
            internal static readonly Color labelColor = EditorGUIUtility.isProSkin ? new Color(0.31f, 0.31f, 0.31f) : new Color(0.909f, 0.909f, 0.909f);
#else
            internal static readonly Color labelColor = EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f) : new Color(0.855f, 0.855f, 0.855f);
#endif
            internal static readonly GUIStyle headerLabelStyle;
            internal static readonly GUIStyle remarkLabelStyle;
            internal static readonly GUIStyle selectLabelStyle;
            internal static readonly GUIStyle backgroundStyle;

            static Style()
            {
                headerLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };
                remarkLabelStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                selectLabelStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                {
                    alignment = TextAnchor.MiddleRight,
                    contentOffset = new Vector2()
                    {
                        x = -2.0f
                    }
                };

                backgroundStyle = new GUIStyle();
                backgroundStyle.normal.background = EditorGuiUtility.CreateColorTexture(labelColor);
            }
        }
    }
}