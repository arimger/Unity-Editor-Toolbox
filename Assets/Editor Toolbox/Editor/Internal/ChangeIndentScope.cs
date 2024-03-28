using System;
using UnityEditor;

namespace Toolbox.Editor.Internal
{
    internal class ChangeIndentScope : IDisposable
    {
        private readonly int indentChange;

        public ChangeIndentScope(int indentChange)
        {
            this.indentChange = indentChange;
            Prepare(indentChange);
        }

        public void Prepare(int indentChange)
        {
            EditorGUI.indentLevel += indentChange;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel -= indentChange;
        }
    }
}