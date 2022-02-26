using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Creates thin, horizontal line.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class LineAttribute : ToolboxDecoratorAttribute
    {
        public LineAttribute(float thickness = 0.75f, float padding = 6.0f)
        {
            Thickness = Math.Max(thickness, 0);
            Padding = padding;
            IsHorizontal = true;
        }

        public float Thickness { get; private set; }

        public float Padding { get; private set; }

        /// <summary>
        /// Indicates if the line should be horizontal, otherwise will be drawn vertically.
        /// </summary>
        public bool IsHorizontal { get; set; }

        /// <summary>
        /// Indicates if drawer should apply additional indent to the line's width.
        /// </summary>
        public bool ApplyIndent { get; set; }

        /// <summary>
        /// Allows to override the color of the horizontal line.
        /// </summary>
        public string HexColor { get; set; }

        /// <summary>
        /// Returns the expected color of the horizontal line.
        /// </summary>
        public Color GuiColor
        {
            get => ColorUtility.TryParseHtmlString(HexColor, out var color)
                ? color
                : new Color(0.3f, 0.3f, 0.3f);
        }
    }
}