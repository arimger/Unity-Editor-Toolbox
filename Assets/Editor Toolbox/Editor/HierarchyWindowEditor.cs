using UnityEngine;
using UnityEditor;
using Size = UnityEngine.Vector2;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    public class HierarchyWindowEditor
    {
        static HierarchyWindowEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
        }


        private static void HierarchyItemCB(int instanceID, Rect rect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject)
            {
                DrawIcon(gameObject, rect);
                DrawTag(gameObject, rect);
            }
        }

        private static Rect DrawIcon(GameObject gameObject, Rect rect)
        {
            var content = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject)).image;
            if (content.name == ToolboxEditorUtility.defaultIconName)
            {
                return rect;
            }

            var contentSize = new Size(Style.iconWidth, Style.iconHeight);
            var contentRect = new Rect(rect.xMax, rect.y, contentSize.x, contentSize.y);
            //draw icon using internal style class
            GUI.Label(contentRect, content);
            return contentRect;
        }

        private static Rect DrawTag(GameObject gameObject, Rect rect)
        {
            if (gameObject.tag == ToolboxEditorUtility.defaultUnityTag)
            {
                return rect;
            }
            //set native label size using predefined style
            var contentText = "[" + gameObject.tag + "]";
            var contentSize = Style.labelStyle.CalcSize(new GUIContent(contentText));
            var contentRect = new Rect(rect.xMax - contentSize.x, rect.y, contentSize.x, contentSize.y);
            //draw tag label using internal style class
            GUI.Label(contentRect, contentText, Style.labelStyle);
            return contentRect;
        }


        internal static class Style
        {
            internal static float maxWidth = 70.0f;
            internal static float iconWidth = 18.0f;
            internal static float iconHeight = 18.0f;

            internal static GUIStyle labelStyle;

            static Style()
            {
                labelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    fontSize = 8
                };
            }
        }
    }
}