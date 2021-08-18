using System;
using System.Reflection;

namespace Toolbox.Editor.Drawers
{
    public class MethodValueExtractor : IValueExtractor
    {
        public bool TryGetValue(string source, object declaringObject, out object value)
        {
            value = default;
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }

            var type = declaringObject.GetType();
            var info = type.GetMethod(source, ReflectionUtility.allBindings, null, CallingConventions.Any, new Type[0], null);
            if (info == null)
            {
                return false;
            }

            value = info.Invoke(declaringObject, null);
            return true;
        }
    }
}