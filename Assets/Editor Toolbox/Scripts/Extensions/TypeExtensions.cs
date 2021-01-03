using System;
using System.Collections.Generic;

namespace Toolbox
{
    public static class TypeExtensions
    {
        public static bool IsSubclassOfGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var current = toCheck.IsGenericType
                    ? toCheck.GetGenericTypeDefinition()
                    : toCheck;
                if (generic == current)
                {
                    return true;
                }

                toCheck = toCheck.BaseType;
            }

            return false;
        }

        public static bool IsAssignableFromGeneric(this Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var current = toCheck.IsGenericType
                    ? toCheck.GetGenericTypeDefinition()
                    : toCheck;
                if (generic == current)
                {
                    return true;
                }

                toCheck = toCheck.BaseType;
            }

            return false;
        }

        public static List<Type> GetAllChildClasses(this Type baseType, bool allowAbstract = false, bool ignoreObsolete = true)
        {
            var types = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var isSubclass = baseType.IsGenericType
                ? (Func<Type, bool>)
                  ((type) => type.IsSubclassOfGeneric(baseType))
                : ((type) => type.IsSubclassOf(baseType));

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!isSubclass(type) || !type.IsVisible || (!allowAbstract && type.IsAbstract) || 
                        (ignoreObsolete && Attribute.IsDefined(type, typeof(ObsoleteAttribute))))
                    {
                        continue;
                    }

                    types.Add(type);
                }
            }

            return types;
        }
    }
}