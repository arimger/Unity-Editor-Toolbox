using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ComparisonAttributeDrawer<T> : ToolboxConditionDrawer<T> where T : ComparisonAttribute
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, T attribute)
        {
            var declaringObject = property.GetDeclaringObject();

            var sourceName = attribute.SourceHandle;
            if (!ValueExtractionHelper.TryGetValue(sourceName, declaringObject, out var value))
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    string.Format("Source ({0}) not found.", sourceName));
                return PropertyCondition.Valid;
            }

            var comparison = (ValueComparisonMethod)attribute.Comparison;
            var targetValue = attribute.ValueToMatch;
            if (!ValueComparisonHelper.TryCompare(value, targetValue, comparison, out var result))
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    string.Format("Invalid comparison input: source:{0}, target:{1}, method:{2}.",
                    value?.GetType(), targetValue?.GetType(), comparison));
                return PropertyCondition.Valid;
            }

            return OnComparisonResult(result);
        }

        protected abstract PropertyCondition OnComparisonResult(bool result);
    }
}