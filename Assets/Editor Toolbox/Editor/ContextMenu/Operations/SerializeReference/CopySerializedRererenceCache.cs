using System;

namespace Toolbox.Editor.ContextMenu.Operations
{
    internal class CopySerializedRererenceCache
    {
        public CopySerializedRererenceCache(Type referenceType, string data)
        {
            ReferenceType = referenceType;
            Data = data;
        }

        public Type ReferenceType { get; }
        public string Data { get; }
    }
}