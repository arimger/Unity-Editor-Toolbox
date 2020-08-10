using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    public enum HierarchyObjectDataItem
    {
        Icon,
        Toggle,
        Tag,
        Layer,
        Script
    }

    /// <summary>
    /// Static GUI representation for the Hierarchy Overlay. It is directly managed by the <see cref="ToolboxHierarchyUtility"/>.
    /// </summary>
    [InitializeOnLoad]
    public static class ToolboxEditorHierarchy
    {
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
        /// Collection of all available content drawers associated to particular data type.
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
        /// Collection of all wanted hierarchy elements drawers.
        /// </summary>
        private static readonly List<DrawHierarchyContentCallback>
            allowedDrawContentCallbacks = new List<DrawHierarchyContentCallback>(4)
            {
                DrawIcon,
                DrawToggle,
                DrawTag,
                DrawLayer,
                DrawScript
            };


        internal static void RepaintHierarchyOverlay() => EditorApplication.RepaintHierarchyWindow();

        internal static void AddDrawHierarchyContentCallback(DrawHierarchyContentCallback callback)
        {
            allowedDrawContentCallbacks.Add(callback);
        }

        internal static void RemoveDrawHierarchyContentCallback(DrawHierarchyContentCallback callback)
        {
            allowedDrawContentCallbacks.Remove(callback);
        }

        internal static void RemoveAllDrawHierarchyContentCallbacks(Predicate<DrawHierarchyContentCallback> predicate)
        {
            allowedDrawContentCallbacks.RemoveAll(predicate);
        }


        /// <summary>
        /// Tries to display item label in the hierarchy window.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="rect"></param>
        private static void OnItemCallback(int instanceId, Rect rect)
        {
            if (!ToolboxHierarchyUtility.ToolboxHierarchyAllowed)
            {
                return;
            }

            //use Unity's internal method to determinate the proper GameObject instance
            var gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (gameObject)
            {
                //NOTE: the prime item can be used to draw single options for the whole hierarchy
                if (IsPrimeGameObject(gameObject))
                {                    
                    //pick all choosen items directly from the settings utility
                    PrepareDrawCallbacks();                    
                }

                var name = gameObject.name;
                if (name.StartsWith("#") && name.Length > 1)
                {
                    var label = name.Remove(0, 2);
                    var prefix = name[1];

                    switch (prefix)
                    {
                        case 'h':
                            DrawHeaderItemLabel(gameObject, rect, label);
                            break;
                        default:
                            DrawDefaultItemLabel(gameObject, rect, label);
                            break;
                    }
                    
                    return;
                }
                //draw the current object in default way
                DrawDefaultItemLabel(gameObject, rect, name);
            }
        }

        /// <summary>
        /// Prepares allowed drawers collection based on the settings file.
        /// </summary>
        private static void PrepareDrawCallbacks()
        {
            if (!ToolboxHierarchyUtility.AreRowDataItemsUpdated)
            {
                return;
            }

            allowedDrawContentCallbacks.Clear();

            var rowDataItems = ToolboxHierarchyUtility.GetRowDataItems();
            foreach (var item in rowDataItems)
            {
                //validate current item and try to get associated drawer
                if (!availableDrawContentCallbacks.TryGetValue(item, out var drawer))
                {
                    continue;
                }

                //add drawer to the allowed drawers collection
                allowedDrawContentCallbacks.Add(drawer);
            }
        }

        /// <summary>
        /// Draws GameObject's as header. Creates separation lines and a proper background.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="rect"></param>
        /// <param name="label"></param>
        private static void DrawHeaderItemLabel(GameObject gameObject, Rect rect, string label)
        {
            //repaint background on proper event
            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(rect, false, false, false, false);
            }

            EditorGUI.DrawRect(new Rect(rect.xMax, rect.y, Style.lineWidth, rect.height), Style.lineColor);
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.y, Style.lineWidth, rect.height), Style.lineColor);

            //prepare content for the associated label
            var content = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject));
            if (content.image)
            {
                var iconName = content.image.name;
                if (iconName == ToolboxEditorUtility.defaultObjectIconName ||
                    iconName == ToolboxEditorUtility.defaultPrefabIconName)
                {
                    content.image = null;
                }
            }
            content.text = label;
            content.tooltip = string.Empty;

            //draw custom label field for the provided GameObject
            EditorGUI.LabelField(rect, content, Style.headerLabelStyle);

            EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - Style.lineWidth, rect.width, Style.lineWidth), Style.lineColor);
        }

        /// <summary>
        /// Draws label as whole. Creates separation lines, associated icons and
        /// additional elements stored in the <see cref="allowedDrawContentCallbacks"/> collection.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="rect"></param>
        private static void DrawDefaultItemLabel(GameObject gameObject, Rect rect, string label)
        {
            var contentRect = rect;
            var drawersCount = allowedDrawContentCallbacks.Count;

            EditorGUI.DrawRect(new Rect(contentRect.xMax, rect.y, Style.lineWidth, rect.height), Style.lineColor);

            //determine if there is anything to draw
            if (drawersCount > 0)
            {
                //draw first the callback element in proper rect
                //we have to adjust a given rect to our purpose
                contentRect = new Rect(contentRect.xMax - Style.maxWidth, rect.y, Style.maxWidth, contentRect.height);
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

            EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - Style.lineWidth, rect.width, Style.lineWidth), Style.lineColor);

            contentRect.xMax = contentRect.xMin;
            contentRect.xMin = rect.xMin;

            EditorGUI.LabelField(contentRect, new GUIContent(string.Empty, label));
        }


        private static Rect DrawIcon(GameObject gameObject, Rect rect)
        {
            //get specific icon for this gameObject
            var content = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject));
            var contentIcon = content.image;
            var contentRect = rect;

            contentRect.width = Style.iconWidth;
            //contentRect.height = Style.iconHeight;
