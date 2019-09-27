using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public abstract class ButtonAttribute : PropertyAttribute
    {
        public ButtonAttribute(string methodName, string label = null,
            ButtonActivityType type = ButtonActivityType.Everything)
        {
            MethodName = methodName;
            Label = label;
            Type = type;
        }

        public ButtonActivityType Type { get; private set; }

        public string MethodName { get; private set; }

        public string Label { get; private set; }
    }

    [Flags]
    public enum ButtonActivityType
    {
        Nothing = 0,
        OnPlayMode = 1,
        OnEditMode = 2,
        Everything = ~0
    }
}