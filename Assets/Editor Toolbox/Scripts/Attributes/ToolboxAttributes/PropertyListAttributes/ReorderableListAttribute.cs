using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws collection in form of the reorderable list.
    /// 
    /// <para>Supported types: any <see cref="System.Collections.IList"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ReorderableListAttribute : ToolboxListPropertyAttribute
    {
        public ReorderableListAttribute(ListStyle style = ListStyle.Round, string elementLabel = null, bool fixedSize = false, bool draggable = true)
        {
            Draggable = draggable;
            FixedSize = fixedSize;
            ListStyle = style;
            ElementLabel = elementLabel;
        }

        public bool HasHeader { get; set; } = true;

        public bool HasLabels { get; set; } = true;

        public bool Draggable { get; private set; }

        public bool FixedSize { get; private set; }

        public ListStyle ListStyle { get; private set; }

        public string ElementLabel { get; private set; }
    }

    public enum ListStyle
    {
        Round,
        Boxed,
        Lined
    }
}