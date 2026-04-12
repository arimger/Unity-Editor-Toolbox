using System;

namespace UnityEngine
{
    //TODO: move to a separate namespace
    public enum TabGroupVisual
    {
        Default,
        /// <summary>
        /// Modern flat buttons.
        /// </summary>
        Flat,
        /// <summary>
        /// Connected segmented control.
        /// </summary>
        Segmented
    }

    //TODO: move to a separate scripts
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class BeginTabGroupAttribute : ToolboxDecoratorAttribute
    {
        public BeginTabGroupAttribute(string groupId = "Default", TabGroupVisual visual = TabGroupVisual.Default)
        {
            GroupId = groupId;
            Visual = visual;
        }

        public string GroupId { get; }

        public TabGroupVisual Visual { get; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class TabAttribute : ToolboxConditionAttribute
    {
        public TabAttribute(string tab)
        {
            Tab = tab;
        }

        public string Tab { get; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EndTabGroupAttribute : ToolboxDecoratorAttribute
    { }
}
