using System.Collections.Generic;

namespace Toolbox.Editor.Drawers
{
    public static class ValueComparisonHelper
    {
        static ValueComparisonHelper()
        {
            comparers = new List<ValueComparerBase>()
            {
                new BooleanComparer(),
                new IntegerComparer(),
                new FloatComparer(),
                new StringComparer(),
                new ObjectComparer()
            };
        }

        private readonly static List<ValueComparerBase> comparers;


        private static ValueComparerBase GetComparer(object sourceValue, object valueToMatch, ValueComparisonMethod method)
        {
            foreach (var comparer in comparers)
            {
                if (comparer.IsValidSource(sourceValue) &&
                    comparer.IsValidTarget(valueToMatch) &&
                    comparer.IsValidMethod(method))
                {
                    return comparer;
                }
            }

            return null;
        }


        public static bool TryCompare(object sourceValue, object valueToMatch, out bool result)
        {
            return TryCompare(sourceValue, valueToMatch, ValueComparisonMethod.Equal, out result);
        }

        public static bool TryCompare(object sourceValue, object valueToMatch, ValueComparisonMethod method, out bool result)
        {
            var comparer = GetComparer(sourceValue, valueToMatch, method);
            if (comparer == null)
            {
                result = false;
                return false;
            }

            result = comparer.Compare(sourceValue, valueToMatch, method);
            return true;
        }
    }
}