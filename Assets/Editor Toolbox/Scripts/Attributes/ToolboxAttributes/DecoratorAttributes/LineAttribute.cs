using System;

namespace UnityEngine
{
    /// <summary>
    /// Creates thin, horizontal line.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class LineAttribute : ToolboxDecoratorAttribute
    {
        public LineAttribute(float thickness = 0.75f, float padding = 6.0f)
        {
            //validate the thickness property
            Thickness = Math.Max(thickness, 0);
            //validate the padding property
            Padding = Math.Max(padding, 0);
        }

        public Color GetLineColor()
        {
            if (ColorUtility.TryParseHtmlString(HexColor, out var color))
            {
                return color;
            }
            else
            {
                return new Color(0.3f, 0.3f, 0.3f);
            }
        }

        public float Thickness { get; private set; }

        public float Padding { get; private set; }

        public string HexColor { get; set; }
    }
}