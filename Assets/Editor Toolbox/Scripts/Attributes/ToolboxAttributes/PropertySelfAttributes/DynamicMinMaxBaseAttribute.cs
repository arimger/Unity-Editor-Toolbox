namespace UnityEngine
{
    public abstract class DynamicMinMaxBaseAttribute : ToolboxSelfPropertyAttribute
    {
        protected DynamicMinMaxBaseAttribute(string minValueSource, string maxValueSource)
        {
            MinValueSource = minValueSource;
            MaxValueSource = maxValueSource;
        }

        public string MinValueSource { get; protected set; }

        public string MaxValueSource { get; protected set; }
    }
}