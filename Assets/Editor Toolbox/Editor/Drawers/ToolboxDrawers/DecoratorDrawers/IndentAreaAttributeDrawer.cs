using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class IndentAreaAttributeDrawer : ToolboxDecoratorDrawer<IndentAreaAttribute>
    {
        protected override void OnGuiBeginSafe(IndentAreaAttribute attribute)
        {
            EditorGUI.indentLevel += attribute.IndentLevelChange;
        }

        protected override void OnGuiCloseSafe(IndentAreaAttribute attribute)
        {
            EditorGUI.indentLevel -= attribute.IndentLevelChange;
        }
    }
}