using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws progress bar.
    /// Supported types: <see cref="int"/>, <see cref="float"/>, <see cref="double"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ProgressBarAttribute : PropertyAttribute
    {
        public ProgressBarAttribute(string name = "", float minValue = 0, float maxValue = 100)
        {
            Name = name;

            MinValue = Mathf.Min(minValue, maxValue);
            MaxValue = Mathf.Max(maxValue, minValue);
        }

        public Color GetBarColor()
        {
            if (ColorUtility.TryParseHtmlString(HexColor, out var color))
            {
                return color;
            }
            else
            {
                return new Color(0.28f, 0.38f, 0.88f);
            }
        }

        public string Name { get; private set; }

        public string HexColor { get; set; }

        public float MinValue { get; private set; }

        public float MaxValue { get; private set; }
    }
}