using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class ConditionAttribute : ToolboxConditionAttribute
    {
        public ConditionAttribute(string comparedPropertyName, object comparedConditionValue)
        {
            ComparedPropertyName = comparedPropertyName;
            ComparedConditionValue = comparedConditionValue;
        }

        public string ComparedPropertyName { get; private set; }

        public object ComparedConditionValue { get; private set; }
    }
}