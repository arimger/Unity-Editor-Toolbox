using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class EndHorizontalAttributeDrawer : ToolboxDecoratorDrawer<EndHorizontalAttribute>
    {
        protected override void OnGuiCloseSafe(EndHorizontalAttribute attribute)
        {
            //end horizontal group
            ToolboxLayoutHelper.CloseHorizontal();

            //restore label & field 
            EditorGUIUtility.labelWidth = 0.0f;
            EditorGUIUtility.fieldWidth = 0.0f;
        }
    }
}