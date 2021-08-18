namespace Toolbox.Editor.Drawers
{
    public class FieldValueExtractor : IValueExtractor
    {
        public bool TryGetValue(string source, object declaringObject, out object value)
        {
            value = default;
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }

            var type = declaringObject.GetType();
            var info = type.GetField(source, ReflectionUtility.allBindings);
            if (info == null)
            {
                return false;
            }

            value = info.GetValue(declaringObject);
            return true;
        }
    }
}