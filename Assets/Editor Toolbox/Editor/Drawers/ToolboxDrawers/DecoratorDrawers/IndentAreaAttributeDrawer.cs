using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class IndentAreaAttributeDrawer : ToolboxDecoratorDrawer<IndentAreaAttribute>
    {
        protected override void OnGuiBeginSafe(IndentAreaAttribute attribute)
        {
            EditorGUI.indentLevel += attribute.IndentLevelChange;
        }

        protected override void OnGuiEndSafe(IndentAreaAttribute attribute)
        {
            EditorGUI.indentLevel -= attribute.IndentLevelChange;
        }
    }
}