using System;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ComparisonAttributeDrawer<T> : ToolboxConditionDrawer<T> where T : ComparisonAttribute
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, T attribute)
        {
            if (attribute.SourceHandles == null || attribute.ValueToMatches == null)
            {
                return PropertyCondition.Valid;
            }
            
            var validLength = Math.Min(attribute.SourceHandles.Length, attribute.ValueToMatches.Length);
            if (validLength == 0)
            {
                return PropertyCondition.Valid;
            }

            PropertyCondition[] resultItems = new PropertyCondition[validLength];
            for (int i = 0; i < validLength; i++)
            {
                var sourceHandle = attribute.SourceHandles[i];
                if (!ValueExtractionHelper.TryGetValue(sourceHandle, property, out var value, out var hasMixedValues))
                {
                    ToolboxEditorLog.MemberNotFoundWarning(attribute, property, sourceHandle);
                    resultItems[i] = PropertyCondition.Valid;
                    continue;
                }

                var comparison = (ValueComparisonMethod)attribute.Comparison;
                if (attribute.IndividualComparisons != null && attribute.IndividualComparisons.Length > i)
                {
                    comparison = (ValueComparisonMethod)attribute.IndividualComparisons[i];
                }
                
                var targetValue = attribute.ValueToMatches[i];
                if (!ValueComparisonHelper.TryCompare(value, targetValue, comparison, out var result))
                {
                    ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                        string.Format("Invalid comparison input: source:{0}, target:{1}, method:{2}.",
                            value?.GetType(), targetValue?.GetType(), comparison));
                    resultItems[i] = PropertyCondition.Valid;
                    continue;
                }

                resultItems[i] = OnComparisonResult(hasMixedValues ? false : result);
            }

            // Logic And
            if (attribute.LogicAnd)
            {
                foreach (var resultItem in resultItems)
                {
                    if (resultItem != PropertyCondition.Valid)
                    {
                        return resultItem;
                    }
                }
            }
            // Logic Or
            else
            {
                foreach (var resultItem in resultItems)
                {
                    if (resultItem == PropertyCondition.Valid)
                    {
                        return PropertyCondition.Valid;
                    }
                }

                foreach (var resultItem in resultItems)
                {
                    if (resultItem != PropertyCondition.Valid)
                    {
                        return resultItem;
                    }
                }
            }

            return PropertyCondition.Valid;
        }

        protected abstract PropertyCondition OnComparisonResult(bool result);
    }
}