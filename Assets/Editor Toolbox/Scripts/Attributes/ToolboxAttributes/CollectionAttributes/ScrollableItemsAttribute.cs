using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws collection in form of the scrollable list.
    /// Supported types: any <see cref="System.Collections.IList"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ScrollableItemsAttribute : ToolboxListPropertyAttribute
    {
        public ScrollableItemsAttribute(float areaHeight = 100.0f)
        {
            AreaHeight = areaHeight;
        }

        //TODO:
        //public int MinItemsCount { get; set; } = 0;

        public float AreaHeight { get; private set; }
    }
}