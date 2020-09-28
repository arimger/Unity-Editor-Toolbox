using System;

namespace UnityEngine
{
    /// <summary>
    /// TODO
    /// </summary>
    [Obsolete]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ToolboxButtonAttribute : ToolboxDecoratorAttribute
    {
        public ToolboxButtonAttribute(string methodName, string label = null, ButtonActivityType type = ButtonActivityType.Everything)
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