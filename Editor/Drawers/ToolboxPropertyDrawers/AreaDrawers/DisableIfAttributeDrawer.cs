using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class DisableIfAttributeDrawer : ToolboxAreaDrawer<DisableIfAttribute>
    {
        public override void OnGuiBegin(DisableIfAttribute attribute)
        {
            EditorGUILayout.BeginVertical();
            
        }

        public override void OnGuiEnd(DisableIfAttribute attribute)
        {
            EditorGUILayout.EndVertical();
        }
    }
}