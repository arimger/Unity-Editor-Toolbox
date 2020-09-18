using System;

using UnityEditor;

namespace Toolbox.Editor.Internal
{
    internal class ZeroIndentScope : IDisposable
    {
        private readonly int nextIndent = 0;
        private readonly int prevIndent = 0;

        public ZeroIndentScope()
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