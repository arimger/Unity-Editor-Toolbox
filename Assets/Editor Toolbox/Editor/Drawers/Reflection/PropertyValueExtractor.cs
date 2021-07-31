namespace Toolbox.Editor.Drawers.Reflection
{
    public class PropertyValueExtractor : IValueExtractor
    {
        public object GetValue(string source, object declaringObject)
        {
            //TODO: bindings
            var type = declaringObject.GetType();
            var info = type.GetProperty(source);
            return info.GetValue(declaringObject);
        }

        public bool TryGetValue(string source, object declaringObject, out object value)
        {
            throw new System.NotImplementedException();
        }
    }
}