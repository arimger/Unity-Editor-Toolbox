using System;

namespace UnityEngine
{
    /// <summary>
    /// Use this attribute on <see cref="Enum"/> fields marked with the <see cref="FlagsAttribute"/>.
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