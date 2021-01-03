using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public abstract class ButtonAttribute : PropertyAttribute
    {
        public ButtonAttribute(string methodName, string label = null, ButtonActivityType type = ButtonActivityType.Everything)
        {
            MethodName = methodName;
            Label = label;
            Type = type;
        }

        public ButtonActivityType Type { get; private set; }

        public string MethodName { get; private set; }

        public string Label { get; private set; }
    }
}