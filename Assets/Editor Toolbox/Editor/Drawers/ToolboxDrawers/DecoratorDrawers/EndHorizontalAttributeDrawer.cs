using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class EndHorizontalAttributeDrawer : ToolboxDecoratorDrawer<EndHorizontalAttribute>
    {
        protected override void OnGuiEndSafe(EndHorizontalAttribute attribute)
        {
            //end horizontal group
            ToolboxLayoutHelper.EndHorizontal();

            //restore label width 
            EditorGUIUtility.labelWidth = 0.0f;
            EditorGUIUtility.fieldWidth = 0.0f;
        }
    }
}