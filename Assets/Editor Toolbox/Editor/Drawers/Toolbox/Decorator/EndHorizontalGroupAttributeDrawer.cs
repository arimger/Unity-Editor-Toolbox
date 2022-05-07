using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class EndHorizontalGroupAttributeDrawer : ToolboxDecoratorDrawer<EndHorizontalGroupAttribute>
    {
        protected override void OnGuiCloseSafe(EndHorizontalGroupAttribute attribute)
        {
            ToolboxLayoutHelper.CloseHorizontal();
            EditorGUILayout.EndScrollView();
            ToolboxLayoutHelper.CloseVertical();

            EditorGUIUtility.labelWidth = 0.0f;
            EditorGUIUtility.fieldWidth = 0.0f;
        }
    }
}