using System;
using UnityEditor;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;
    using Editor = UnityEditor.Editor;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Base Editor class for all Toolbox-related features.
    /// </summary>
#if !TOOLBOX_IGNORE_CUSTOM_EDITOR
    [CustomEditor(typeof(Object), true, isFallback = true)]
#endif
    [CanEditMultipleObjects]
    public class ToolboxEditor : Editor, IToolboxEditor
    {
        /// <summary>
        /// Inspector GUI re-draw call.
        /// </summary>
        public sealed override void OnInspectorGUI()
        {
            ToolboxEditorHandler.HandleToolboxEditor(this);
        }

        /// <inheritdoc />
        [Obsolete("To draw properties in a different way override the Drawer property.")]
        public virtual void DrawCustomProperty(SerializedProperty property)
        { }

        /// <inheritdoc />
        public virtual void DrawCustomInspector()
        {
            Drawer.DrawEditor(serializedObject);
        }

        /// <inheritdoc />
        public void IgnoreProperty(SerializedProperty property)
        {
            Drawer.IgnoreProperty(property);
        }

        /// <inheritdoc />
        public void IgnoreProperty(string propertyPath)
        {
            Drawer.IgnoreProperty(propertyPath);
        }

        Editor IToolboxEditor.ContextEditor => this;
        /// <inheritdoc />
        public virtual IToolboxEditorDrawer Drawer { get; } = new ToolboxEditorDrawer();

#pragma warning disable 0067
        [Obsolete("ToolboxEditorHandler.OnBeginToolboxEditor")]
        public static event Action<Editor> OnBeginToolboxEditor;
        [Obsolete("ToolboxEditorHandler.OnBreakToolboxEditor")]
        public static event Action<Editor> OnBreakToolboxEditor;
        [Obsolete("ToolboxEditorHandler.OnCloseToolboxEditor")]
        public static event Action<Editor> OnCloseToolboxEditor;
#pragma warning restore 0067
    }
}