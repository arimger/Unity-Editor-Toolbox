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

            var result = true;
            switch (propertyToCheck.propertyType)
            {
                case SerializedPropertyType.Integer:
                    result = ComparisionHelper.CheckInteger(propertyToCheck, attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case SerializedPropertyType.Boolean:
                    result = ComparisionHelper.CheckBoolean(propertyToCheck, attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case SerializedPropertyType.Float:
                    result = ComparisionHelper.CheckFloat(propertyToCheck, attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case SerializedPropertyType.String:
                    result = ComparisionHelper.CheckString(propertyToCheck, attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case SerializedPropertyType.ObjectReference:
                    result = ComparisionHelper.CheckObject(propertyToCheck, attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case SerializedPropertyType.Enum:
                    result = ComparisionHelper.CheckEnum(propertyToCheck, attribute.ValueToMatch, attribute.TestMethod);
                    break;
                default:
                    ToolboxEditorLog.TypeNotSupportedWarning(property, propertyToCheck.type);
                    break;
            }

            return OnComparisonResult(result);
        }

        protected abstract PropertyCondition OnComparisonResult(bool result);


        private static class ComparisionHelper
        {
            private static void LogMethodNotSupported(ComparisionTestMethod testMethod, object valueToMatch)
            {
                ToolboxEditorLog.LogWarning(string.Format("{0} comparision method is not supported for type: {1}.", testMethod, valueToMatch.GetType()));
            }


            internal static bool CheckInteger(SerializedProperty propertyToCheck, object valueToMatch, ComparisionTestMethod testMethod)
            {
                switch (testMethod)
                {
                    case ComparisionTestMethod.Equal:
                        return propertyToCheck.intValue == (int)valueToMatch;
                    case ComparisionTestMethod.Greater:
                        return propertyToCheck.intValue > (int)valueToMatch;
                    case ComparisionTestMethod.Less:
                        return propertyToCheck.intValue < (int)valueToMatch;
                    case ComparisionTestMethod.GreaterEqual:
                        return propertyToCheck.intValue >= (int)valueToMatch;
                    case ComparisionTestMethod.LessEqual:
                        return propertyToCheck.intValue <= (int)valueToMatch;
                    case ComparisionTestMethod.Mask:
                        return (propertyToCheck.intValue & (int)valueToMatch) == (int)valueToMatch;
                    default:
                        return false;
                }
            }

            internal static bool CheckBoolean(SerializedProperty propertyToCheck, object valueToMatch, ComparisionTestMethod testMethod)
            {
                switch (testMethod)
                {
                    case ComparisionTestMethod.Equal:
                        return propertyToCheck.boolValue.Equals(valueToMatch);
                    default:
                        LogMethodNotSupported(testMethod, valueToMatch);
                        return false;
                }
            }

            internal static bool CheckFloat(SerializedProperty propertyToCheck, object valueToMatch, ComparisionTestMethod testMethod)
            {
                switch (testMethod)
                {
                    case ComparisionTestMethod.Equal:
                        return propertyToCheck.floatValue == (float)valueToMatch;
                    case ComparisionTestMethod.Greater:
                        return propertyToCheck.floatValue > (float)valueToMatch;
                    case ComparisionTestMethod.Less:
                        return propertyToCheck.floatValue < (float)valueToMatch;
                    case ComparisionTestMethod.GreaterEqual:
                        return propertyToCheck.floatValue >= (float)valueToMatch;
                    case ComparisionTestMethod.LessEqual:
                        return propertyToCheck.floatValue <= (float)valueToMatch;
                    default:
                        LogMethodNotSupported(testMethod, valueToMatch);
                        return false;
                }
            }

            internal static bool CheckString(SerializedProperty propertyToCheck, object valueToMatch, ComparisionTestMethod testMethod)
            {
                switch (testMethod)
                {
                    case ComparisionTestMethod.Equal:
                        return propertyToCheck.stringValue.Equals(valueToMatch);
                    default:
                        LogMethodNotSupported(testMethod, valueToMatch);
                        return false;
                }
            }

            internal static bool CheckObject(SerializedProperty propertyToCheck, object valueToMatch, ComparisionTestMethod testMethod)
            {
                switch (testMethod)
                {
                    case ComparisionTestMethod.Equal:
                        return propertyToCheck.objectReferenceValue == (bool)valueToMatch;
                    default:
                        LogMethodNotSupported(testMethod, valueToMatch);
                        return false;
                }
            }

            internal static bool CheckEnum(SerializedProperty propertyToCheck, object valueToMatch, ComparisionTestMethod testMethod)
            {
                return CheckInteger(propertyToCheck, valueToMatch, testMethod);
            }
        }
    }
}