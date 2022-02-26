using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Creates a progress bar.
    /// 
    /// <para>Supported types: <see cref="int"/>, <see cref="float"/>, <see cref="double"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class ProgressBarAttribute : PropertyAttribute
    {
        public ProgressBarAttribute(string name = "", float minValue = 0, float maxValue = 100)
        {
            Name = name;

            MinValue = Mathf.Min(minValue, maxValue);
            MaxValue = Mathf.Max(maxValue, minValue);
        }

        public string Name { get; private set; }

        public float MinValue { get; private set; }

        public float MaxValue { get; private set; }

        public Color Color
        {
            get => ColorUtility.TryParseHtmlString(HexColor, out var color)
                ? color
                : new Color(0.28f, 0.38f, 0.88f);
        }

        public string HexColor { get; set; }
    }
}