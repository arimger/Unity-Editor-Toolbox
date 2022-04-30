using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class HelpAttributeDrawer : ToolboxDecoratorDrawer<HelpAttribute>
    {
        protected override void OnGuiBeginSafe(HelpAttribute attribute)
        {
            EditorGUILayout.HelpBox(attribute.Text, (MessageType)attribute.Type);
        }
    }
}