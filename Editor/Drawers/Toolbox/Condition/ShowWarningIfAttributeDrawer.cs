using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class ShowWarningIfAttributeDrawer : ComparisonAttributeDrawer<ShowWarningIfAttribute>
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, ShowWarningIfAttribute attribute)
        {
            var result = base.OnGuiValidateSafe(property, attribute);
            if (result == PropertyCondition.Disabled)
            {
                EditorGUILayout.HelpBox(attribute.Message, MessageType.Warning);
                result = attribute.DisableField 
                    ? PropertyCondition.Disabled 
                    : PropertyCondition.Valid;
            }

            return result;
        }

        protected override PropertyCondition OnComparisonResult(bool result)
        {
            return result ? PropertyCondition.Disabled : PropertyCondition.Valid;
        }
    }
}