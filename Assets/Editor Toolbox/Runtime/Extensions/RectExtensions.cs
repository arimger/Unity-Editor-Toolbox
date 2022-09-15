using UnityEngine;

namespace Toolbox
{
    public static class RectExtensions
    {
        public static Rect AlignRight(this Rect rect, float newWidth)
        {
            rect.xMin = rect.xMax - newWidth;
            return rect;
        }

        public static Rect AlignLeft(this Rect rect, float newWidth)
        {
            rect.xMax = rect.xMin + newWidth;
            return rect;
        }

        public static Rect AlignBottom(this Rect rect, float newHeight)
        {
            rect.yMin = rect.yMax - newHeight;
            return rect;
        }

        public static Rect AlignTop(this Rect rect, float newHeight)
        {
            rect.yMax = rect.yMin + newHeight;
            return rect;
        }

        public static Rect AlignCenterX(this Rect rect, float newWidth)
        {
            var offset = (rect.width - newWidth) / 2;
            rect.xMin += offset;
            rect.xMax -= offset;
            return rect;
        }

        public static Rect AlignCenterY(this Rect rect, float newHeight)
        {
            var offset = (rect.height - newHeight) / 2;
            rect.yMin += offset;
            rect.yMax -= offset;
            return rect;
        }

        public static Rect MoveByX(this Rect rect, float offset, float spacing = 0.0f)
        {
            rect.x += offset + spacing;
            return rect;
        }

        public static Rect MoveByY(this Rect rect, float offset, float spacing = 0.0f)
        {
            rect.y += offset + spacing;
            return rect;
        }

        public static Rect AddMax(this Rect rect, Vector2 range)
        {
            rect.xMax += range.x;
            rect.yMin += range.y;
            return rect;
        }

        public static Rect AddMin(this Rect rect, Vector2 range)
        {
            rect.xMin += range.x;
            rect.yMin += range.y;
            return rect;
        }

        public static Rect SubMax(this Rect rect, Vector2 range)
        {
            rect.xMax -= range.x;
            rect.yMin -= range.y;
            return rect;
        }

        public static Rect SubMin(this Rect rect, Vector2 range)
        {
            rect.xMin -= range.x;
            rect.yMin -= range.y;
            return rect;
        }
    }
}