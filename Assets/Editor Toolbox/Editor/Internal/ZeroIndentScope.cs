using System;

using UnityEditor;

namespace Toolbox.Editor.Internal
{
    internal class ZeroIndentScope : IDisposable
    {
        private int prevIndent;

        public ZeroIndentScope()
        {
            Prepare(0);
        }

        public void Prepare(int nextIndent)
        {
            prevIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = nextIndent;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel = prevIndent;
        }
    }
}