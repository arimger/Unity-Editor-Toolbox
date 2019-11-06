using System;
using System.Collections.Generic;

public static class TypeExtensions
{
    public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }
            toCheck = toCheck.BaseType;
        }

        return false;
    }

    public static bool IsAssignableFromRawGeneric(this Type generic, Type toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }
            toCheck = toCheck.BaseType;
        }

        return false;
    }

    public static List<Type> GetAllChildClasses(this Type baseType, bool allowAbstract = false)
    {
        var types = new List<Type>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var isSubclass = baseType.IsGenericType 
            ? (Func<Type, bool>)
              ((type) => type.IsSubclassOfRawGeneric(baseType))
            : ((type) => type.IsSubclassOf(baseType));

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!isSubclass(type) || !type.IsVisible || (!allowAbstract && type.IsAbstract))
                {
                    continue;
                }

                types.Add(type);
            }
        }

        return types;
    }
}