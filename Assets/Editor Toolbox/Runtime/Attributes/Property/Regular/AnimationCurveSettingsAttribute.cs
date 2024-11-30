using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Allows to draw <see cref="AnimationCurve"/> in a custom way, e.g. you can specify the range.
    /// 
    /// <para>Supported types: any <see cref="AnimationCurve"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class AnimationCurveSettingsAttribute : PropertyAttribute
    {
        public AnimationCurveSettingsAttribute()
            : this(Vector2.zero, Vector2.one)
        { }

        public AnimationCurveSettingsAttribute(float minX, float minY, float maxX, float maxY)
            : this(new Vector2(minX, minY), new Vector2(maxX, maxY))
        { }

        public AnimationCurveSettingsAttribute(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public Vector2 Min { get; private set; }

        public Vector2 Max { get; private set; }

        public Color Color
        {
            get => ColorUtility.TryParseHtmlString(HexColor, out var color)
                ? color
                : Color.green;
        }

        public string HexColor { get; set; }
    }
}