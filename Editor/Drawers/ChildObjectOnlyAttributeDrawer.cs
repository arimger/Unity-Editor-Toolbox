using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ChildObjectOnlyAttribute))]
    public class ChildObjectOnlyAttributeDrawer : ToolboxNativePropertyDrawer
    {
        private Transform GetTransform(Object reference)
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


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, false);
            if (EditorGUI.EndChangeCheck())
            {
                var target = property.serializedObject.targetObject as Component;
                var reference = property.objectReferenceValue;
                if (reference)
                {
                    var transfrom = GetTransform(reference);
                    if (transfrom && transfrom != target.transform && transfrom.IsChildOf(target.transform))
                    {
                        return;
                    }

                    property.objectReferenceValue = null;
                    ToolboxEditorLog.AttributeUsageWarning(attribute, property, "Assigned value has to be a child of the target transform.");
                }
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }
    }
}