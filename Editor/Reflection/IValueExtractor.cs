namespace Toolbox.Editor.Reflection
{
    internal interface IValueExtractor
    {
        bool TryGetValue(string source, object declaringObject, out object value);
    }
}