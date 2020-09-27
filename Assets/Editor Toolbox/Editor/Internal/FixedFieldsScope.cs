using System;

using UnityEditor;

namespace Toolbox.Editor.Internal
{
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