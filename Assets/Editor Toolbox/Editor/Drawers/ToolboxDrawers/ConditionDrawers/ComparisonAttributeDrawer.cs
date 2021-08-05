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

            //var valueToMatch = attribute.ValueToMatch;
            //TODO: what about UnityEngine.Object
            //if (valueToMatch.GetType() != value.GetType())
            //{
            //    ToolboxEditorLog.AttributeUsageWarning(attribute, property, "Given valueToMatch type missmatched with targeted source.");
            //    return PropertyCondition.Valid;
            //}

            //TODO: more types
            // - long
            // - ulong
            // - uint
            // - double
            // https://docs.microsoft.com/pl-pl/dotnet/csharp/language-reference/builtin-types/built-in-types

            var typeCode = Type.GetTypeCode(value.GetType());
            var result = true;
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    result = ComparisionHelper.CheckInteger(Convert.ToInt32(value), attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    result = ComparisionHelper.CheckFloat(Convert.ToSingle(value), attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case TypeCode.String:
                    result = ComparisionHelper.CheckString(Convert.ToString(value), attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case TypeCode.Boolean:
                    result = ComparisionHelper.CheckBoolean(Convert.ToBoolean(value), attribute.ValueToMatch, attribute.TestMethod);
                    break;
                case TypeCode.Object:
                    if (value is Object obj)
                    {
                        result = ComparisionHelper.CheckBoolean(obj, attribute.ValueToMatch, attribute.TestMethod);
                    }
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
                var realValueToMatch = Convert.ToInt32(valueToMatch);
                switch (testMethod)
                {
                    case ComparisionTestMethod.Equal:
                        return value == realValueToMatch;
                    case ComparisionTestMethod.Greater:
                        return value > realValueToMatch;
                    case ComparisionTestMethod.Less:
                        return value < realValueToMatch;
                    case ComparisionTestMethod.GreaterEqual:
                        return value >= realValueToMatch;
                    case ComparisionTestMethod.LessEqual:
                        return value <= realValueToMatch;
                    case ComparisionTestMethod.Mask:
                        return (value & realValueToMatch) == realValueToMatch;
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
                var realValueToMatch = Convert.ToSingle(valueToMatch);
                switch (testMethod)
                {
                    case ComparisionTestMethod.Equal:
                        return value == realValueToMatch;
                    case ComparisionTestMethod.Greater:
                        return value > realValueToMatch;
                    case ComparisionTestMethod.Less:
                        return value < realValueToMatch;
                    case ComparisionTestMethod.GreaterEqual:
                        return value >= realValueToMatch;
                    case ComparisionTestMethod.LessEqual:
                        return value <= realValueToMatch;
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
        }
    }
}