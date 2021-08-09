using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ComparisonAttributeDrawer<T> : ToolboxConditionDrawer<T> where T : ComparisonAttribute
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, T attribute)
        {
            var sourceName = attribute.PropertyName;
            var declaringObject = property.GetDeclaringObject();
            if (!ValueExtractionHelper.TryGetValue(sourceName, declaringObject, out var value))
            {
                //TODO: new warning
                ToolboxEditorLog.PropertyNotFoundWarning(property, attribute.PropertyName);
                return PropertyCondition.Valid;
            }

            var method = (ValueComparisonMethod)attribute.Comparison;
            var valueToMatch = attribute.ValueToMatch;
            if (!ValueComparisonHelper.TryCompare(value, valueToMatch, method, out var result))
            {
                //TODO: new warning
                ToolboxEditorLog.LogWarning("Cannot compare!");
                return PropertyCondition.Valid;
            }

            return OnComparisonResult(result);
        }

        protected abstract PropertyCondition OnComparisonResult(bool result);
    }
}