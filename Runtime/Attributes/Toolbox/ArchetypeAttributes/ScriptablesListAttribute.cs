using System;
using System.Diagnostics;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class ScriptablesListAttribute : ToolboxArchetypeAttribute
    {
        public override ToolboxAttribute[] Process()
        {
            return new ToolboxAttribute[]
            {
                new InLineEditorAttribute(),
                new ReorderableListAttribute()
                {
                    Foldable = Foldable,
                    HasLabels = HasLabels,
                    HasHeader = HasHeader
                }
            };
        }

        /// <summary>
        /// Indicates whether list should be allowed to fold in and out.
        /// </summary>
        public bool Foldable { get; set; } = true;
        /// <summary>
        /// Indicates whether list should have a label above elements.
        /// </summary>
        public bool HasHeader { get; set; } = true;
        /// <summary>
        /// Indicates whether each element should have an additional label.
        /// </summary>
        public bool HasLabels { get; set; } = false;
    }
}