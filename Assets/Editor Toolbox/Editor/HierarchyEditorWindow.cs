using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    public static class HierarchyEditorWindow
    {
        static HierarchyEditorWindow()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnItemCallback;
        }

        
        /// <summary>
        /// Delegate used in label element drawing.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private delegate Rect DrawElementCallback(GameObject gameObject, Rect rect);

        /// <summary>
        /// Collection of all wanted hierarchy elements drawers.
        /// </summary>
        private static readonly DrawElementCallback[] DrawElementCallbacks = new DrawElementCallback[]
        {       
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
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject)
            {
                DrawItemLabel(gameObject, rect);
            }
        }

        /// <summary>
        /// Draws label as whole. Creates separation line, standard icon and additional elements stored in collection.
        /// Iterates over all neeeded draw callbacks and creates elements in horizontal group.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="rect"></param>
        private static void DrawItemLabel(GameObject gameObject, Rect rect)
        {
            var contRect = rect;
            //draw game object's specific icon
            contRect = new Rect(contRect.xMax - Style.maxWidth, rect.y, Style.maxWidth, contRect.height);
            contRect = DrawIcon(gameObject, contRect);

            //draw vertical separation line
            EditorGUI.DrawRect(new Rect(contRect.xMin, contRect.y + Style.lineOffset / 2, Style.lineWidth,
                contRect.height - Style.lineOffset), Style.lineColor);
            //draw each needed element content stored in callbacks collection
            foreach (var drawCallback in DrawElementCallbacks)
            {
                contRect =  new Rect(contRect.xMin - Style.maxWidth, rect.y, Style.maxWidth, contRect.height);
                contRect = drawCallback(gameObject, contRect);
                //draw vertical separation line
                EditorGUI.DrawRect(new Rect(contRect.xMin, contRect.y + Style.lineOffset / 2, Style.lineWidth, 
                    contRect.height - Style.lineOffset), Style.lineColor);
            }
            //draw horizontal separation line
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height, rect.width, Style.lineWidth), Style.lineColor);
        }


        private static Rect DrawIcon(GameObject gameObject, Rect rect)
        {
            var content = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject));
            var contentIcon = content.image;
            var contentRect = rect;

            contentRect.x = rect.xMax;
            contentRect.width = Style.iconWidth;
            contentRect.height = Style.iconHeight;

            if (contentIcon.name != ToolboxEditorUtility.defaultIconName)
            {
                GUI.Label(contentRect, contentIcon);
            }

            return contentRect;
        }

        private static Rect DrawLayer(GameObject gameObject, Rect rect)
        {
            rect = new Rect(rect.x + rect.width - Style.layerWidth, rect.y, Style.layerWidth, rect.height);

            GUI.Label(rect, new GUIContent(gameObject.layer.ToString()), Style.layerLabelStyle);

            return rect;
        }

        private static Rect DrawTag(GameObject gameObject, Rect rect)
        {
            var content = new GUIContent(gameObject.tag);

            if (content.text != ToolboxEditorUtility.defaultUnityTag)
            {
                GUI.Label(rect, content, Style.tagLabelStyle);
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

            internal static float maxWidth = 55.0f;
            internal static float lineWidth = 1.0f;
            internal static float lineOffset = 2.0f;
            internal static float iconWidth = 18.0f;
            internal static float iconHeight = 18.0f;
            internal static float layerWidth = 17.5f;

            internal static Color lineColor = Color.grey;

            /// 
            /// Custom label styles
            /// 

            internal static GUIStyle tagLabelStyle;
            internal static GUIStyle layerLabelStyle;

            static Style()
            {
                tagLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    fontSize = 8                   
                };
                tagLabelStyle.normal.textColor = new Color(0.35f, 0.35f, 0.35f);

                layerLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    fontSize = 8,
                    alignment = TextAnchor.UpperCenter
                };
            }
        }
    }
}