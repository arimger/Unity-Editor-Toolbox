using System;
using System.Diagnostics;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class TabAttribute : ToolboxConditionAttribute
    {
        public TabAttribute(string tab)
        {
            Tab = tab;
        }

        public string Tab { get; }
    }
}