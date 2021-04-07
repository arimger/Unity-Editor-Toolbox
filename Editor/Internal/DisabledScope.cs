using System;

using UnityEngine;

namespace Toolbox.Editor.Internal
{
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
