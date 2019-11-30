using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class EndHorizontalAttributeDrawer : ToolboxDecoratorDrawer<EndHorizontalAttribute>
    {
        private const float minWidthSize = 120.0f;
        private const float minWidthOffset = 40.0f;
        private const float sizeToWidthRatio = 0.45f;


        protected override void OnGuiEndSafe(EndHorizontalAttribute attribute)
        {
            //end horizontal group
            EditorGUILayout.EndHorizontal();
            //restore label width using native values - hard coded and retrieved from source code
            EditorGUIUtility.labelWidth = Mathf.Max(EditorGUIUtility.currentViewWidth * sizeToWidthRatio - minWidthOffset, minWidthSize);
        }
    }
}