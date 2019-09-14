using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class EndGroupAttributeDrawer : ToolboxAreaDrawer<EndGroupAttribute>
    {
        public override void OnGuiEnd(EndGroupAttribute attribute)
        {          
            //add additional space between vertical group and last property
            GUILayout.Space(Style.spacing * 2);
            EditorGUILayout.EndVertical();
        }


        private static class Style
        {
            internal static readonly float spacing = 2.5f;
        }
    }
}
