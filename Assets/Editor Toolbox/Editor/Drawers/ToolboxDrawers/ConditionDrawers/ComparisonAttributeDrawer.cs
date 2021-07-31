using System;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Reflection;

    public abstract class ComparisonAttributeDrawer<T> : ToolboxConditionDrawer<T> where T : ComparisonAttribute
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, T attribute)
        {
            var declaringObject = property.GetDeclaringObject();
            if (!ValueExtractionUtility.TryGetValue(attribute.PropertyName, declaringObject, out var value))
            {
                ToolboxEditorLog.PropertyNotFoundWarning(property, attribute.PropertyName);
                return PropertyCondition.Valid;
            }

            //TODO: validate 'propertyToCheck' type with 'attribute.ValueToMatch'
            //TODO: test masks
            //TODO: more types

            var result = true;
            switch (value)
            {
                case Enum v:
                    result = ComparisionHelper.CheckEnum(v, attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case int v:
                    result = ComparisionHelper.CheckInteger(v, attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case bool v:
                    result = ComparisionHelper.CheckBoolean(v, attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case float v:
                    result = ComparisionHelper.CheckFloat(v, attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case string v:
                    result = ComparisionHelper.CheckString(v, attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case Object v:
                    result = ComparisionHelper.CheckBoolean(v, attribute.ValueToMatch, attribute.TestMethod);
                    break;
                default:
                    ToolboxEditorLog.TypeNotSupportedWarning(property, value.GetType());
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


            internal static bool CheckInteger(int value, object valueToMatch, ComparisionTestMethod testMethod)
            {
                switch (testMethod)
                {
                    case ComparisionTestMethod.Equal:
                        return value == (int)valueToMatch;
                    case ComparisionTestMethod.Greater:
                        return value > (int)valueToMatch;
                    case ComparisionTestMethod.Less:
                        return value < (int)valueToMatch;
                    case ComparisionTestMethod.GreaterEqual:
                        return value >= (int)valueToMatch;
                    case ComparisionTestMethod.LessEqual:
                        return value <= (int)valueToMatch;
                    case ComparisionTestMethod.Mask:
                        return (value & (int)valueToMatch) == (int)valueToMatch;
                    default:
                        return false;
                }
            }

            internal static bool CheckBoolean(bool value, object valueToMatch, ComparisionTestMethod testMethod)
            {
                switch (testMethod)
                {
                    case ComparisionTestMethod.Equal:
                        return value.Equals(valueToMatch);
                    default:
                        LogMethodNotSupported(testMethod, valueToMatch);
                        return false;
                }
            }

            internal static bool CheckFloat(float value, object valueToMatch, ComparisionTestMethod testMethod)
            {
                switch (testMethod)
                {
                    case ComparisionTestMethod.Equal:
                        return value == (float)valueToMatch;
                    case ComparisionTestMethod.Greater:
                        return value > (float)valueToMatch;
                    case ComparisionTestMethod.Less:
                        return value < (float)valueToMatch;
                    case ComparisionTestMethod.GreaterEqual:
                        return value >= (float)valueToMatch;
                    case ComparisionTestMethod.LessEqual:
                        return value <= (float)valueToMatch;
                    default:
                        LogMethodNotSupported(testMethod, valueToMatch);
                        return false;
                }
            }

            internal static bool CheckString(string value, object valueToMatch, ComparisionTestMethod testMethod)
            {
                switch (testMethod)
                {
                    case ComparisionTestMethod.Equal:
                        return value.Equals(valueToMatch);
                    default:
                        LogMethodNotSupported(testMethod, valueToMatch);
                        return false;
                }
            }

            internal static bool CheckObject(Object value, object valueToMatch, ComparisionTestMethod testMethod)
            {
                switch (testMethod)
                {
                    case ComparisionTestMethod.Equal:
                        return value == (bool)valueToMatch;
                    default:
                        LogMethodNotSupported(testMethod, valueToMatch);
                        return false;
                }
            }

            internal static bool CheckEnum(Enum value, object valueToMatch, ComparisionTestMethod testMethod)
            {
                return CheckInteger(Convert.ToInt32(value), valueToMatch, testMethod);
            }
        }
    }
}