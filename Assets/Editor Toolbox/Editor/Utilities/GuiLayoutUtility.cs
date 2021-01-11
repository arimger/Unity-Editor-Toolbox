using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    /// <summary>
    /// Toolbox-based overlay for the built-in classes <see cref="GUILayout"/> and <see cref="EditorGUILayout"/>.
    /// </summary>
    public static class GuiLayoutUtility
    {
        /// <summary>
        /// Base padding for the vertical groups to keep them fixed relative to non-layout elements.
        /// </summary>
        private static readonly float layoutPadding = -2 * EditorGUIUtility.standardVerticalSpacing;


        public static void BeginStrechedVertical()
        {
            BeginStrechedVertical(new GUIStyle());
        }

        public static void BeginStrechedVertical(GUIStyle style)
        {
            BeginStrechedVertical(style, layoutPadding);
        }

        public static void BeginStrechedVertical(GUIStyle style, float padding)
        {
            //NOTE: horizontal layout as a parent allow us to create additional padding
            GUILayout.BeginHorizontal();
            GUILayout.Space(padding);
            GUILayout.BeginVertical(style);
        }

        public static void CloseStretchedVertical()
        {
            CloseStretchedVertical(layoutPadding);
        }

        public static void CloseStretchedVertical(float padding)
        {
            GUILayout.EndVertical();
            GUILayout.Space(padding);
            GUILayout.EndHorizontal();
        }
    }
}