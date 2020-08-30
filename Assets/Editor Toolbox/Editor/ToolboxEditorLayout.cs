using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;

    /// <summary>
    /// Utility class to handle and validate creation of <see cref="GUILayout"/> and <see cref="EditorGUILayout"/> groups.
    /// Should be used only within the Inspector Window.
    /// </summary>
    [InitializeOnLoad]
    internal static class ToolboxEditorLayout
    {
        static ToolboxEditorLayout()
        {
            //Editor.finishedDefaultHeaderGUI is a quite useful event which can be used to 
            //validate layout data. Actually it should be used to draw additional information
            //into the header but in this case we will check previously created scopes

            //TODO: try to find better way/callback
            Editor.finishedDefaultHeaderGUI += editor => ValidateScopes(editor);
        }


        /// <summary>
        /// Validates currently cached layout scopes.
        /// Should be called right after the layout system call within the Inspector Window.
        /// </summary>
        /// <returns>true if scopes were clean.</returns>
        internal static bool ValidateScopes(Editor editor)
        {
            if (verticalScopes.Count == 0 && horizontalScopes.Count == 0)
            {
                return true;
            }

            ToolboxEditorLog.LogWarning("Invalid layout data. Check if created groups (vertical or horizontal) are properly closed.");

            while (verticalScopes.Count > 0)
            {
                verticalScopes.Pop().Dispose();
            }

            while (horizontalScopes.Count > 0)
            {
                horizontalScopes.Pop().Dispose();
            }

            return false;
        }


        private static readonly Stack<IDisposable> verticalScopes = new Stack<IDisposable>();


        internal static void BeginVertical()
        {
            BeginVertical(new EditorGUILayout.VerticalScope());
        }

        internal static void BeginVertical(GUIStyle style, params GUILayoutOption[] options)
        {
            BeginVertical(new EditorGUILayout.VerticalScope(style, options));
        }

        internal static void BeginVertical(EditorGUILayout.VerticalScope verticalScope)
        {
            verticalScopes.Push(verticalScope);
        }

        internal static void EndVertical()
        {
            if (verticalScopes.Count == 0)
            {
                ToolboxEditorLog.LogWarning("There is no a vertical group to end. Call will be ignored.");
                return;
            }

            verticalScopes.Pop().Dispose();
        }


        private static readonly Stack<IDisposable> horizontalScopes = new Stack<IDisposable>();


        internal static void BeginHorizontal()
        {
            BeginHorizontal(new EditorGUILayout.HorizontalScope());
        }

        internal static void BeginHorizontal(GUIStyle style, params GUILayoutOption[] options)
        {
            BeginHorizontal(new EditorGUILayout.HorizontalScope(style, options));
        }

        internal static void BeginHorizontal(EditorGUILayout.HorizontalScope horizontalScope)
        {
            if (horizontalScopes.Count > 0)
            {
                horizontalScope.Dispose();
                ToolboxEditorLog.LogWarning("Nested horizontal groups are not supported.");
                return;
            }

            horizontalScopes.Push(horizontalScope);
        }

        internal static void EndHorizontal()
        {
            if (horizontalScopes.Count == 0)
            {
                ToolboxEditorLog.LogWarning("There is no a horizontal group to end. Call will be ignored.");
                return;
            }

            horizontalScopes.Pop().Dispose();
        }
    }
}