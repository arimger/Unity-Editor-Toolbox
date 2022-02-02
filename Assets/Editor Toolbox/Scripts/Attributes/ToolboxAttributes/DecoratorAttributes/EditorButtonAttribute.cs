using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Creates Button control and allows to invoke particular methods within the target class.
    /// This extension supports static methods, standard methods, and <see cref="Coroutine"/>s.
    /// Requirements: method has to be parameterless. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class EditorButtonAttribute : ToolboxDecoratorAttribute
    {
        public EditorButtonAttribute(string methodName, string extraLabel = null, ButtonActivityType activityType = ButtonActivityType.Everything)
        {
            MethodName = methodName;
            ExtraLabel = extraLabel;
            ActivityType = activityType;
        }

        public string MethodName { get; private set; }

        public string ExtraLabel { get; private set; }

        public string Tooltip { get; set; }

        public ButtonActivityType ActivityType { get; private set; }
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