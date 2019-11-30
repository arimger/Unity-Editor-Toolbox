using System;

namespace UnityEngine
{
    /// <summary>
    /// Draw editors associated to <see cref="Object"/> property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class InLineEditorAttribute : ToolboxPropertyAttribute
    {
        public InLineEditorAttribute(bool drawHeader = false, bool drawPreview = true)
        {
            DrawHeader = drawHeader;
            DrawPreview = drawPreview;
        }

        public bool DrawHeader { get; private set; }

        public bool DrawPreview { get; private set; }

        public float PreviewHeight { get; set; } = 90.0f;
    }
}