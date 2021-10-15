using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class DynamicHelpAttributeDrawer : ToolboxDecoratorDrawer<DynamicHelpAttribute>
    {
        protected override void OnGuiBeginSafe(DynamicHelpAttribute attribute)
        {
            var sourceHandle = attribute.SourceHandle;
            var targetObjects = InspectorUtility.CurrentTargetObjects;
            if (ValueExtractionHelper.TryGetValue(sourceHandle, targetObjects, out var value, out var hasMixedValues))
            {
                var messageText = hasMixedValues ? "-" : value?.ToString();
                var messageType = (MessageType)attribute.Type;
                EditorGUILayout.HelpBox(messageText, messageType);
                return;
            }

            var targetType = targetObjects[0].GetType();
            ToolboxEditorLog.MemberNotFoundWarning(attribute, targetType, sourceHandle);
        }
    }
}