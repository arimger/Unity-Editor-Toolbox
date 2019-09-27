using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class BroadcastButtonAttribute : ButtonAttribute
    {
        public BroadcastButtonAttribute(string methodName, string label = null, ButtonActivityType type = ButtonActivityType.Everything) 
            : base(methodName, label, type)
        { }
    }
}