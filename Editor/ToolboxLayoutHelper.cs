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
            //ToolboxEditor.OnCloseToolboxEditor is a quite useful event which can be used to 
            //validate layout data. Actually it should be used to draw additional information
            //into the target Editor but in this case we will check previously created scopes

            ToolboxEditor.OnBeginToolboxEditor += OnBeginEditor;
            ToolboxEditor.OnBreakToolboxEditor += OnBreakEditor;
            ToolboxEditor.OnCloseToolboxEditor += OnCloseEditor;
        }


        /// <summary>
        /// Determines whether we are currently within any Editor's layout scope.
        /// </summary>
        private static bool inEditorLayout;
        /// <summary>
        /// Determines whether layout was prematurely exited by the external API.
        /// </summary>
        private static bool isExitedLayout;

        private static int vLayoutClips;
        private static int hLayoutClips;


        private static void OnBeginEditor(Editor editor)
        {
            inEditorLayout = true;
        }

        private static void OnBreakEditor(Editor editor)
        {
            isExitedLayout = true;
        }

        private static void OnCloseEditor(Editor editor)
        {
            ValidateScopes();

            inEditorLayout = false;
            isExitedLayout = false;
        }

        /// <summary>
        /// Validates currently cached layout scopes (vertical and horizontal).
        /// </summary>
        /// <returns>true if scopes were clean.</returns>
        private static bool ValidateScopes()
        {
            if (vLayoutClips > 0 || hLayoutClips > 0)
            {
                if (!isExitedLayout)
                {
                    ToolboxEditorLog.LogWarning("Invalid layout data. Check if created groups (vertical or horizontal) are properly closed.");
                }

                while (vLayoutClips > 0)
                {
                    CloseVertical();
                }

                while (hLayoutClips > 0)
                {
                    CloseHorizontal();
                }

                return false;
            }
            else
            {
                return true;
            }
        }


        internal static void BeginVertical()
        {
            BeginVertical(new GUIStyle());
        }

        internal static void BeginVertical(GUIStyle style, params GUILayoutOption[] options)
        {
            if (!inEditorLayout)
            {
                ToolboxEditorLog.LogWarning("Begin vertical layout group action can be executed only within the Toolbox Editor.");
                return;
            }

            vLayoutClips++;
            EditorGUILayout.BeginVertical(style, options);
        }

        internal static void CloseVertical()
        {
            if (vLayoutClips == 0)
            {
                ToolboxEditorLog.LogWarning("There is no a vertical group to end. Call will be ignored.");
                return;
            }

            vLayoutClips--;
            EditorGUILayout.EndVertical();
        }

        internal static void BeginHorizontal()
        {
            BeginHorizontal(new GUIStyle());
        }

        internal static void BeginHorizontal(GUIStyle style, params GUILayoutOption[] options)
        {
            if (!inEditorLayout)
            {
                ToolboxEditorLog.LogWarning("Begin horizontal layout group action can be executed only within the Toolbox Editor.");
                return;
            }

            if (hLayoutClips > 0)
            {
                ToolboxEditorLog.LogWarning("Nested horizontal layout groups are not supported.");
                return;
            }

            hLayoutClips++;
            EditorGUILayout.BeginHorizontal(style, options);
        }

        internal static void CloseHorizontal()
        {
            if (hLayoutClips == 0)
            {
                ToolboxEditorLog.LogWarning("There is no a horizontal group to end. Call will be ignored.");
                return;
            }

            hLayoutClips--;
            EditorGUILayout.EndHorizontal();
        }
    }
}