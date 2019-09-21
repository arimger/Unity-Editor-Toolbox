using System;

namespace Toolbox.Editor
{
    internal class AttributeNotSupportedException : Exception
    {
        public AttributeNotSupportedException(Attribute attribute) : base("Error - " + attribute.GetType() + " is not supported. Assign it in ComponentEditorSettings.")
        { }
    }
}