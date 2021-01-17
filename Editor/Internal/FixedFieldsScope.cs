using System;

using UnityEditor;

namespace Toolbox.Editor.Internal
{
    internal class FixedFieldsScope : IDisposable
    {
        private float labelWidth;
        private float fieldWidth;

        public FixedFieldsScope()
        {
            Prepare();
        }

        public void Prepare()
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