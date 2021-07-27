using System;

namespace UnityEngine
{
    /// <summary>
    /// Calls given callback each time associated property is changed.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class OnValueChangedAttribute : PropertyAttribute
    {
        public OnValueChangedAttribute(string callbackMethodName)
        {
            CallbackMethodName = callbackMethodName;
        }

        public string CallbackMethodName { get; private set; }
    }
}