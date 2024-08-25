using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Core
{
    [FilePath("ProjectSettings/ToolboxSettings.assets", FilePathAttribute.Location.ProjectFolder)]
    internal class ToolboxSettings : ScriptableSingleton<ToolboxSettings>
    {
        //TODO: handle settings from different systems
        [SerializeField]
        private bool useToolboxHierarchy;
        [SerializeField]
        private bool drawHorizontalLines;
        [SerializeField]
        private bool showSelectionsCount;

        private void OnEnable()
        {
            hideFlags &= ~HideFlags.NotEditable;
        }

        internal void Save()
        {
            Save(true);
        }
    }
}