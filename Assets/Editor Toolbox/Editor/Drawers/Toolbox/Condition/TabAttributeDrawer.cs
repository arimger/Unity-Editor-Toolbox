using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class TabAttributeDrawer : ToolboxConditionDrawer<TabAttribute>
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, TabAttribute attribute)
        {
            if (TabsCacheManager.TryGetIsTabActive(property, attribute.Tab, out var isTabActive))
            {
                return isTabActive
                    ? PropertyCondition.Valid
                    : PropertyCondition.NonValid;
            }

            ToolboxEditorLog.AttributeUsageWarning(attribute, property, $"Tab '{attribute.Tab}' is not inside Tabs Group.");
            return PropertyCondition.Valid;
        }
    }
}