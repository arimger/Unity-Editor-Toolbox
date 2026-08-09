using System;
using System.Diagnostics;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class BeginTabGroupAttribute : ToolboxDecoratorAttribute
    {
        public BeginTabGroupAttribute(string groupId = "Default")
        {
            GroupId = groupId;
        }

        public string GroupId { get; }
    }
}