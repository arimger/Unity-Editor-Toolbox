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
        private readonly SerializedProperty property;

        /// <summary>
        /// All associated <see cref="ToolboxDecoratorAttribute"/>s.
        /// </summary>
        private readonly ToolboxDecoratorAttribute[] decoratorAttributes;

        /// <summary>
        /// First cached <see cref="ToolboxPropertyAttribute"/>.
        /// </summary>
        private readonly ToolboxPropertyAttribute propertyFieldAttribute;
        /// <summary>
        /// First cached <see cref="ToolboxCollectionAttribute"/>.
        /// </summary>
        private readonly ToolboxCollectionAttribute propertyArrayAttribute;
        /// <summary>
        /// First cached <see cref="ToolboxConditionAttribute"/>.
        /// </summary>
        private readonly ToolboxConditionAttribute conditionAttribute;

        /// <summary>
        /// Field Type associated to <see cref="property"/>.
        /// </summary>
        private readonly Type propertyType;
        /// <summary>
        /// Field Info associated to <see cref="property"/>.
        /// </summary>
        private readonly FieldInfo propertyFieldInfo;

        /// <summary>
        /// Property label conent based on the display name and optional tooltip.
        /// </summary>
        private readonly GUIContent propertyLabel;

        /// <summary>
        /// This flag determines whenever property has a custom <see cref="PropertyDrawer"/>.
        /// </summary>
        private readonly bool hasNativePropertyDrawer;
        /// <summary>
        /// This flag determines whenever property has a custom <see cref="ToolboxTargetTypeDrawer"/> for its type, <see cref="ToolboxPropertyDrawer{T}"/> or <see cref="ToolboxCollectionDrawer{T}"/>.
        /// </summary>
        private readonly bool hasToolboxPropertyDrawer;
        /// <summary>
        /// This flag determines whenever property has a custom <see cref="ToolboxTargetTypeDrawer"/>.
        /// </summary>
        private readonly bool hasToolboxTargetTypeDrawer;

        private readonly bool isArray;


        /// <summary>
        /// Constructor prepares all property-related data for custom drawing.
        /// </summary>
        /// <param name="property"></param>
        public ToolboxPropertyHandler(SerializedProperty property)
        {
            this.property = property;

            //here starts preparation of all needed data for this handler
            //first of all we have to retrieve native data like field info, custom native drawer, etc.
            //after this we have to retrieve (if possible) all Toolbox-related data - ToolboxAttributes

            //set basic content for handled property
            propertyLabel = new GUIContent(property.displayName);

            //get field info associated with this property, this property is needed for custom attributes
            if ((propertyFieldInfo = property.GetFieldInfo(out propertyType)) == null)
            {
                return;
            }

            isArray = property.isArray && property.propertyType == SerializedPropertyType.Generic;

            //check if this property has built-in property drawer
            if (!(hasNativePropertyDrawer = ToolboxDrawerModule.HasCustomTypeDrawer(propertyType)))
            {
                var propertyAttributes = propertyFieldInfo.GetCustomAttributes<PropertyAttribute>();
                foreach (var attribute in propertyAttributes)
                {
                    var attributeType = attribute.GetType();
                    if (hasNativePropertyDrawer = ToolboxDrawerModule.HasCustomTypeDrawer(attributeType))
                    {
                        break;
                    }
                }
            }

            if (!(hasToolboxTargetTypeDrawer = ToolboxDrawerModule.HasTargetTypeDrawer(propertyType)))
            {
                if (isArray)
                {
                    //get collection drawer associated to this array
                    propertyArrayAttribute = propertyFieldInfo.GetCustomAttribute<ToolboxCollectionAttribute>();
                }
                else
                {
                    //get property drawer associated to this field
                    propertyFieldAttribute = propertyFieldInfo.GetCustomAttribute<ToolboxPropertyAttribute>();
                }
            }

            hasToolboxPropertyDrawer = propertyFieldAttribute != null || 
                                       propertyArrayAttribute != null ||
                                       hasToolboxTargetTypeDrawer;

            //validate child property using associated field info
            if (propertyFieldInfo == null || propertyFieldInfo.Name != property.name)
            {
                return;
            }

            //get only one condition attribute to valdiate state of this property
            conditionAttribute = propertyFieldInfo.GetCustomAttribute<ToolboxConditionAttribute>();
            //get all available decorator attributes
            decoratorAttributes = propertyFieldInfo.GetCustomAttributes<ToolboxDecoratorAttribute>()
                                                   .ToArray();
            //keep decorator attributes in proper order
            Array.Sort(decoratorAttributes, (a1, a2) => a1.Order.CompareTo(a2.Order));
        }


        /// <summary>
        /// Draw property using Unity's layouting system and cached <see cref="ToolboxAttributeDrawer"/>s.
        /// </summary>
        public void OnGuiLayout()
        {
            //depending on previously gained data we can provide more action
            //using custom attributes and information about native drawers
            //we can use associated ToolboxDrawers or/and draw property in the default way

            //begin all needed decorator drawers in proper order
            if (decoratorAttributes != null)
            {
                for (var i = 0; i < decoratorAttributes.Length; i++)
                {
                    ToolboxDrawerModule.GetDecoratorDrawer(decoratorAttributes[i])?.OnGuiBegin(decoratorAttributes[i]);
                }
            }

            //handle condition attribute(only one allowed)
            var conditionState = PropertyCondition.Valid;
            if (conditionAttribute != null)
            {
                conditionState = ToolboxDrawerModule.GetConditionDrawer(conditionAttribute)?.OnGuiValidate(property, conditionAttribute) ?? conditionState;
            }

            if (conditionState == PropertyCondition.NonValid)
            {
                goto Finish;
            }

            //disable property field if it is needed
            if (conditionState == PropertyCondition.Disabled)
            {
                EditorGUI.BeginDisabledGroup(true);
            }

            //get property drawer for single property or draw it in the default way
            if (hasToolboxPropertyDrawer && (!hasNativePropertyDrawer || isArray))
            {
                if (hasToolboxTargetTypeDrawer)
                {
                    //draw target property using associated type drawer
                    ToolboxDrawerModule.GetTargetTypeDrawer(propertyType).OnGui(property, propertyLabel);
                }
                else if (isArray)
                {
                    //draw array property using associated collection drawer
                    ToolboxDrawerModule.GetCollectionDrawer(propertyArrayAttribute)?.OnGui(property, propertyLabel, propertyArrayAttribute);
                }
                else
                {
                    //draw single property using associated property drawer
                    ToolboxDrawerModule.GetPropertyDrawer(propertyFieldAttribute)?.OnGui(property, propertyLabel, propertyFieldAttribute);
                }
            }
            else
            {
                if (hasToolboxPropertyDrawer)
                {
                    //TODO: warning
                    //NOTE: since property has custom drawer it will override any Toolbox-related one
                }

                OnGuiDefault();
            }

            //end disabled state check
            if (conditionState == PropertyCondition.Disabled)
            {
                EditorGUI.EndDisabledGroup();
            }

            Finish:
            //end all needed decorator drawers in proper order
            if (decoratorAttributes != null)
            {
                for (var i = decoratorAttributes.Length - 1; i >= 0; i--)
                {
                    ToolboxDrawerModule.GetDecoratorDrawer(decoratorAttributes[i])?.OnGuiEnd(decoratorAttributes[i]);
                }
            }
        }

        /// <summary>
        /// Draws property in the default way, without additional <see cref="ToolboxAttributeDrawer"/>s.
        /// </summary>
        /// <param name="property"></param>
        public void OnGuiDefault()
        {
            //all "single" properties and all properties with custom native drawers should be drawn in the standard way
            if (!property.hasVisibleChildren || hasNativePropertyDrawer)
            {
                ToolboxEditorGui.DrawLayoutNativeProperty(property, propertyLabel);
                return;
            }

            //handles property in default native way but supports ToolboxDrawers in children
            ToolboxEditorGui.DrawLayoutDefaultProperty(property, propertyLabel);
        }
    }
}