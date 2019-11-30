using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class BeginHorizontalAttributeDrawer : ToolboxDecoratorDrawer<BeginHorizontalAttribute>
    {
        protected override void OnGuiBeginSafe(BeginHorizontalAttribute attribute)
        {
            //set new label width for this area
            EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * attribute.LabelToWdithRatio;
            //begin horizontal group
            EditorGUILayout.BeginHorizontal();
        }
    }
}