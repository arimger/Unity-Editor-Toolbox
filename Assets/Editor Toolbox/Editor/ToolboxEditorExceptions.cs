using System;

namespace Toolbox.Editor
{
    internal class AttributeNotSupportedException : Exception
    {
        public AttributeNotSupportedException(Attribute attribute) 
            : base("Error - " + attribute.GetType() + " is not supported. Assign it in ToolboxEditorSettings.")
        { }
    }

    [Obsolete]
    internal class WrongAttributeUsageException : Exception
    {
        public WrongAttributeUsageException(UnityEditor.SerializedProperty property, Attribute attribute) 
            : base(property.name + " property in " + property.serializedObject.targetObject +
                   " - " + attribute.GetType() + " cannot be used on this property.")
        { }
    }

    internal class DrawerAttributeArgumentException : ArgumentException
    {
        public DrawerAttributeArgumentException(Type targetAttributeType)
            : base("Invalid drawer attribute, " + targetAttributeType.Name + " expected.", "attribute")
        { }
    }
}