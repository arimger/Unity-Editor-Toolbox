namespace Toolbox.Editor.Reflection
{
    internal interface IValueExtractor
    {
        public bool TryGetValue(string source, object declaringObject, out object value);
    }
}