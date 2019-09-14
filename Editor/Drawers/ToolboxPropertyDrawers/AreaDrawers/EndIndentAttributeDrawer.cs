using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class EndIndentAttributeDrawer : ToolboxAreaDrawer<EndIndentAttribute>
    {
        public override void OnGuiEnd(EndIndentAttribute attribute)
        {
            EditorGUI.indentLevel -= attribute.IndentToSubtract;
        }
    }
}