using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.ContextMenu
{
    internal interface IContextMenuOperation
    {
        bool IsVisible(SerializedProperty property);
        bool IsEnabled(SerializedProperty property);
        void Perform(SerializedProperty property);

        GUIContent Label { get; }
    }
}