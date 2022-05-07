using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Draws an associated built-in Editor.
    /// 
    /// <para>Supported types: any <see cref="Object"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class InLineEditorAttribute : ToolboxSelfPropertyAttribute
    {
        public InLineEditorAttribute(bool drawPreview = true, bool drawSettings = false)
        {
            DrawPreview = drawPreview;
            DrawSettings = drawSettings;
        }

        [Obsolete("Drawing the inlined header is no longer supported.")]
        public InLineEditorAttribute(bool drawHeader, bool drawPreview = true, bool drawSettings = false)
        {
            DrawHeader = false;
            DrawPreview = drawPreview;
            DrawSettings = drawSettings;
        }

        [Obsolete]
        public bool DrawHeader { get; private set; }

        public bool DrawPreview { get; private set; }

        public bool DrawSettings { get; private set; }

        /// <summary>
        /// Indicates if the inlined Editor should be disabled.
        /// </summary>
        public bool DisableEditor { get; set; }

        public float PreviewHeight { get; set; } = 90.0f;
    }
}