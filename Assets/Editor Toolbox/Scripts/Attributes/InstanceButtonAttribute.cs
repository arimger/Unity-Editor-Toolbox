using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class InstanceButtonAttribute : ButtonAttribute
    {
        public InstanceButtonAttribute(Type instanceType, string methodName, string label = null,
            ButtonActivityType type = ButtonActivityType.Everything) : base(methodName, label, type)
        {
            InstanceType = instanceType;
        }

        public Type InstanceType { get; private set; }
    }
}