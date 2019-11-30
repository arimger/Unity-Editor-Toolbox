using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class EndGroupAttributeDrawer : ToolboxDecoratorDrawer<EndGroupAttribute>
    {
        protected override void OnGuiEndSafe(EndGroupAttribute attribute)
        {          
            EditorGUILayout.EndVertical();
        }
    }
}
