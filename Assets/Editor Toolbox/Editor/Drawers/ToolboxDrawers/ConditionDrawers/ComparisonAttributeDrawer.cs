using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ComparisonAttributeDrawer<T> : ToolboxConditionDrawer<T> where T : ComparisonAttribute
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, T attribute)
        {
            var propertyToCheck = property.GetSibiling(attribute.PropertyName);
            if (propertyToCheck == null)
            {
                ToolboxEditorLog.PropertyNotFoundWarning(property, attribute.PropertyName);
                return PropertyCondition.Valid;
            }

            //TODO: validate 'propertyToCheck' type with 'attribute.ValueToMatch'

            switch (propertyToCheck.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return OnComparisonResult(propertyToCheck.intValue.Equals(attribute.ValueToMatch));
                case SerializedPropertyType.Boolean:
                    return OnComparisonResult(propertyToCheck.boolValue.Equals(attribute.ValueToMatch));
                case SerializedPropertyType.Float:
                    return OnComparisonResult(propertyToCheck.floatValue.Equals(attribute.ValueToMatch));
                case SerializedPropertyType.String:
                    return OnComparisonResult(propertyToCheck.stringValue.Equals(attribute.ValueToMatch));
                case SerializedPropertyType.ObjectReference:
                    var expectedValue = (bool)attribute.ValueToMatch;
                    return OnComparisonResult(propertyToCheck.objectReferenceValue == expectedValue);
                case SerializedPropertyType.Enum:
                    return OnComparisonResult(propertyToCheck.intValue.Equals((int)attribute.ValueToMatch));
                default:
                    ToolboxEditorLog.TypeNotSupportedWarning(property, propertyToCheck.type);
                    return PropertyCondition.Valid;
            }
        }

        protected abstract PropertyCondition OnComparisonResult(bool result);
    }
}