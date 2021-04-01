using System;

using UnityEditor;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Stores and restores <see cref="EditorGUIUtility.labelWidth"/> and <see cref="EditorGUIUtility.fieldWidth"/>.
    /// </summary>
    internal class FixedFieldsScope : IDisposable
    {
        private readonly float labelWidth;
        private readonly float fieldWidth;

        public FixedFieldsScope()
        {
            labelWidth = EditorGUIUtility.labelWidth;
            fieldWidth = EditorGUIUtility.fieldWidth;
        }

        public void Dispose()
        {
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUIUtility.fieldWidth = fieldWidth;
        }
    }
}