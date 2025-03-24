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
        /// <summary>
        /// If not <see langword="null"/> will be used to retrive validation method.
        /// Validation method will be used to disable/enable the button in the Inspector Window.
        /// </summary>
        public string ValidateMethodName { get; set; }
        public string ExtraLabel { get; private set; }
        public string Tooltip { get; set; }
        public ButtonActivityType ActivityType { get; set; }
        public ButtonPositionType PositionType { get; set; } = ButtonPositionType.Default;
    }

    public enum ButtonPositionType
    {
        Default,
        Below,
        Above
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