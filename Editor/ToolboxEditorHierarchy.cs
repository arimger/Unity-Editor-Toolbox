using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    public static class ToolboxEditorHierarchy
    {
        static ToolboxEditorHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnItemCallback;
        }

        
        /// <summary>
        /// Delegate used in label element drawing process.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private delegate Rect DrawElementCallback(GameObject gameObject, Rect rect);


        /// <summary>
        /// Collection of all wanted hierarchy elements drawers.
        /// </summary>
        private static readonly DrawElementCallback[] drawElementCallbacks = new DrawElementCallback[]
        {       
            DrawIcon,
            DrawToggle,
            DrawTag,
            DrawLayer
        };


        /// <summary>
        /// Tries to display item label in hierarchy window.
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="rect"></param>
        private static void OnItemCallback(int instanceID, Rect rect)
        {
            if (!ToolboxSettingsUtility.ToolboxHierarchyAllowed) return;

            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject)
            {
                DrawItemLabel(gameObject, rect);
            }
        }

        /// <summary>
        /// Draws label as whole. Creates separation lines, standard icon and
        /// additional elements stored in <see cref="drawElementCallbacks"/> collection.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="rect"></param>
        private static void DrawItemLabel(GameObject gameObject, Rect rect)
        {
            //validate callbacks count
            if (drawElementCallbacks.Length == 0)
            {
                return;
            }
            
            var contRect = rect;

            //draw vertical separation line
            EditorGUI.DrawRect(new Rect(contRect.xMax, rect.y, Style.lineWidth, rect.height), Style.lineColor);

            //draw first callback element in proper rect
            //we have to adjust given rect to our purpose
            contRect = new Rect(contRect.xMax - Style.maxWidth, rect.y, Style.maxWidth, contRect.height);
            contRect = drawElementCallbacks[0](gameObject, contRect);
  
            //draw vertical separation line
            EditorGUI.DrawRect(new Rect(contRect.xMin, rect.y, Style.lineWidth, rect.height), Style.lineColor);

            //draw each needed element content stored in callbacks collection
            for (var i = 1; i < drawElementCallbacks.Length; i++)
            {
                contRect =  new Rect(contRect.xMin - Style.maxWidth, rect.y, Style.maxWidth, rect.height);
                contRect = drawElementCallbacks[i](gameObject, contRect);

                //draw vertical separation line
                EditorGUI.DrawRect(new Rect(contRect.xMin, rect.y, Style.lineWidth, rect.height), Style.lineColor);
            }

            //draw horizontal separation line
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - Style.lineWidth, rect.width, Style.lineWidth), Style.lineColor);
        }


        /// 
        /// Item drawers implementations
        /// 

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
            if (contentIcon.name != Utility.defaultIconName)
            {
                GUI.Label(contentRect, contentIcon);
            }

            return contentRect;
        }

        private static Rect DrawLayer(GameObject gameObject, Rect rect)
        {
            //adjust rect for layer field
            var contentRect = new Rect(rect.x + rect.width - Style.layerWidth, rect.y, Style.layerWidth, rect.height);

            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(contentRect, false, false, false, false);
            }
            //draw label for gameObject's specific layer
            EditorGUI.LabelField(contentRect, new GUIContent(gameObject.layer.ToString()), Style.layerLabelStyle);

            return contentRect;
        }

        private static Rect DrawTag(GameObject gameObject, Rect rect)
        {
            var content = new GUIContent(gameObject.tag);

            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(rect, false, false, false, false);
            }

            if (content.text != Utility.defaultUnityTag)
            {
                EditorGUI.LabelField(rect, content, Style.tagLabelStyle);
            }

            return rect;
        }

        private static Rect DrawToggle(GameObject gameObject, Rect rect)
        {
            rect = new Rect(rect.x + rect.width - Style.toggleWidth, rect.y, Style.toggleWidth, rect.height);

            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(rect, false, false, false, false);
            }

            var value = GUI.Toggle(new Rect(rect.x + Style.padding, rect.y, rect.width, rect.height), gameObject.activeSelf, GUIContent.none);
            //NOTE: using EditorGUI.Toggle will cause bug and deselect all hierarchy toggles when will you pick multiselected property in inspector
            if (rect.Contains(Event.current.mousePosition))
            {
                if (value != gameObject.activeSelf)
                {
                    Undo.RecordObject(gameObject, "SetActive");
                    gameObject.SetActive(value);
                }
            }

            return rect;
        }


        /// <summary>
        /// Static representation of custom hierarchy style.
        /// </summary>
        internal static class Style
        {
            /// 
            /// Custom rect handling fields
            /// 

            internal static readonly float padding = 2.0f;
            internal static readonly float maxHeight = 16.0f;
            internal static readonly float maxWidth = 55.0f;
            internal static readonly float lineWidth = 1.0f;
            internal static readonly float lineOffset = 2.0f;
            internal static readonly float iconWidth = 17.0f;
            internal static readonly float iconHeight = 17.0f;
            internal static readonly float layerWidth = 17.0f;
            internal static readonly float toggleWidth = 17.0f;

            internal static readonly Color lineColor = new Color(0.59f, 0.59f, 0.59f);
            internal static readonly Color labelColor = EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f) : new Color(0.855f, 0.855f, 0.855f);

            /// 
            /// Custom label styles
            ///
 
            internal static readonly GUIStyle toggleStyle;
            internal static readonly GUIStyle tagLabelStyle;
            internal static readonly GUIStyle layerLabelStyle;
            internal static readonly GUIStyle backgroundStyle;
       
            static Style()
            {
                //set tag label style based on mini label
                tagLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    fontSize = 8                   
                };
                tagLabelStyle.normal.textColor = new Color(0.35f, 0.35f, 0.35f);

                //set layer label style based on mini label
                layerLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    fontSize = 8,
                    alignment = TextAnchor.UpperCenter
                };

                toggleStyle = new GUIStyle(EditorStyles.toggle);

                //set proper background texture object
                var backgroundTex = new Texture2D(1, 1);
                backgroundTex.SetPixel(0, 0, labelColor);
                backgroundTex.Apply();
                backgroundTex.hideFlags = HideFlags.HideAndDontSave;

                //set background style based on custom background texture
                backgroundStyle = new GUIStyle();
                backgroundStyle.normal.background = backgroundTex;
            }
        }

        /// <summary>
        /// Custom utility class.
        /// </summary>
        internal static class Utility
        {
            internal const string defaultUnityTag = "Untagged";

            internal const string defaultIconName = "GameObject Icon";
        }
    }
}