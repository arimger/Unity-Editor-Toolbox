using System;
using System.Collections.Generic;

namespace Toolbox.Editor.ContextMenu.Operations
{
    /// <summary>
    /// Cache created by the copy operation.
    /// All properties are based on data from the source <see cref="UnityEditor.SerializedProperty"/>.
    /// </summary>
    internal class CopySerializeReferenceCache
    {
        public CopySerializeReferenceCache(Type referenceType, IReadOnlyList<CopySerializeReferenceEntry> entires)
        {
            ReferenceType = referenceType;
            Entries = entires;
        }

        /// <summary>
        /// Base managed reference type of the source <see cref="UnityEditor.SerializedProperty"/>.
        /// </summary>
        public Type ReferenceType { get; }
        public IReadOnlyList<CopySerializeReferenceEntry> Entries { get; }
        public bool IsArrayCopy => Entries != null && Entries.Count > 1;
    }
}