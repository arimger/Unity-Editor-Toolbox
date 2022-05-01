using System;
using System.Collections;
using System.Collections.Generic;

namespace Toolbox.Editor.Internal
{
    public class TypesCollection : IEnumerable<Type>
    {
        public IEnumerator<Type> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}