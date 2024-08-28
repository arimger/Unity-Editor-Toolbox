using System;
using System.Collections;
using System.Collections.Generic;

namespace Toolbox.Editor.Internal.Types
{
    public class TypesCachedCollection : IEnumerable<Type>
    {
        private readonly List<Type> values;

        public TypesCachedCollection() : this(new List<Type>())
        { }

        public TypesCachedCollection(List<Type> values)
        {
            this.values = values;
        }

        public virtual int IndexOf(Type type)
        {
            return values.IndexOf(type);
        }

        public virtual bool Contains(Type type)
        {
            return values.Contains(type);
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }

        public IReadOnlyList<Type> Values => values;

        public static implicit operator List<Type>(TypesCachedCollection collection) => collection.values;
    }
}