using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class ComparisonAttribute : ToolboxConditionAttribute
    {
        public ComparisonAttribute(string comparedPropertyName, object comparedConditionValue)
        {
            ComparedPropertyName = comparedPropertyName;
            ComparedConditionValue = comparedConditionValue;
        }

        public string ComparedPropertyName { get; private set; }

        public object ComparedConditionValue { get; private set; }
    }
}