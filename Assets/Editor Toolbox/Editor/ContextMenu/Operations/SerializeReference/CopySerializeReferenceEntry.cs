using System;

namespace Toolbox.Editor.ContextMenu.Operations
{
    internal class CopySerializeReferenceEntry
    {
        public CopySerializeReferenceEntry(Type referenceType, string referenceData)
        {
            ReferenceType = referenceType;
            ReferenceData = referenceData;
        }

        public Type ReferenceType { get; }
        public string ReferenceData { get; }
    }
}