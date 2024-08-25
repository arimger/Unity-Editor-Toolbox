using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Hierarchy
{
    /// <summary>
    /// Dedicates utility class used to draw horizontal/vertical lines when rendering tree connection lines within the Hierarchy window.
    /// </summary>
    internal static class HierarchyTreeUtility
    {
        //NOTE: consider caching pre-created texturess for dashed lines and use them to optimize each call
        private const float dashLength = 4.0f;
        private const float spaceLength = 1.0f;

        private static Rect GetLineRect(Rect rect, float thickness, float padding, bool horizontal)
        {
            return horizontal
                ? new Rect(rect.x, rect.y + padding / 2, rect.width, thickness)
                : new Rect(rect.x + padding / 2, rect.y, thickness, rect.height);
        }

        public static void DrawVerticalLine(Rect rect, bool isDashed, float tickness, Color color)
        {
            DrawVerticalLine(rect, isDashed, tickness, color, 0.0f);
        }

        public static void DrawVerticalLine(Rect rect, bool isDashed, float tickness, Color color, float paddingOffset)
        {
            rect = GetLineRect(rect, tickness, rect.width - paddingOffset, false);
            if (!isDashed)
            {
                EditorGUI.DrawRect(rect, color);
                return;
            }

            var dashesCount = rect.height / (dashLength + spaceLength);
            var maxY = rect.yMax;

            rect.yMax = rect.yMin + dashLength;
            for (var i = 0; i < dashesCount; i++)
            {
                EditorGUI.DrawRect(rect, color);
                rect.y += dashLength + spaceLength;
                if (rect.yMax > maxY)
                {
                    rect.yMax = maxY;
                }
            }
        }

        public static void DrawHorizontalLine(Rect rect, bool isDashed, float tickness, Color color)
        {
            DrawHorizontalLine(rect, isDashed, tickness, color, 0.0f);
        }

        public static void DrawHorizontalLine(Rect rect, bool isDashed, float tickness, Color color, float paddingOffset)
        {
            rect = GetLineRect(rect, tickness, rect.height - paddingOffset, true);
            if (!isDashed)
            {
                EditorGUI.DrawRect(rect, color);
                return;
            }

            var dashesCount = rect.width / (dashLength + spaceLength);
            var maxX = rect.xMax;

            rect.xMax = rect.xMin + dashLength;
            for (var i = 0; i < dashesCount; i++)
            {
                EditorGUI.DrawRect(rect, color);
                rect.x += dashLength + spaceLength;
                if (rect.xMax > maxX)
                {
                    rect.xMax = maxX;
                }
            }
        }

        public static void DrawPassingLine(Rect rect, bool isDashed, float tickness, Color color)
        {
            DrawPassingLine(rect, isDashed, tickness, color, Vector2.zero);
        }

        public static void DrawPassingLine(Rect rect, bool isDashed, float tickness, Color color, Vector2 paddingOffset)
        {
            DrawVerticalLine(rect, isDashed, tickness, color, paddingOffset.x);
        }

        public static void DrawCornerLine(Rect rect, bool isDashed, float tickness, Color color)
        {
            DrawCornerLine(rect, isDashed, tickness, color, Vector2.zero, 0.0f);
        }

        public static void DrawCornerLine(Rect rect, bool isDashed, float tickness, Color color, Vector2 paddingOffset, float horizontalSizeOffset)
        {
            //NOTE: -1 as a offset for halfs in corners
            var verticalRect = rect;
            verticalRect.yMax -= verticalRect.height / 2 - 1;
            DrawVerticalLine(verticalRect, isDashed, tickness, color, paddingOffset.x);

            var horizontalRect = rect;
            horizontalRect.xMin += horizontalRect.width / 2;
            horizontalRect.xMax += horizontalSizeOffset;
            DrawHorizontalLine(horizontalRect, isDashed, tickness, color, paddingOffset.y);
        }

        public static void DrawCrossLine(Rect rect, bool isDashed, float tickness, Color color)
        {
            DrawCrossLine(rect, isDashed, tickness, color, Vector2.zero, 0.0f);
        }

        public static void DrawCrossLine(Rect rect, bool isDashed, float tickness, Color color, Vector2 paddingOffset, float horizontalSizeOffset)
        {
            DrawVerticalLine(rect, isDashed, tickness, color, paddingOffset.x);

            rect.xMin += rect.width / 2;
            rect.xMax += horizontalSizeOffset;
            DrawHorizontalLine(rect, isDashed, tickness, color, paddingOffset.y);
        }
    }
}