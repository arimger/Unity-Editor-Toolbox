using System;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Fixed version of the <see cref="EditorGUI.DisabledScope"/>.
    /// </summary>
    internal class DisabledScope : IDisposable
    {
        private bool wasEnabled;

        public DisabledScope(bool isEnabled)
        {
            Prepare(isEnabled);
        }

        public void Prepare(bool isEnabled)
        {
            wasEnabled = GUI.enabled;
            GUI.enabled = isEnabled;
        }

        public void Dispose()
        {
            GUI.enabled = wasEnabled;
        }
    }
}
