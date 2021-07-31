namespace Toolbox.Editor.Reflection
{
    public class FieldValueExtractor : IValueExtractor
    {
        public bool TryGetValue(string source, object declaringObject, out object value)
        {
            var type = declaringObject.GetType();
            var info = type.GetField(source, ReflectionUtility.allBindings);
            if (info == null)
            {
                value = default;
                return false;
            }

            value = info.GetValue(declaringObject);
            return true;
        }
    }
}