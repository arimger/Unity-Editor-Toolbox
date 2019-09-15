using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class EndHorizontalAttributeDrawer : ToolboxAreaDrawer<EndHorizontalAttribute>
    {
        public override void OnGuiEnd(EndHorizontalAttribute attribute)
        {
            EditorGUILayout.EndHorizontal();
        }
    }
}