using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class OrderedSpaceAttribute : OrderedAttribute
{
    public OrderedSpaceAttribute(float spaceBefore = 5.0f, float spaceAfter = 0.0f)
    {
        SpaceBefore = spaceBefore;
        SpaceAfter = spaceAfter;
    }

    public float SpaceBefore { get; private set; }
    public float SpaceAfter { get; private set; }
}
