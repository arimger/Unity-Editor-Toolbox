using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws collection in form of the scrollable list.
    /// It's a perfect way to optimize large arrays within the Inspector Window.
    /// 
    /// <para>Supported types: any <see cref="System.Collections.IList"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ScrollableItemsAttribute : ToolboxListPropertyAttribute
    {
        public ScrollableItemsAttribute(int defaultMinIndex = 0, int defaultMaxIndex = 20)
        {
            DefaultMinIndex = defaultMinIndex;
            DefaultMaxIndex = defaultMaxIndex;
        }

        public int DefaultMinIndex { get; private set; }
        public int DefaultMaxIndex { get; private set; }
    }
}