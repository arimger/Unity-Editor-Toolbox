using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class BeginIndentAttributeDrawer : ToolboxDecoratorDrawer<BeginIndentAttribute>
    {
        protected override void OnGuiBeginSafe(BeginIndentAttribute attribute)
        {
            EditorGUI.indentLevel += attribute.IndentToAdd;
        }
    }
}