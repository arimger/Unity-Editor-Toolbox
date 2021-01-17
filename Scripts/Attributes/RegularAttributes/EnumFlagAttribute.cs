using System;

namespace UnityEngine
{
    /// <summary>
    /// Allows to pick multiple enum values and crates a proper result mask.
    /// Supported types: any <see cref="Enum"/> with the <see cref="FlagsAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumFlagAttribute : PropertyAttribute
    {
        public EnumFlagAttribute(EnumStyle style = EnumStyle.Popup)
        {
            Style = style;
        }

        public EnumStyle Style { get; private set; }

        public float ButtonsSpacing { get; set; } = 3.0f;
        public float ButtonsHeight { get; set; } = 16.0f;
        public float ButtonsWidth { get; set; } = 85.0f;
    }

    public enum EnumStyle
    {
        Popup,
        Button
    }
}