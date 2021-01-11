using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class ComparisonAttribute : ToolboxConditionAttribute
    {
        public ComparisonAttribute(string propertyName, object valueToMatch)
        {
            PropertyName = propertyName;
            ValueToMatch = valueToMatch;
        }

        public string PropertyName { get; private set; }

        public object ValueToMatch { get; private set; }
    }
}