#if UNITY_2018_1_OR_NEWER
            contentRect.x = rect.xMax - contentRect.width;
#else
            contentRect.x = rect.xMax;
#endif
            //draw hierarchy background
            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(contentRect, false, false, false, false);
            }
            //draw specific icon 
            if (contentIcon.name != ToolboxEditorUtility.defaultObjectIconName)
            {
                GUI.Label(contentRect, contentIcon);
            }

            return contentRect;
        }

        private static Rect DrawToggle(GameObject gameObject, Rect rect)
        {
            rect = new Rect(rect.x + rect.width - Style.toggleWidth, rect.y, Style.toggleWidth, rect.height);

            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(rect, false, false, false, false);
            }

            var content = new GUIContent(string.Empty, "Enable/disable GameObject");
            var result = GUI.Toggle(new Rect(rect.x + Style.padding, rect.y, rect.width, rect.height), gameObject.activeSelf, content);
            //NOTE: using EditorGUI.Toggle will cause bug and deselect all hierarchy toggles when you will pick multi-selected property in inspector
            if (rect.Contains(Event.current.mousePosition))
            {
                if (result != gameObject.activeSelf)
                {
                    Undo.RecordObject(gameObject, "SetActive");
                    gameObject.SetActive(result);
                }
            }

            return rect;
        }

        private static Rect DrawLayer(GameObject gameObject, Rect rect)
        {
            //adjust rect for layer field
            var contentRect = new Rect(rect.x + rect.width - Style.layerWidth, rect.y, Style.layerWidth, rect.height);
            var objectLayer = gameObject.layer;
            var contentText = new GUIContent(objectLayer.ToString(), LayerMask.LayerToName(objectLayer) + " layer");

            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(contentRect, false, false, false, false);
            }
            //draw label for gameObject's specific layer
            EditorGUI.LabelField(contentRect, contentText, Style.layerLabelStyle);

            return contentRect;
        }

        private static Rect DrawTag(GameObject gameObject, Rect rect)
        {
            const string defaultUnityTag = "Untagged";

            var content = new GUIContent(gameObject.tag, gameObject.tag);

            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(rect, false, false, false, false);
            }

            if (!gameObject.CompareTag(defaultUnityTag))
            {
                EditorGUI.LabelField(rect, content, Style.tagLabelStyle);
            }

            return rect;
        }

        private static Rect DrawScript(GameObject gameObject, Rect rect)
        {
            rect = new Rect(rect.x + rect.width - Style.iconWidth, rect.y, Style.iconWidth, rect.height);
                 
            var tooltip = string.Empty;
            var texture = Style.componentTexture;

            if (rect.Contains(Event.current.mousePosition))
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
                    GUI.Label(rect, new GUIContent(string.Empty, tooltip));

                    //adjust rect to all icons
                    rect.xMin -= Style.iconWidth * (components.Length - 2);

                    //draw standard background
                    if (Event.current.type == EventType.Repaint)
                    {
                        Style.backgroundStyle.Draw(rect, false, false, false, false);
                    }

                    var iconRect = rect;
                    iconRect.xMin = rect.xMin;
                    iconRect.xMax = rect.xMin + Style.iconWidth;

                    //iterate over available components
                    for (var i = 1; i < components.Length; i++)
                    {
                        var component = components[i];
                        var content = EditorGUIUtility.ObjectContent(component, component.GetType());

                        //draw icon for the current component
                        GUI.Label(iconRect, new GUIContent(content.image));
                        //adjust rect for the next icon
                        iconRect.x += Style.iconWidth;
                    }

                    return rect;
                }
                else
                {
                    texture = Style.transformTexture;
                    tooltip = "There is no additional component";
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(rect, false, false, false, false);
            }

            GUI.Label(rect, new GUIContent(texture, tooltip));
            return rect;
        }


        /// <summary>
        /// Determines if the provided GameObject is first element inside hierarchy.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private static bool IsPrimeGameObject(GameObject gameObject)
        {
            return gameObject.transform.parent == null && gameObject.transform.GetSiblingIndex() == 0;
        }


        [MenuItem("GameObject/Editor Toolbox/Hierarchy Header", false, 10)]
        private static void CreateHeaderObject(MenuCommand menuCommand)
        {
            const string key = "#h";
            const string name = "Header";
            const string tag = "EditorOnly";

            var gameObject = new GameObject();
            //hide the obsolete transform component
            gameObject.transform.hideFlags = HideFlags.HideInInspector;
            //set proper essentials
            gameObject.name = key + name;
            gameObject.tag = tag;
            gameObject.layer = 0;
            gameObject.isStatic = true;
            
            //ensure it gets reparented if this was a context click(otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);
            //register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
            //set proper selection
            Selection.activeObject = gameObject;
        }


        /// <summary>
        /// Static representation of custom hierarchy style.
        /// </summary>
        internal static class Style
        {
            internal static readonly float padding = 2.0f;
            internal static readonly float maxHeight = 16.0f;
            internal static readonly float maxWidth = 55.0f;
            internal static readonly float lineWidth = 1.0f;
            internal static readonly float lineOffset = 2.0f;
            internal static readonly float iconWidth = 17.0f;
            internal static readonly float iconHeight = 17.0f;
            internal static readonly float layerWidth = 17.0f;
            internal static readonly float toggleWidth = 17.0f;

            internal static readonly Color textColor = new Color(0.35f, 0.35f, 0.35f);
            internal static readonly Color lineColor = new Color(0.59f, 0.59f, 0.59f);
#if UNITY_2019_3_OR_NEWER
            internal static readonly Color labelColor = EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f) : new Color(0.909f, 0.909f, 0.909f);
#else
            internal static readonly Color labelColor = EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f) : new Color(0.855f, 0.855f, 0.855f);
#endif

            internal static readonly GUIStyle toggleStyle;
            internal static readonly GUIStyle tagLabelStyle;
            internal static readonly GUIStyle layerLabelStyle;
            internal static readonly GUIStyle backgroundStyle;
            internal static readonly GUIStyle headerLabelStyle;

            internal static readonly Texture componentTexture;
            internal static readonly Texture transformTexture;

            static Style()
            {
                tagLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    fontSize = 8
                };
                tagLabelStyle.normal.textColor = textColor;

                layerLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    fontSize = 8,
#if UNITY_2019_3_OR_NEWER
                    alignment = TextAnchor.MiddleCenter
#else
                    alignment = TextAnchor.UpperCenter
#endif
                };

                toggleStyle = new GUIStyle(EditorStyles.toggle);

                backgroundStyle = new GUIStyle();
                backgroundStyle.normal.background = AssetUtility.GetPersistentTexture(labelColor);

                headerLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };

                componentTexture = EditorGUIUtility.IconContent("cs Script Icon").image;
                transformTexture = EditorGUIUtility.IconContent("Transform Icon").image;
            }
        }
    }
}