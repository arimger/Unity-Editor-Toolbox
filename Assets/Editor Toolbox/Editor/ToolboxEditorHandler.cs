using System;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;

    internal static class ToolboxEditorHandler
    {
        //TODO: how to pass dditor and multiple serialized objects
        public static void HandleToolboxEditor(IToolboxEditor editor)
        {
            try
            {
                ContextEditor = editor.ContextEditor;
                OnBeginToolboxEditor?.Invoke(ContextEditor);
                editor.DrawCustomInspector();
            }
            catch (Exception)
            {
                //make sure to catch all Exceptions (especially ExitGUIException),
                //it will allow us to safely dispose all layout-based controls, etc.
                OnBreakToolboxEditor?.Invoke(ContextEditor);
                throw;
            }
            finally
            {
                OnCloseToolboxEditor?.Invoke(ContextEditor);
                ContextEditor = null;
            }
        }


        public static event Action<Editor> OnBeginToolboxEditor;
        public static event Action<Editor> OnBreakToolboxEditor;
        public static event Action<Editor> OnCloseToolboxEditor;

        public static Editor ContextEditor { get; private set; }
    }
}