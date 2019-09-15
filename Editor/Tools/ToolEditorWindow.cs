using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Tools
{
    public abstract class ToolEditorWindow : EditorWindow
    {
        protected virtual void OnEnable()
        { }

        protected virtual void OnDisable()
        { }

        protected virtual void OnGUI()
        {
            GUILayout.Label("Tool Editor", EditorStyles.centeredGreyMiniLabel);
        }
    }
}