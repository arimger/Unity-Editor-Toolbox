using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class EndHorizontalGroupAttributeDrawer : ToolboxDecoratorDrawer<EndHorizontalGroupAttribute>
    {
        protected override void OnGuiCloseSafe(EndHorizontalGroupAttribute attribute)
        {
            ToolboxLayoutHandler.CloseHorizontal();
            EditorGUILayout.EndScrollView();
            ToolboxLayoutHandler.CloseVertical();

            EditorGUIUtility.labelWidth = 0.0f;
            EditorGUIUtility.fieldWidth = 0.0f;
        }
    }
}