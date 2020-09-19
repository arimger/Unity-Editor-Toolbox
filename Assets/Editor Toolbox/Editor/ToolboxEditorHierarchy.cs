using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;

    public enum HierarchyObjectDataItem
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

        /// <summary>
        /// Delegate used in label element drawing process.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        internal delegate Rect DrawHierarchyContentCallback(GameObject gameObject, Rect rect);


        static ToolboxEditorHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnItemCallback;
            EditorApplication.hierarchyWindowItemOnGUI += OnItemCallback;
        }


        /// <summary>
        /// Collection of all available content drawers associated to predefined data types.
        /// </summary>
        private static readonly Dictionary<HierarchyObjectDataItem, DrawHierarchyContentCallback>
            availableDrawContentCallbacks = new Dictionary<HierarchyObjectDataItem, DrawHierarchyContentCallback>(4)
            {
                { HierarchyObjectDataItem.Icon, DrawIcon },
                { HierarchyObjectDataItem.Toggle, DrawToggle },
                { HierarchyObjectDataItem.Tag, DrawTag },
                { HierarchyObjectDataItem.Layer, DrawLayer },
                { HierarchyObjectDataItem.Script, DrawScript },
            };

        /// <summary>
        /// Collection of all currently allowed hierarchy element drawers.
        /// </summary>
        private static readonly List<DrawHierarchyContentCallback>
            allowedDrawContentCallbacks = new List<DrawHierarchyContentCallback>()
            {
                DrawIcon,
                DrawToggle,
                DrawTag,
                DrawLayer,
                DrawScript
            };


        /// <summary>
        /// Index of current (last processed) GameObject within the Hierarchy Window.
        /// </summary>
        private static int currentItemIndex = 0;


        /// <summary>
        /// Tries to display item label in the Hierarchy Window.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="rect"></param>
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
                        DrawEmptyItemLabel(gameObject, rect, label, currentItemIndex);
                        break;
                    case LabelType.Header:
                        DrawHeaderItemLabel(gameObject, rect, label, currentItemIndex);
                        break;
                    case LabelType.Default:
                        DrawDefaultItemLabel(gameObject, rect, label, currentItemIndex);
                        break;
                }

                //increment index after drawing
                currentItemIndex++;
            }
        }


        /// <summary>
        /// Draws item in the completely raw way.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="rect"></param>
        /// <param name="label"></param>
        /// <param name="index"></param>
        private static void DrawEmptyItemLabel(GameObject gameObject, Rect rect, string label, int index = 0)
        { }

        /// <summary>
        /// Draws GameObject's as header. Creates separation lines and a proper background.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="rect"></param>
        /// <param name="label"></param>
        /// <param name="index"></param>
        private static void DrawHeaderItemLabel(GameObject gameObject, Rect rect, string label, int index = 0)
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
        /// <param name="gameObject"></param>
        /// <param name="rect"></param>
        /// <param name="label"></param>
        /// <param name="index"></param>
        private static void DrawDefaultItemLabel(GameObject gameObject, Rect rect, string label, int index = 0)
        {
            var contentRect = rect;
            var drawersCount = allowedDrawContentCallbacks.Count;

            EditorGUI.DrawRect(new Rect(contentRect.xMax, rect.y, Style.lineWidth, rect.height), Style.lineColor);

            //determine if there is anything to draw
            if (drawersCount > 0)
            {
                //draw first the callback element in proper rect
                //we have to adjust a given rect to our purpose
                contentRect = new Rect(contentRect.xMax - Style.maxWidth, rect.y, Style.maxWidth, rect.height);
                contentRect = allowedDrawContentCallbacks[0](gameObject, contentRect);

                EditorGUI.DrawRect(new Rect(contentRect.xMin, rect.y, Style.lineWidth, rect.height), Style.lineColor);

                //draw each needed element content stored in the callbacks collection
                for (var i = 1; i < drawersCount; i++)
                {
                    contentRect = new Rect(contentRect.xMin - Style.maxWidth, rect.y, Style.maxWidth, rect.height);
                    contentRect = allowedDrawContentCallbacks[i](gameObject, contentRect);

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


        private static Rect DrawIcon(GameObject gameObject, Rect rect)
        {
            var contentRect = rect;
            contentRect.width = Style.minWidth;
#if UNITY_2018_1_OR_NEWER
            contentRect.x = rect.xMax - contentRect.width;
#else
            contentRect.x = rect.xMax;
#endif
            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(contentRect, false, false, false, false);
            }

            //get specific icon for this gameObject
            var content = EditorGuiUtility.GetObjectContent(gameObject, typeof(GameObject));
            if (content.image)
            {
                //draw the specific icon 
                GUI.Label(contentRect, content.image);
            }

            return contentRect;
        }

        private static Rect DrawToggle(GameObject gameObject, Rect rect)
        {
            var contentRect = new Rect(rect.x + rect.width - Style.minWidth, rect.y, Style.minWidth, rect.height);

            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(contentRect, false, false, false, false);
            }

            var content = new GUIContent(string.Empty, "Enable/disable GameObject");
            //NOTE: using EditorGUI.Toggle will cause bug and deselect all hierarchy toggles when you will pick a multi-selected property in the Inspector
            var result = GUI.Toggle(new Rect(contentRect.x + Style.padding,
                                             contentRect.y,
                                             contentRect.width,
                                             contentRect.height),
                                             gameObject.activeSelf, content);

            if (contentRect.Contains(Event.current.mousePosition))
            {
                if (result != gameObject.activeSelf)
                {
                    Undo.RecordObject(gameObject, "SetActive");
                    gameObject.SetActive(result);
                }
            }

            return contentRect;
        }

        private static Rect DrawLayer(GameObject gameObject, Rect rect)
        {
            //adjust rect for the layer field
            var contentRect = new Rect(rect.x + rect.width - Style.minWidth, rect.y, Style.minWidth, rect.height);

            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(contentRect, false, false, false, false);
            }

            var layerMask = gameObject.layer;
            var layerName = LayerMask.LayerToName(layerMask);

            string GetContentText()
            {
                switch (layerMask)
                {
                    //keep the default layer as an empty label
                    case 00: return string.Empty;
                    //for UI elements we can use the full name
                    case 05: return layerName;
                    default: return layerMask.ToString();
                }
            }

            var content = new GUIContent(GetContentText(), layerName + " layer");

            //draw label for the gameObject's specific layer
            EditorGUI.LabelField(contentRect, content, Style.layerLabelStyle);
            return contentRect;
        }

        private static Rect DrawTag(GameObject gameObject, Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(rect, false, false, false, false);
            }

            //prepare content based on the GameObject's tag
            var content = new GUIContent(gameObject.CompareTag("Untagged") ? string.Empty : gameObject.tag, gameObject.tag);

            //draw related label field using prepared content
            EditorGUI.LabelField(rect, content, Style.normalLabelStyle);
            return rect;
        }

        private static Rect DrawScript(GameObject gameObject, Rect rect)
        {
            var contentRect = new Rect(rect.x + rect.width - Style.minWidth, rect.y, Style.minWidth, rect.height);

            var tooltip = string.Empty;
            var texture = Style.componentIcon;

            if (contentRect.Contains(Event.current.mousePosition))
            {
                var components = gameObject.GetComponents<Component>();
                if (components.Length > 1)
                {
                    texture = null;
                    tooltip = "Components:\n";
                    //set tooltip based on available components
                    for (var i = 1; i < components.Length; i++)
                    {
                        tooltip += "- " + components[i].GetType().Name;
                        tooltip += i == components.Length - 1 ? "" : "\n";
                    }

                    //create tooltip for the basic rect
                    GUI.Label(contentRect, new GUIContent(string.Empty, tooltip));

                    //adjust rect to all icons
                    contentRect.xMin -= Style.minWidth * (components.Length - 2);

                    //draw standard background
                    if (Event.current.type == EventType.Repaint)
                    {
                        Style.backgroundStyle.Draw(contentRect, false, false, false, false);
                    }

                    var iconRect = contentRect;
                    iconRect.xMin = contentRect.xMin;
                    iconRect.xMax = contentRect.xMin + Style.minWidth;

                    //iterate over available components
                    for (var i = 1; i < components.Length; i++)
                    {
                        var component = components[i];
                        var content = EditorGUIUtility.ObjectContent(component, component.GetType());

                        //draw icon for the current component
                        GUI.Label(iconRect, new GUIContent(content.image));
                        //adjust rect for the next icon
                        iconRect.x += Style.minWidth;
                    }

                    return contentRect;
                }
                else
                {
                    texture = Style.transformIcon;
                    tooltip = "There is no additional component";
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(contentRect, false, false, false, false);
            }

            GUI.Label(contentRect, new GUIContent(texture, tooltip));
            return contentRect;
        }


        /// <summary>
        /// Determines if the provided GameObject is first element inside hierarchy.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private static bool IsFirstGameObject(GameObject gameObject)
        {
            return gameObject.transform.parent == null && gameObject.transform.GetSiblingIndex() == 0;
        }

        /// <summary>
        /// Determines valid <see cref="LabelType"/> using GameObject's name property.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="label"></param>
        /// <returns></returns>
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
            GameObjectUtility.EnsureUniqueNameForSibling(headerGameObject);
            GameObjectUtility.SetParentAndAlign(headerGameObject, parentGameObject);

            //register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(headerGameObject, "Create a Header");

            //set proper selection
            Selection.activeObject = headerGameObject;
        }
        [Obsolete]
        private static void HandleHeaderObject(Editor editor)
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


        internal static void AppendAllowedHierarchyContentCallback(DrawHierarchyContentCallback callback)
        {
            allowedDrawContentCallbacks.Add(callback);
        }

        internal static void RemoveAllowedHierarchyContentCallback(DrawHierarchyContentCallback callback)
        {
            allowedDrawContentCallbacks.Remove(callback);
        }

        internal static void CreateAllowedHierarchyContentCallbacks(params HierarchyObjectDataItem[] items)
        {
            foreach (var item in items)
            {
                //validate current item and try to get associated drawer
                if (!availableDrawContentCallbacks.TryGetValue(item, out var drawer))
                {
                    continue;
                }

                //add drawer to the defined drawers collection
                AppendAllowedHierarchyContentCallback(drawer);
            }
        }

        internal static void RemoveAllowedHierarchyContentCallbacks()
        {
            allowedDrawContentCallbacks.Clear();
        }

        internal static void RepaintHierarchyOverlay() => EditorApplication.RepaintHierarchyWindow();


        /// <summary>
        /// Determines if <see cref="ToolboxEditorHierarchy"/> can create an additional overlay on the Hierarchy Window.
        /// </summary>
        internal static bool IsOverlayAllowed { get; set; } = false;

        internal static bool DrawHorizontalLines { get; set; } = true;
        internal static bool DrawSeparationLines { get; set; } = true;


        internal static class Style
        {
            internal static readonly float padding = 2.0f;
            internal static readonly float minWidth = 17.0f;
            internal static readonly float maxWidth = 60.0f;
            internal static readonly float minHeight = 16.0f;
            internal static readonly float maxHeight = 17.0f;
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
            internal static readonly GUIStyle normalLabelStyle;
            internal static readonly GUIStyle layerLabelStyle;
            internal static readonly GUIStyle remarkLabelStyle;
            internal static readonly GUIStyle headerLabelStyle;
            internal static readonly GUIStyle backgroundStyle;

            internal static readonly Texture componentIcon;
            internal static readonly Texture transformIcon;

            static Style()
            {
                normalLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 9
#else
                    fontSize = 8
#endif
                };

                layerLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 9,
#else
                    fontSize = 8,
#endif
#if UNITY_2019_3_OR_NEWER
                    alignment = TextAnchor.MiddleCenter
#else
                    alignment = TextAnchor.UpperCenter
#endif
                };

                remarkLabelStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);

                headerLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };

                backgroundStyle = new GUIStyle();
                backgroundStyle.normal.background = EditorGuiUtility.CreatePersistantTexture(labelColor);

                componentIcon = EditorGUIUtility.IconContent("cs Script Icon").image;
                transformIcon = EditorGUIUtility.IconContent("Transform Icon").image;
            }
        }
    }
}