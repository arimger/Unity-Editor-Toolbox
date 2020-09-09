using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EndGroupAttribute : ToolboxDecoratorAttribute
    {
        public EndGroupAttribute()
        {
            Order = -1000;
        }
    }
}