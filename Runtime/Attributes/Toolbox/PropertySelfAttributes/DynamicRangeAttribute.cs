using System;
using System.Diagnostics;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class DynamicRangeAttribute : DynamicMinMaxBaseAttribute
    {
        public DynamicRangeAttribute(string minValueSource, string maxValueSource) : base(minValueSource, maxValueSource)
        { }
    }
}