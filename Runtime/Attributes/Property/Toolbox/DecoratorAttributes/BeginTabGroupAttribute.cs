using System;

namespace UnityEngine
{
    //TODO: move to a separate scripts
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class BeginTabGroupAttribute : ToolboxDecoratorAttribute
    {
        public BeginTabGroupAttribute(string groupId = "Default")
        {
            GroupId = groupId;
        }

        //TODO: different label and group id
        public string GroupId { get; }
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
