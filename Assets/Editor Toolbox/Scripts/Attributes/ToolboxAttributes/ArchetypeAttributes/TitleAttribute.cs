using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine
{
    /// <summary>
    /// Standardized header, it's composition of the <see cref="LabelAttribute"/> and the <see cref="LineAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TitleAttribute : ToolboxArchetypeAttribute
    {
        public TitleAttribute(string label)
        {
            Label = label;
        }


        public override ToolboxAttribute[] Process()
        {
            return new ToolboxAttribute[]
            {
                //NOTE: unfortunately we have to remove standard spacing between controls
                new LabelAttribute(Label)
                {
#if UNITY_EDITOR
                    SpaceAfter = -EditorGUIUtility.standardVerticalSpacing
#endif
                },
                new LineAttribute(padding: 0)
                {
                    ApplyIndent = true
                }
            };
        }


        public string Label { get; private set; }
    }
}