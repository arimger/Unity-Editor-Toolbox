using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class BeginIndentAttributeDrawer : ToolboxAreaDrawer<BeginIndentAttribute>
    {
        public override void OnGuiBegin(BeginIndentAttribute attribute)
        {
            EditorGUI.indentLevel += attribute.IndentToAdd;
        }
    }
}