using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws an associated built-in Editor.
    /// Supported types: any <see cref="Object"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
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