using System;
using System.Collections.Generic;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;
    using Object = UnityEngine.Object;

    public static class ToolboxEditorHandler
    {
        private static int lastCachedEditorId;
        private static Editor lastCachedEditor;
        private static readonly Stack<Editor> cachedEditors = new Stack<Editor>();

        private static void OnBeginEditor(Editor editor)
        {
            //NOTE: it means that last Editor was null or disposed, anyway we probably want to reload drawers-related cache
            if (lastCachedEditor == null || lastCachedEditorId != lastCachedEditor.GetInstanceID())
            {
                lastCachedEditor = editor;
                lastCachedEditorId = editor.GetInstanceID();
                OnEditorReload?.Invoke();
            }

            cachedEditors.Push(editor);
            OnBeginToolboxEditor?.Invoke(editor);
        }

        private static void OnBreakEditor(Editor editor)
        {
            cachedEditors.Clear();
            OnBreakToolboxEditor?.Invoke(editor);
        }

        private static void OnCloseEditor(Editor editor)
        {
            if (InToolboxEditor)
            {
                cachedEditors.Pop();
            }

            OnCloseToolboxEditor?.Invoke(editor);
            ContextEditor = null;
        }

        public static void HandleToolboxEditor(IToolboxEditor editor)
        {
            try
            {
                ContextEditor = editor.ContextEditor;
                OnBeginEditor(ContextEditor);
                editor.DrawCustomInspector();
            }
            catch (Exception)
            {
                //make sure to catch all Exceptions (especially ExitGUIException),
                //it will allow us to safely dispose all layout-based controls, etc.
                OnBreakEditor(ContextEditor);
                throw;
            }
            finally
            {
                OnCloseEditor(ContextEditor);
            }
        }

        /// <summary>
        /// Event fired every time when <see cref="ToolboxEditor"/>s were re-created.
        /// </summary>
        internal static event Action OnEditorReload;

        internal static event Action<Editor> OnBeginToolboxEditor;
        internal static event Action<Editor> OnBreakToolboxEditor;
        internal static event Action<Editor> OnCloseToolboxEditor;

        internal static bool InToolboxEditor
        {
            get => cachedEditors.Count > 0;
        }

        /// <summary>
        /// Last cached targetObjects from the currently processed <see cref="ToolboxEditor"/>.
        /// </summary>
        internal static Object[] CurrentTargetObjects
        {
            get => cachedEditors.Count > 0 ? cachedEditors.Peek().targets : new Object[0];
        }

        /// <summary>
        /// Currently maintained <see cref="Editor"/>.
        /// </summary>
        internal static Editor ContextEditor { get; private set; }
    }
}