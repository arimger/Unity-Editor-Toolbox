namespace Toolbox.Editor.Drawers
{
    internal interface IValueExtractor
    {
        bool TryGetValue(string source, object declaringObject, out object value);
    }
}