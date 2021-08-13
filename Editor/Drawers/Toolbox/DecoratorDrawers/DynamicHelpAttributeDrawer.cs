using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class DynamicHelpAttributeDrawer : ToolboxDecoratorDrawer<DynamicHelpAttribute>
    {
        protected override void OnGuiBeginSafe(DynamicHelpAttribute attribute)
        {
            //TODO:
            //if (ValueExtractionHelper.TryGetValue(attribute.SourceHandle,))
            EditorGUILayout.HelpBox(attribute.SourceHandle, (MessageType)attribute.Type);
        }
    }
}