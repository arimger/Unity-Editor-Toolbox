using System;

using static UnityEditor.EditorGUI;

namespace Toolbox.Editor.Internal
{
    internal class ZeroIndentScope : IDisposable
    {
        private readonly int nextIndent = 0;
        private readonly int prevIndent = 0;

        public ZeroIndentScope()
        {
            prevIndent = indentLevel;
            indentLevel = nextIndent;
        }

        public void Dispose()
        {
            indentLevel = prevIndent;
        }
    }
}