namespace Toolbox.Editor.Drawers.Reflection
{
    internal interface IValueExtractor
    {
        public object GetValue(string source, object declaringObject);
        public bool TryGetValue(string source, object declaringObject, out object value);
    }
}