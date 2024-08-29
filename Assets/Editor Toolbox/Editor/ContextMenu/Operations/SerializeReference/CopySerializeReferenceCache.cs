using System;

namespace Toolbox.Editor.ContextMenu.Operations
{
    internal class CopySerializeReferenceCache
    {
        public CopySerializeReferenceCache(Type referenceType, string data)
        {
            ReferenceType = referenceType;
            Data = data;
        }

        public Type ReferenceType { get; }
        public string Data { get; }
    }
}