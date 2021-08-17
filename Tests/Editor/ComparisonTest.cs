using NUnit.Framework;

namespace Toolbox.Editor.Tests
{
    using Toolbox.Editor.Drawers;

    public class ComparisonTest
    {
        [TestCase("stringValue", "stringValue", ValueComparisonMethod.Equal, true)]
        [TestCase("stringValue", "s s ta d", ValueComparisonMethod.Equal, false)]
        [TestCase("stringValue", "stringValue", ValueComparisonMethod.Greater, false)]
        [TestCase(2, 1, ValueComparisonMethod.Greater, true)]
        [TestCase(1, 1, ValueComparisonMethod.Equal, true)]
        [TestCase(-20, 0, ValueComparisonMethod.Less, true)]
        [TestCase(0, 0, ValueComparisonMethod.GreaterEqual, true)]
        [TestCase(1, -1, ValueComparisonMethod.Equal, false)]
        [TestCase(1.0f, 1.0f, ValueComparisonMethod.Equal, true)]
        [TestCase(2.0f, 1.99f, ValueComparisonMethod.Greater, true)]
        [TestCase(-0.1f, 1.99f, ValueComparisonMethod.Less, true)]
        [TestCase(-0.5f, -0.6f, ValueComparisonMethod.Less, false)]
        [TestCase(true, false, ValueComparisonMethod.Equal, false)]
        [TestCase(false, false, ValueComparisonMethod.Equal, true)]
        [TestCase(true, true, ValueComparisonMethod.Equal, true)]
        [TestCase(true, true, ValueComparisonMethod.LessEqual, false)]
        public void TestComparisonPass(object targetValue, object sourceValue, ValueComparisonMethod method, bool expected)
        {
            ValueComparisonHelper.TryCompare(targetValue, sourceValue, method, out var actual);
            Assert.AreEqual(expected, actual);
        }
    }
}