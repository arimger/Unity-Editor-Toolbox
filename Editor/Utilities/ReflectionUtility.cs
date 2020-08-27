using System.Reflection;

namespace Toolbox.Editor
{
    public static class ReflectionUtility
    {
        public const BindingFlags allPossibleFieldsBinding =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;
    }
}