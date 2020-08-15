using System;

namespace UnityEngine
{
    /// <summary>
    /// Draw editors associated to <see cref="Object"/> property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class InLineEditorAttribute : ToolboxPropertyAttribute
    {
        public InLineEditorAttribute(bool drawHeader = false, bool drawPreview = true, bool drawSettings = false)
        {
            DrawHeader = drawHeader;
            DrawPreview = drawPreview;
            DrawSettings = drawSettings;
        }

        public bool DrawHeader { get; private set; }

        public bool DrawPreview { get; private set; }

        public bool DrawSettings { get; private set; }

        public float PreviewHeight { get; set; } = 90.0f;
    }
}