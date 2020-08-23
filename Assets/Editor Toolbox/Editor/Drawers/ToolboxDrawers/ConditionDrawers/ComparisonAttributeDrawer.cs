using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ComparisonAttributeDrawer<T> : ToolboxConditionDrawer<T> where T : ComparisonAttribute
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, T attribute)
        {
            var propertyToCheck = property.serializedObject.FindProperty(attribute.ComparedPropertyName);
            if (propertyToCheck == null)
            {
                ToolboxEditorLog.PropertyNotFoundWarning(property, attribute.ComparedPropertyName);
                return PropertyCondition.Valid;
            }

            //TODO: validate 'propertyToCheck' type with 'attribute.ComparedConditionValue'

            switch (propertyToCheck.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return OnComparisonResult(propertyToCheck.boolValue.Equals(attribute.ComparedConditionValue));
                case SerializedPropertyType.String:
                    return OnComparisonResult(propertyToCheck.stringValue.Equals(attribute.ComparedConditionValue));
                case SerializedPropertyType.Integer:
                    return OnComparisonResult(propertyToCheck.intValue.Equals(attribute.ComparedConditionValue));
                case SerializedPropertyType.Float:
                    return OnComparisonResult(propertyToCheck.floatValue.Equals(attribute.ComparedConditionValue));
                case SerializedPropertyType.Enum:
                    return OnComparisonResult(propertyToCheck.intValue.Equals((int)attribute.ComparedConditionValue));
                default:
                    ToolboxEditorLog.TypeNotSupportedWarning(property, propertyToCheck.type);
                    return PropertyCondition.Valid;
            }
        }

        protected abstract PropertyCondition OnComparisonResult(bool result);
    }
}