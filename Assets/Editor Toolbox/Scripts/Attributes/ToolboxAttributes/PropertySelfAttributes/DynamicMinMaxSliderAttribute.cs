using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DynamicMinMaxSliderAttribute : DynamicMinMaxBaseAttribute
    {
        public DynamicMinMaxSliderAttribute(string minValueSource, string maxValueSource) : base(minValueSource, maxValueSource)
        { }
    }
}