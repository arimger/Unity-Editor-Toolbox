using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class ComparisonAttribute : ToolboxConditionAttribute
    {
        public ComparisonAttribute(string comparedPropertyName, object targetConditionValue)
        {
            ComparedPropertyName = comparedPropertyName;
            TargetConditionValue = targetConditionValue;
        }

        public string ComparedPropertyName { get; private set; }

        public object TargetConditionValue { get; private set; }
    }
}