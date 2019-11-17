using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class EndGroupAttributeDrawer : ToolboxDecoratorDrawer<EndGroupAttribute>
    {
        public override void OnGuiEnd(EndGroupAttribute attribute)
        {          
            EditorGUILayout.EndVertical();
        }
    }
}
