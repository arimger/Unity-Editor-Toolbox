using System;

namespace UnityEngine
{
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false)]
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
        [Obsolete("Work in progress.")]
        Button
    }
}