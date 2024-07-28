using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class DisableInEditModeAttributeDrawer : ToolboxConditionDrawer<DisableInEditModeAttribute>
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, DisableInEditModeAttribute attribute)
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode ? PropertyCondition.Disabled : PropertyCondition.Valid;
        }
    }
}