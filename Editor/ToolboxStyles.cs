using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    /// <summary>
    /// Utility class responsible for creation and sharing custom <see cref="GUIStyle"/>s.
    /// </summary>
    public static class ToolboxStyles
    {
        static ToolboxStyles()
        {
            ZeroMargins = new GUIStyle(EditorStyles.inspectorFullWidthMargins)
            {
                padding = new RectOffset()
            };
        }


        public static GUIStyle ZeroMargins { get; private set; }
    }
}