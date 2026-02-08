using System;

namespace UnityEngine
{
    public enum TabGroupVisual
    {
        Default,
        Flat, // modern flat buttons
        Segmented, // connected segmented control
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class BeginTabGroupAttribute : ToolboxDecoratorAttribute
    {
        public string GroupId { get; }
        public TabGroupVisual Visual { get; }

        public BeginTabGroupAttribute(
            string groupId = "Default",
            TabGroupVisual visual = TabGroupVisual.Default
        )
        {
            GroupId = groupId;
            Visual = visual;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class TabAttribute : ToolboxConditionAttribute
    {
        public string Tab { get; }

        public TabAttribute(string tab)
        {
            Tab = tab;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EndTabGroupAttribute : ToolboxDecoratorAttribute { }
}
