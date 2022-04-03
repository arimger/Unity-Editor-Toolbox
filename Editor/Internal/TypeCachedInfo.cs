using System;
using System.Collections.Generic;

namespace Toolbox.Editor.Internal
{
    public class TypeCachedInfo
    {
        private readonly string[] options;
        private readonly List<Type> types;


        public TypeCachedInfo(List<Type> types)
        {
            this.types = types;
            var count = types.Count;
            options = new string[count];
            for (var i = 0; i < count; i++)
            {
                var type = types[i];
                var name = type.FullName;
                options[i] = name;
            }
        }


        public int IndexOf(Type type)
        {
            return types.IndexOf(type);
        }
    }
}