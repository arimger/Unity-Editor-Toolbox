using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws collection in form of the scrollable list.
    /// It's a perfect way to optimize large arrays within the Inspector Window.
    /// Supported types: any <see cref="System.Collections.IList"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ScrollableItemsAttribute : ToolboxListPropertyAttribute
    {
        public ScrollableItemsAttribute(float areaHeight = 100.0f)
        {
            AreaHeight = areaHeight;
        }

        public int DefaultMinIndex { get; set; } = 0;
        public int DefaultMaxIndex { get; set; } = 20;

        public float AreaHeight { get; private set; }
    }
}