using System;

namespace UnityEngine
{
    /// <summary>
    /// Use this attribute on arrays or generic lists to draw reorderable list in inspector window. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReorderableListAttribute : ToolboxCollectionAttribute
    {
        public ReorderableListAttribute(ListStyle style = ListStyle.Round, string elementLabel = null,  bool fixedSize = false, bool draggable = true)
        {
            Draggable = draggable;
            FixedSize = fixedSize;
            ListStyle = style;
            ElementLabel = elementLabel;
        }

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