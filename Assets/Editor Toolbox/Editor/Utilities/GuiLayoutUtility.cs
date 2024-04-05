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
        public static readonly float layoutPadding = -2 * EditorGUIUtility.standardVerticalSpacing;

        public static void BeginStrechedVertical()
        {
            BeginFixedVertical(GUIStyle.none);
        }

        public static void BeginFixedVertical(GUIStyle style)
        {
            BeginFixedVertical(style, layoutPadding);
        }

        public static void BeginFixedVertical(GUIStyle style, float padding)
        {
            //NOTE: horizontal layout as a parent allow us to create additional padding
            GUILayout.BeginHorizontal();
            GUILayout.Space(padding);
            GUILayout.BeginVertical(style);
        }

        public static void CloseFixedVertical()
        {
            CloseFixedVertical(layoutPadding);
        }

        public static void CloseFixedVertical(float padding)
        {
            GUILayout.EndVertical();
            GUILayout.Space(padding);
            GUILayout.EndHorizontal();
        }

        public static void RemoveStandardSpacing()
        {
            GUILayout.Space(-EditorGUIUtility.standardVerticalSpacing);
        }

        public static void CreateSpace(float space)
        {
            GUILayout.Space(space);
        }

        /// <summary>
        /// A bit hacky way to retrieve the width of currently active layout.
        /// Width is properly calculated only when the <see cref="Event.type"/> equals to <see cref="EventType.Repaint"/>.
        /// </summary>
        public static bool TryGetLayoutWidth(out float width)
        {
            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                if (Event.current.type == EventType.Repaint)
                {
                    var scopeRect = scope.rect;
                    width = scopeRect.width;
                    return true;
                }
                else
                {
                    width = 0.0f;
                    return false;
                }
            }
        }
    }
}