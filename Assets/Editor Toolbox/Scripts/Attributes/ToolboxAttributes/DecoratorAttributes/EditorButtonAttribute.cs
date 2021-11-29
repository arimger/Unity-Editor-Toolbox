using System;

namespace UnityEngine
{
    /// <summary>
    /// Creates Button control and allows to invoke particular methods within the target class.
    /// This extension supports static methods, standard methods, and <see cref="Coroutine"/>s.
    /// Requirements: method has to be parameterless. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EditorButtonAttribute : ToolboxDecoratorAttribute
    {
        public EditorButtonAttribute(string methodName, string extraLabel = null,
            string interactionMethodName = null)
        {
            MethodName = methodName;
            ExtraLabel = extraLabel;
            InteractionMethodName = interactionMethodName;
        }

        public string MethodName { get; private set; }

        public string ExtraLabel { get; private set; }

        public string Tooltip { get; set; }

        public string InteractionMethodName { get; private set; }
    }
}