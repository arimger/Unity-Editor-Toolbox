using System;
using System.Collections.Generic;

namespace Toolbox
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Generic equivalent of the <see cref="Type.IsSubclassOf(Type)"/>.
        /// </summary>
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

        /// <summary>
        /// Generic equivalent of the <see cref="Type.IsAssignableFrom(Type)"/>.
        /// </summary>
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

        /// <summary>
        /// Returns all child classes of the given <see cref="Type"/>.
        /// Additionally abstract and obsolete types may be ignored.
        /// </summary>
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

        /// <summary>
        /// Returns element type of the array or <see cref="List{T}"/>.
        /// </summary>
        public static Type GetEnumeratedType(this Type type)
        {
            var elementType = type.GetElementType();
            if (null != elementType)
            {
                return elementType;
            }

            var genericTypes = type.GetGenericArguments();
            if (genericTypes.Length > 0)
            {
                return genericTypes[0];
            }

            return null;
        }
    }
}