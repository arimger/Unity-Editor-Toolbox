using UnityEngine;
using UnityEditor;

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
            DrawTag
        };


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
            contRect = new Rect(contRect.xMax - Style.maxWidth, rect.y, Style.maxWidth, contRect.height);
            contRect = DrawIcon(gameObject, contRect);
            foreach (var drawCallback in DrawElementCallbacks)
            {
                contRect =  new Rect(contRect.xMin - Style.maxWidth, rect.y, Style.maxWidth, contRect.height);
                contRect = drawCallback(gameObject, contRect);
            }

            DrawLine(rect);
        }

        private static Rect DrawEmpty(GameObject gameObject, Rect rect)
        {
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.y + Style.lineOffset / 2, Style.lineWidth, rect.height - Style.lineOffset), Color.grey);
            EditorGUI.DrawRect(new Rect(rect.xMax, rect.y + Style.lineOffset / 2, Style.lineWidth, rect.height - Style.lineOffset), Color.grey);
            return rect;
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

        private static Rect DrawTag(GameObject gameObject, Rect rect)
        {
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.y + Style.lineOffset / 2, Style.lineWidth, rect.height - Style.lineOffset), Style.lineColor);
            EditorGUI.DrawRect(new Rect(rect.xMax, rect.y + Style.lineOffset / 2, Style.lineWidth, rect.height - Style.lineOffset), Style.lineColor);

            var content = new GUIContent(gameObject.tag);

            if (content.text != ToolboxEditorUtility.defaultUnityTag)
            {
                GUI.Label(rect, content, Style.labelStyle);
            }

            return rect;
        }

        private static void DrawLine(Rect rect)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height, rect.width, Style.lineWidth), Style.lineColor);
        }


        /// <summary>
        /// Static representation of custom hierarchy style.
        /// </summary>
        internal static class Style
        {
            internal static float maxWidth = 55.0f;
            internal static float lineWidth = 1.0f;
            internal static float lineOffset = 2.0f;
            internal static float iconWidth = 18.0f;
            internal static float iconHeight = 18.0f;

            internal static GUIStyle labelStyle;

            internal static Color lineColor = Color.grey;


            static Style()
            {
                labelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    fontSize = 8                   
                };
                labelStyle.normal.textColor = new Color(0.35f, 0.35f, 0.35f);
            }
        }
    }
}