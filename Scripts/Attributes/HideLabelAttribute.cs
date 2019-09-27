using System;

namespace UnityEngine
{
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class HideLabelAttribute : PropertyAttribute
    { }
}