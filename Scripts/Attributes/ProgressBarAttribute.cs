using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
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
            if (ColorUtility.TryParseHtmlString(Color, out var color))
            {
                return color;
            }
            else
            {
                return new Color(0.28f, 0.38f, 0.88f);
            }
        }

        public string Name { get; private set; }

        public string Color { get; set; }

        public float MinValue { get; private set; }

        public float MaxValue { get; private set; }
    }
}