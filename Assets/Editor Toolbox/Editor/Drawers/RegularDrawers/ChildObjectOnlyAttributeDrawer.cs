using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ChildObjectOnlyAttribute))]
    public class ChildObjectOnlyAttributeDrawer : ObjectValidationDrawer
    {
        protected Transform GetTransform(Object reference)
        {
            switch (reference)
            {
                case GameObject gameObject:
                    return gameObject.transform;
                case Component component:
                    return component.transform;
            }

            return null;
        }


        protected override string GetWarningMessage()
        {
            return "Assigned value has to be a child of the target transform.";
        }

        protected override bool IsObjectValid(Object objectValue, SerializedProperty property)
        {
            var component = property.serializedObject.targetObject as Component;
            var transfrom = GetTransform(objectValue);
            return transfrom && transfrom != component.transform && transfrom.IsChildOf(component.transform);
        }
    }
}