using System;
using System.Diagnostics;

namespace UnityEngine
{

    /// <summary>
    /// Defines a custom drawing order for the associated property within a parent marked with <see cref="SortableAttribute"/>.
    /// Properties with lower order values are drawn first. Unordered properties are drawn at the end.
    /// 
    /// <para>Supported types: any.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class OrderAttribute : ToolboxAttribute
    {
        public int Order { get; private set; }

        public OrderAttribute(int order)
        {
            Order = order;
        }
    }

}