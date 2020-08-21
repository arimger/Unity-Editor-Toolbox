using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class HelpAreaAttributeDrawer : ToolboxDecoratorDrawer<HelpAreaAttribute>
    {
        protected override void OnGuiBeginSafe(HelpAreaAttribute attribute)
        {
            EditorGUILayout.HelpBox(attribute.Text, (MessageType)attribute.Type);
        }
    }
}
