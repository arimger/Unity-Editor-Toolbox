using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Draws collection in form of the reorderable list.
    /// 
    /// <para>Supported types: any <see cref="System.Collections.IList"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class ReorderableListAttribute : ToolboxListPropertyAttribute
    {
        public ReorderableListAttribute(ListStyle style = ListStyle.Round, string elementLabel = null, bool fixedSize = false, bool draggable = true)
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

        /// <summary>
        /// Indicates whether list should be allowed to fold in and out.
        /// </summary>
        public bool Foldable { get; set; }
        /// <summary>
        /// Indicates whether list should have a label above elements.
        /// </summary>
        public bool HasHeader { get; set; } = true;
        /// <summary>
        /// Indicates whether each element should have an additional label.
        /// </summary>
        public bool HasLabels { get; set; } = true;
    }

    public enum ListStyle
    {
        Round,
        Boxed,
        Lined
    }
}