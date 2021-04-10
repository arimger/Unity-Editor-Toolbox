using System;

namespace UnityEngine
{
    /// <summary>
    /// Allows to pick multiple enum values and crates a proper result mask.
    /// 
    /// <para>Supported types: any <see cref="Enum"/> with the <see cref="FlagsAttribute"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumFlagAttribute : PropertyAttribute
    {
        public EnumFlagAttribute(EnumStyle style = EnumStyle.Popup)
        {
            Style = style;
        }

        public EnumStyle Style { get; private set; }

        /// <summary>
        /// Spacing between buttons in the <see cref="EnumStyle.Button"/> version.
        /// </summary>
        public float ButtonsSpacing { get; set; } = 3.0f;
        /// <summary>
        /// Height of the buttons in the <see cref="EnumStyle.Button"/> version.
        /// </summary>
        public float ButtonsHeight { get; set; } = 16.0f;
        /// <summary>
        /// Width of the buttons in the <see cref="EnumStyle.Button"/> version.
        /// </summary>
        public float ButtonsWidth { get; set; } = 85.0f;
    }

    public enum EnumStyle
    {
        Popup,
        Button
    }
}