using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class BoxedGroupAttributeDrawer : ToolboxGroupDrawer<GroupAttribute>
    {
        /// <summary>
        /// Custom styling class.
        /// </summary>
        private static class Style
        {
            internal static readonly float spacing = 2.5f;

            internal static GUIStyle headerStyle;
            internal static GUIStyle headerBackgroundStyle;
            internal static GUIStyle sectionBackgroundStyle;
            internal static GUIStyle foldoutStyle;

            static Style()
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel);
                headerBackgroundStyle = new GUIStyle(GUI.skin.box);
                sectionBackgroundStyle = new GUIStyle(GUI.skin.box);
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };
            }
        }
    }
}