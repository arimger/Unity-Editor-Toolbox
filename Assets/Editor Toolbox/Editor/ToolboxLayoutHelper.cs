using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;

    /// <summary>
    /// Helper class to handle and validate creation of <see cref="GUILayout"/> and <see cref="EditorGUILayout"/> groups.
    /// Remark: can be used only within the Toolbox Editors.
    /// </summary>
    [InitializeOnLoad]
    internal static class ToolboxLayoutHelper
    {
        static ToolboxLayoutHelper()
        {
            //ensure that on the begining of a frame we are not within the Inspector Window
            EditorApplication.update += () =>
            {
                inEditorLayout = false;
            };

            //ToolboxEditor.OnCloseToolboxEditor is a quite useful event which can be used to 
            //validate layout data. Actually it should be used to draw additional information
            //into the target Editor but in this case we will check previously created scopes

            ToolboxEditor.OnBeginToolboxEditor += OnBeginEditor;
            ToolboxEditor.OnCloseToolboxEditor += OnCloseEditor;
        }


        /// <summary>
        /// All currently cached vertical scopes.
        /// </summary>
        private static readonly Stack<IDisposable> verticalScopes = new Stack<IDisposable>();

        /// <summary>
        /// All currently cached horizontal scopes.
        /// </summary>
        private static readonly Stack<IDisposable> horizontalScopes = new Stack<IDisposable>();

        /// <summary>
        /// Determines whether we are currently within any Editor's layout scope.
        /// </summary>
        private static bool inEditorLayout;


        private static void OnBeginEditor(Editor editor)
        {
            inEditorLayout = true;
        }

        private static void OnCloseEditor(Editor editor)
        {
            inEditorLayout = false;

            ValidateScope();
        }

        /// <summary>
        /// Validates currently cached layout scopes (vertical and horizontal).
        /// </summary>
        /// <returns>true if scopes were clean.</returns>
        private static bool ValidateScope()
        {
            if (verticalScopes.Count == 0 && horizontalScopes.Count == 0)
            {
                return true;
            }
            else
            {
                ToolboxEditorLog.LogWarning("Invalid layout data. Check if created groups (vertical or horizontal) are properly closed.");

                while (verticalScopes.Count > 0)
                {
                    verticalScopes.Pop().Dispose();
                }

                while (horizontalScopes.Count > 0)
                {
                    horizontalScopes.Pop().Dispose();
                }
            }

            return false;
        }


        internal static void BeginVertical()
        {
            BeginVertical(null);
        }

        internal static void BeginVertical(GUIStyle style, params GUILayoutOption[] options)
        {
            if (!inEditorLayout)
            {
                ToolboxEditorLog.LogWarning("Begin vertical layout group action can be executed only within the Toolbox Editor.");
                return;
            }

            var scope = style == null
                ? new EditorGUILayout.VerticalScope(options)
                : new EditorGUILayout.VerticalScope(style, options);
            verticalScopes.Push(scope);
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


        internal static void BeginHorizontal()
        {
            BeginHorizontal(null);
        }

        internal static void BeginHorizontal(GUIStyle style, params GUILayoutOption[] options)
        {
            if (!inEditorLayout)
            {
                ToolboxEditorLog.LogWarning("Begin horizontal layout group action can be executed only within the Toolbox Editor.");
                return;
            }

            if (horizontalScopes.Count > 0)
            {
                ToolboxEditorLog.LogWarning("Nested horizontal layout groups are not supported.");
                return;
            }

            var scope = style == null
                ? new EditorGUILayout.HorizontalScope(options)
                : new EditorGUILayout.HorizontalScope(style, options);
            horizontalScopes.Push(scope);
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