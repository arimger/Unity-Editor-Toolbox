using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DynamicRangeAttribute : DynamicMinMaxBaseAttribute
    {
        public DynamicRangeAttribute(string minValueSource, string maxValueSource) : base(minValueSource, maxValueSource)
        { }
    }
}