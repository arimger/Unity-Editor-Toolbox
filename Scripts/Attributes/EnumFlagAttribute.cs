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
    }

    public enum EnumStyle
    {
        Popup,
        Button
    }
}