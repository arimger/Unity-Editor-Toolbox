using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(LabelByChildAttribute))]
    public class LabelByChildAttributeDrawer : PropertyDrawerBase
    {
        private GUIContent GetLabelByValue(SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:
                    break;
                case SerializedPropertyType.Integer:
                    label.text = property.intValue.ToString();
                    break;
                case SerializedPropertyType.Boolean:
                    label.text = property.boolValue.ToString();
                    break;
                case SerializedPropertyType.Float:
                    label.text = property.floatValue.ToString();
                    break;
                case SerializedPropertyType.String:
                    label.text = property.stringValue;
                    break;
                case SerializedPropertyType.Color:
                    label.text = property.colorValue.ToString();
                    break;
                case SerializedPropertyType.ObjectReference:
                    label.text = property.objectReferenceValue ? property.objectReferenceValue.name : "null";
                    break;
                case SerializedPropertyType.LayerMask:
                    switch (property.intValue)
                    {
                        case 0:
                            label.text = "Nothing";
                            break;
                        case ~0:
                            label.text = "Everything";
                            break;
                        default:
                            label.text = LayerMask.LayerToName((int)Mathf.Log(property.intValue, 2));
                            break;
                    }
                    break;
                case SerializedPropertyType.Enum:
                    label.text = property.enumNames[property.enumValueIndex];
                    break;
                case SerializedPropertyType.Vector2:
                    label.text = property.vector2Value.ToString();
                    break;
                case SerializedPropertyType.Vector3:
                    label.text = property.vector3Value.ToString();
                    break;
                case SerializedPropertyType.Vector4:
                    label.text = property.vector4Value.ToString();
                    break;
                case SerializedPropertyType.Rect:
                    label.text = property.rectValue.ToString();
                    break;
                case SerializedPropertyType.Character:
                    label.text = ((char)property.intValue).ToString();
                    break;
                case SerializedPropertyType.Bounds:
                    label.text = property.boundsValue.ToString();
                    break;
                case SerializedPropertyType.Quaternion:
                    label.text = property.quaternionValue.ToString();
                    break;
                default:
                    break;
            }

            return label;
        }


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyName = Attribute.ChildName;
            var childProperty = property.FindPropertyRelative(propertyName);
            //validate availability of the child property
            if (childProperty != null)
            {
                //set new label if found (unknown types will be ignored)
                label = GetLabelByValue(childProperty, label);
            }
            else
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property, string.Format("{0} does not exists.", propertyName));
            }

            EditorGUI.PropertyField(position, property, label, property.isExpanded);
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.hasChildren;
        }


        private LabelByChildAttribute Attribute => attribute as LabelByChildAttribute;
    }
}