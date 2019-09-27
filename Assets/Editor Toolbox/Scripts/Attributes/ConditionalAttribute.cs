namespace UnityEngine
{
    public abstract class ConditionalAttribute : PropertyAttribute
    {
        public ConditionalAttribute(string propertyToCheck, object compareValue = null)
        {
            PropertyToCheck = propertyToCheck;
            CompareValue = compareValue;
        }

        public string PropertyToCheck { get; private set; }

        public object CompareValue { get; private set; }
    }
}