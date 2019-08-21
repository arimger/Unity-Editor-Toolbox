using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    public class HierarchyEditorWindow
    {
        static HierarchyEditorWindow()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
        }


        private static void HierarchyItemCB(int instanceID, Rect rect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject)
            {
                //TODO: draw in loop
                rect = new Rect(rect.xMax - Style.maxWidth, rect.y, Style.maxWidth, rect.height);
                rect = DrawIcon(gameObject, rect);
                rect = new Rect(rect.xMin - Style.maxWidth, rect.y, Style.maxWidth, rect.height);
                rect = DrawTag(gameObject, rect);
                //rect = new Rect(rect.xMin - Style.maxWidth, rect.y, Style.maxWidth, rect.height);
                //rect = DrawTag(gameObject, rect);
            }
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
            var content = new GUIContent(gameObject.tag);

            if (content.text != ToolboxEditorUtility.defaultUnityTag)
            {
                GUI.Label(rect, content, Style.labelStyle);
            }

            return rect;
        }


        internal static class Style
        {
            internal static float maxWidth = 60.0f;
            internal static float iconWidth = 18.0f;
            internal static float iconHeight = 18.0f;

            internal static GUIStyle labelStyle;

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