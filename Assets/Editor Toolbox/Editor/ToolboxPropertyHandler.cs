using System;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    /// <summary>
    /// Helper class used in <see cref="SerializedProperty"/> display process.
    /// </summary>
    internal class ToolboxPropertyHandler
    {
        /// <summary>
        /// Type associated to the <see cref="property"/>.
        /// </summary>
        private readonly Type type;

        /// <summary>
        /// Data associated to the <see cref="property"/>.
        /// </summary>
        private readonly FieldInfo fieldInfo;

        /// <summary>
        /// Target property which contains all useful data about the associated field. 
        /// </summary>
        private readonly SerializedProperty property;

        /// <summary>
        /// First cached <see cref="ToolboxPropertyAttribute"/>.
        /// </summary>
        private readonly ToolboxPropertyAttribute propertyAttribute;

        /// <summary>
        /// First cached <see cref="ToolboxConditionAttribute"/>.
        /// </summary>
        private readonly ToolboxConditionAttribute conditionAttribute;

        /// <summary>
        /// All associated <see cref="ToolboxDecoratorAttribute"/>s.
        /// </summary>
        private readonly ToolboxDecoratorAttribute[] decoratorAttributes;

        /// <summary>
        /// Property label content based on the display name and optional tooltip.
        /// </summary>
        private readonly GUIContent label;

        /// <summary>
        /// Determines whenever property is an array/list.
        /// </summary>
        private readonly bool isArray;
        /// <summary>
        /// Determines whenever property is an array child.
        /// </summary>
        private readonly bool isChild;

        /// <summary>
        /// Determines whenever property has a custom <see cref="PropertyDrawer"/>.
        /// </summary>
        private readonly bool hasNativePropertyDrawer;

        /// <summary>
        /// Determines whenever property has a custom <see cref="ToolboxTargetTypeDrawer"/> for its type or <see cref="ToolboxPropertyDrawer{T}"/>.
        /// </summary>
        private readonly bool hasToolboxPropertyDrawer;
        /// <summary>
        /// Determines whenever property has a custom <see cref="ToolboxPropertyDrawer{T}"/>.
        /// </summary>
        private readonly bool hasToolboxPropertyAttributeDrawer;
        /// <summary>
        /// Determines whenever property has a custom <see cref="ToolboxTargetTypeDrawer"/>.
        /// </summary>
        private readonly bool hasToolboxPropertyTargetTypeDrawer;

        private readonly bool hasToolboxConditionDrawer;
        private readonly bool hasToolboxDecoratorDrawer;


        /// <summary>
        /// Constructor prepares all property-related data for custom drawing.
        /// </summary>
        /// <param name="property"></param>
        public ToolboxPropertyHandler(SerializedProperty property)
        {
            this.property = property;

            //here starts preparation of all needed data for this handler
            //first of all we have to retrieve the native data like FieldInfo, custom native drawer, etc.
            //after this we have to retrieve (if possible) all Toolbox-related data - ToolboxAttributes

            //set basic content for the handled property
            label = new GUIContent(property.displayName);

            //get FieldInfo associated to this property, it is needed to cache custom attributes
            if ((fieldInfo = property.GetFieldInfo(out type)) == null)
            {
                return;
            }

            isArray = property.isArray && property.propertyType == SerializedPropertyType.Generic;

            //check if this property has built-in property drawer
            if (!(hasNativePropertyDrawer = ToolboxDrawerModule.HasNativeTypeDrawer(type)))
            {
                var propertyAttributes = fieldInfo.GetCustomAttributes<PropertyAttribute>();
                foreach (var attribute in propertyAttributes)
                {
                    var attributeType = attribute.GetType();
                    if (hasNativePropertyDrawer = ToolboxDrawerModule.HasNativeTypeDrawer(attributeType))
                    {
                        break;
                    }
                }
            }

            if (isArray)
            {
                //get collection drawer associated to this array
                propertyAttribute = fieldInfo.GetCustomAttribute<ToolboxListPropertyAttribute>();
            }
            else
            {
                //get property drawer associated to this field
                propertyAttribute = fieldInfo.GetCustomAttribute<ToolboxSelfPropertyAttribute>();
            }

            //check if property has a custom attribute drawer
            hasToolboxPropertyAttributeDrawer = propertyAttribute != null;
            //check if property has a custom target type drawer
            hasToolboxPropertyTargetTypeDrawer = ToolboxDrawerModule.HasTargetTypeDrawer(type);

            hasToolboxPropertyDrawer = hasToolboxPropertyAttributeDrawer || hasToolboxPropertyTargetTypeDrawer;

            //validate child property using the associated FieldInfo
            if (isChild = (property.name != fieldInfo.Name))
            {
                return;
            }

            //get only one condition attribute to valdiate state of this property
            conditionAttribute = fieldInfo.GetCustomAttribute<ToolboxConditionAttribute>();
            hasToolboxConditionDrawer = conditionAttribute != null;

            //get all available decorator attributes
            decoratorAttributes = fieldInfo.GetCustomAttributes<ToolboxDecoratorAttribute>().ToArray();
            hasToolboxDecoratorDrawer = decoratorAttributes != null && decoratorAttributes.Length > 0;
            //keep decorator attributes in the order
            Array.Sort(decoratorAttributes, (a1, a2) => a1.Order.CompareTo(a2.Order));
        }


        private void BeginDecoratorDrawers()
        {
            if (!hasToolboxDecoratorDrawer)
            {
                return;
            }

            for (var i = 0; i < decoratorAttributes.Length; i++)
            {
                ToolboxDrawerModule.GetDecoratorDrawer(decoratorAttributes[i])?.OnGuiBegin(decoratorAttributes[i]);
            }
        }

        private void CloseDecoratorDrawers()
        {
            if (!hasToolboxDecoratorDrawer)
            {
                return;
            }

            for (var i = decoratorAttributes.Length - 1; i >= 0; i--)
            {
                ToolboxDrawerModule.GetDecoratorDrawer(decoratorAttributes[i])?.OnGuiClose(decoratorAttributes[i]);
            }
        }

        private PropertyCondition Validate()
        {
            var conditionState = PropertyCondition.Valid;
            if (!hasToolboxConditionDrawer)
            {
                return conditionState;
            }

            return ToolboxDrawerModule.GetConditionDrawer(conditionAttribute)?.OnGuiValidate(property, conditionAttribute) ?? conditionState;
        }

        private void DrawProperty()
        {
            //get toolbox drawer for the property or draw it in the default way
            if (hasToolboxPropertyDrawer && (!hasNativePropertyDrawer || isArray))
            {
                //NOTE: attribute-related drawers have priority 
                if (hasToolboxPropertyAttributeDrawer)
                {
                    //draw target property using the associated attribute
                    var propertyDrawer = isArray
                        ? ToolboxDrawerModule.GetListPropertyDrawer(propertyAttribute.GetType())
                        : ToolboxDrawerModule.GetSelfPropertyDrawer(propertyAttribute.GetType());
                    propertyDrawer?.OnGui(property, label, propertyAttribute);
                }
                else
                {
                    //draw target property using the associated type drawer
                    ToolboxDrawerModule.GetTargetTypeDrawer(type)?.OnGui(property, label);
                }
            }
            else
            {
                if (hasToolboxPropertyDrawer)
                {
                    //TODO: warning
                    //NOTE: since property has a custom drawer it will override any Toolbox-related one
                }

                OnGuiDefault();
            }
        }


        /// <summary>
        /// Draw property using built-in layout system and cached <see cref="ToolboxAttributeDrawer"/>s.
        /// </summary>
        public void OnGuiLayout()
        {
            //depending on previously gained data we can provide more action
            //using custom attributes and information about native drawers
            //we can use associated ToolboxDrawers or/and draw property in the default way

            //begin all needed decorator drawers in the proper order
            BeginDecoratorDrawers();

            //handle condition attribute and draw property if possible
            var conditionState = Validate();
            var isValid = conditionState != PropertyCondition.NonValid;
            var disable = conditionState == PropertyCondition.Disabled;
            if (isValid)
            {
                using (new EditorGUI.DisabledScope(disable))
                {
                    DrawProperty();
                }
            }

            //close all needed decorator drawers in the proper order
            CloseDecoratorDrawers();
        }

        /// <summary>
        /// Draws property in the default way, without additional <see cref="ToolboxAttributeDrawer"/>s.
        /// </summary>
        /// <param name="property"></param>
        public void OnGuiDefault()
        {
            //all "single" properties and native drawers should be drawn in the default way
            if (!property.hasVisibleChildren || hasNativePropertyDrawer)
            {
                ToolboxEditorGui.DrawNativeProperty(property, label);
                return;
            }

            //handles property in default native way but supports ToolboxDrawers in children
            ToolboxEditorGui.DrawDefaultProperty(property, label);
        }
    }
}