using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class LabelWithLineAttribute : ToolboxCompositionAttribute
    {
        public LabelWithLineAttribute(string label)
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
                new LineAttribute(padding:0)
            };
        }


        public string Label { get; private set; }
    }
}