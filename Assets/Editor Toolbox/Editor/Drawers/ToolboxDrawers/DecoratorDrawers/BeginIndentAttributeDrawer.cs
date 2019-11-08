using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class BeginIndentAttributeDrawer : ToolboxDecoratorDrawer<BeginIndentAttribute>
    {
        public override void OnGuiBegin(BeginIndentAttribute attribute)
        {
            EditorGUI.indentLevel += attribute.IndentToAdd;
        }
    }
}