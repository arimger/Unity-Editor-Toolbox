using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ComparisonAttributeDrawer<T> : ToolboxConditionDrawer<T> where T : ComparisonAttribute
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, T attribute)
        {
            var propertyToCheck = property.GetSibiling(attribute.ComparedPropertyName);
            if (propertyToCheck == null)
            {
                ToolboxEditorLog.PropertyNotFoundWarning(property, attribute.ComparedPropertyName);
                return PropertyCondition.Valid;
            }

            //TODO: validate 'propertyToCheck' type with 'attribute.TargetConditionValue'

            switch (propertyToCheck.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return OnComparisonResult(propertyToCheck.intValue.Equals(attribute.TargetConditionValue));
                case SerializedPropertyType.Boolean:
                    return OnComparisonResult(propertyToCheck.boolValue.Equals(attribute.TargetConditionValue));
                case SerializedPropertyType.Float:
                    return OnComparisonResult(propertyToCheck.floatValue.Equals(attribute.TargetConditionValue));
                case SerializedPropertyType.String:
                    return OnComparisonResult(propertyToCheck.stringValue.Equals(attribute.TargetConditionValue));
                case SerializedPropertyType.ObjectReference:
                    var expectedValue = (bool)attribute.TargetConditionValue;
                    return OnComparisonResult(propertyToCheck.objectReferenceValue == expectedValue);
                case SerializedPropertyType.Enum:
                    return OnComparisonResult(propertyToCheck.intValue.Equals((int)attribute.TargetConditionValue));
                default:
                    ToolboxEditorLog.TypeNotSupportedWarning(property, propertyToCheck.type);
                    return PropertyCondition.Valid;
            }
        }

        protected abstract PropertyCondition OnComparisonResult(bool result);
    }
}