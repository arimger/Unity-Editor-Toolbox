using System;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine
{
    /// <summary>
    /// Standardized header, it's composition of the <see cref="LabelAttribute"/> and the <see cref="LineAttribute"/>.
    /// <para><see cref="LineAttribute"/> is always created with <see cref="Order"/> + 1.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
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
                    SpaceAfter = -EditorGUIUtility.standardVerticalSpacing,
#endif
                    Order = Order,
                    ApplyCondition = ApplyCondition
                },
                new LineAttribute(padding: 0)
                {
                    ApplyIndent = true,
                    Order = Order + 1,
                    ApplyCondition = ApplyCondition
                }
            };
        }

        public string Label { get; private set; }

        /// <summary>
        /// Order of the decorator relative to other drawers on the same property.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Indicates if decorator should be created independly to state of the property.
        /// If <see cref="ApplyCondition"/> equals <see langword="true"/> it means that the decorator can be hidden/disabled same as an associated property. 
        /// </summary>
        public bool ApplyCondition { get; set; }
    }
}