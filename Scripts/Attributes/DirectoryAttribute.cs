using System;

namespace UnityEngine
{
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false)]
    public class DirectoryAttribute : PropertyAttribute
    { }
}