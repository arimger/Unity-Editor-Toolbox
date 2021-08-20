using System;
using System.Collections.Generic;
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
        /// Target property which contains all useful data about the associated field. 
        /// </summary>
        private readonly SerializedProperty property;

        /// <summary>
        /// Info associated to the <see cref="property"/>.
        /// </summary>
        private readonly FieldInfo fieldInfo;

        /// <summary>
        /// Type associated to the <see cref="property"/>.
        /// </summary>
        private readonly Type type;

        /// <summary>
        /// Determines whenever property is an array/list.
        /// </summary>
        private readonly bool isArray;
        /// <summary>
        /// Determines whenever property is an array child.
        /// </summary>
        private readonly bool isChild;

        /// <summary>
        /// Property label content based on the display name and optional tooltip.
        /// </summary>
        private readonly GUIContent label;

        /// <summary>
        /// First cached <see cref="ToolboxPropertyAttribute"/>.
        /// </summary>
        private ToolboxPropertyAttribute propertyAttribute;

        /// <summary>
        /// All associated <see cref="ToolboxDecoratorAttribute"/>s.
        /// </summary>
        private List<ToolboxDecoratorAttribute> decoratorAttributes;

        /// <summary>
        /// First cached <see cref="ToolboxConditionAttribute"/>.
        /// </summary>
        private ToolboxConditionAttribute conditionAttribute;

        /// <summary>
        /// Determines whenever property has a custom <see cref="PropertyDrawer"/>.
        /// </summary>
        private bool hasBuiltInPropertyDrawer;
        /// <summary>
        /// Determines whenever property has a custom <see cref="ToolboxTargetTypeDrawer"/> for its type or <see cref="ToolboxPropertyDrawer{T}"/>.
        /// </summary>
        private bool hasToolboxPropertyDrawer;
        /// <summary>
        /// Determines whenever property has a custom <see cref="ToolboxPropertyDrawer{T}"/>.
        /// </summary>
        private bool hasToolboxPropertyAssignableDrawer;
        /// <summary>
        /// Determines whenever property has a custom <see cref="ToolboxTargetTypeDrawer"/>.
        /// </summary>
        private bool hasToolboxPropertyTargetTypeDrawer;
        /// <summary>
        /// Determines whenever property has a custom <see cref="ToolboxDecoratorDrawer{T}"/>
        /// </summary>
        private bool hasToolboxDecoratorDrawer;
        /// <summary>
        /// Determines whenever property has a custom <see cref="ToolboxConditionDrawer{T}"/>
        /// </summary>
        private bool hasToolboxConditionDrawer;


        /// <summary>
        /// Constructor prepares all property-related data for custom drawing.
        /// </summary>
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

            //initialize basic information about property
            isArray = property.isArray && property.propertyType == SerializedPropertyType.Generic;
            isChild = property.name != fieldInfo.Name;

            //try to fetch additional data about drawers
            ProcessBuiltInData();
            ProcessToolboxData();
        }


        private void ProcessBuiltInData()
        {
            //arrays cannot have custom built-in property drawers
            if (isArray)
            {
                return;
            }

            //check if this property has built-in property drawer
            if (!(hasBuiltInPropertyDrawer = ToolboxDrawerModule.HasNativeTypeDrawer(type)))
            {
                var propertyAttributes = fieldInfo.GetCustomAttributes<PropertyAttribute>();
                foreach (var attribute in propertyAttributes)
                {
                    var attributeType = attribute.GetType();
                    if (hasBuiltInPropertyDrawer = ToolboxDrawerModule.HasNativeTypeDrawer(attributeType))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Process all Toolbox-related attributes.
        /// </summary>
        private void ProcessToolboxData()
        {
            //get all possible attributes and handle each directly by type
            var attributes = fieldInfo.GetCustomAttributes<ToolboxAttribute>();
            foreach (var attribute in attributes)
            {
                HandleNewAttribute(attribute);
            }

            //check if property has a custom attribute or target type drawer
            hasToolboxPropertyAssignableDrawer = propertyAttribute != null;
            hasToolboxPropertyTargetTypeDrawer = ToolboxDrawerModule.HasTargetTypeDrawer(type);
            //check if property has any of it and cache value
            hasToolboxPropertyDrawer = hasToolboxPropertyAssignableDrawer ||
                                       hasToolboxPropertyTargetTypeDrawer;

            //check if property has custom decorators and keep them in order
            if (decoratorAttributes != null)
            {
                decoratorAttributes.Sort((a1, a2) => a1.Order.CompareTo(a2.Order));
                hasToolboxDecoratorDrawer = true;
            }

            //check if property has custom conditon drawer
            hasToolboxConditionDrawer = conditionAttribute != null;
        }

        private void HandleNewAttribute(ToolboxAttribute attribute)
        {
            switch (attribute)
            {
                case ToolboxListPropertyAttribute a:
                    TryAssignListPropertyAttribute(a);
                    break;
                case ToolboxSelfPropertyAttribute a:
                    TryAssignSelfPropertyAttribute(a);
                    break;
                case ToolboxDecoratorAttribute a:
                    TryAssignDecoratorAttribute(a);
                    break;
                case ToolboxConditionAttribute a:
                    TryAssignConditionAttribute(a);
                    break;
                case ToolboxArchetypeAttribute a:
                    var composition = a.Process();
                    foreach (var newAttribute in composition)
                    {
                        HandleNewAttribute(newAttribute);
                    }
                    break;
            }
        }

        private bool TryAssignBasePropertyAttribute(ToolboxPropertyAttribute attribute)
        {
            //NOTE: we can only have one property attribute
            if (propertyAttribute != null)
            {
                return false;
            }
            else
            {
                propertyAttribute = attribute;
                return true;
            }
        }

        private bool TryAssignListPropertyAttribute(ToolboxListPropertyAttribute attribute)
        {
            return isArray && TryAssignBasePropertyAttribute(attribute);
        }

        private bool TryAssignSelfPropertyAttribute(ToolboxSelfPropertyAttribute attribute)
        {
            return !isArray && TryAssignBasePropertyAttribute(attribute);
        }

        private bool TryAssignDecoratorAttribute(ToolboxDecoratorAttribute attribute)
        {
            //prevent decorators drawing for children (for array properties)
            if (isChild)
            {
                return false;
            }

            if (decoratorAttributes == null)
            {
                decoratorAttributes = new List<ToolboxDecoratorAttribute>();
            }

            decoratorAttributes.Add(attribute);
            return true;
        }

        private bool TryAssignConditionAttribute(ToolboxConditionAttribute attribute)
        {
            //prevent condition checks for children (for array properties)
            //we can only have on condition attribute per serialized property
            if (conditionAttribute != null || isChild)
            {
                return false;
            }
            else
            {
                conditionAttribute = attribute;
                return true;
            }
        }

        private void DrawProperty(GUIContent label)
        {
            //get toolbox drawer for the property or draw it in the default way
            if (hasToolboxPropertyDrawer && (!hasBuiltInPropertyDrawer || isArray))
            {
                //NOTE: attribute-related drawers have priority over type
                if (hasToolboxPropertyAssignableDrawer)
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
                OnGuiDefault(label);
            }
        }

        private void BeginDecoratorDrawers()
        {
            if (!hasToolboxDecoratorDrawer)
            {
                return;
            }

            for (var i = 0; i < decoratorAttributes.Count; i++)
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

            for (var i = decoratorAttributes.Count - 1; i >= 0; i--)
            {
                ToolboxDrawerModule.GetDecoratorDrawer(decoratorAttributes[i])?.OnGuiClose(decoratorAttributes[i]);
            }
        }

        private PropertyCondition Validate()
        {
            if (!hasToolboxConditionDrawer)
            {
                return PropertyCondition.Valid;
            }

            return ToolboxDrawerModule.GetConditionDrawer(conditionAttribute)?.OnGuiValidate(property, conditionAttribute) ?? PropertyCondition.Valid;
        }

        /// <summary>
        /// Begins vertical group to "pack" all property-related controls into one body.
        /// </summary>
        private void BeginVerticalLayoutBody()
        {
            EditorGUILayout.BeginVertical();
        }

        private void CloseVerticalLayoutBody()
        {
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draw property using built-in layout system and cached <see cref="ToolboxAttributeDrawer"/>s.
        /// </summary>
        public void OnGuiLayout()
        {
            OnGuiLayout(label);
        }

        /// <summary>
        /// Draw property using built-in layout system and cached <see cref="ToolboxAttributeDrawer"/>s.
        /// </summary>
        public void OnGuiLayout(GUIContent label)
        {
            //depending on previously gained data we can provide more action
            //using custom attributes and information about native drawers
            //we can use all associated and allowed ToolboxDrawers (for each type)

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
                    BeginVerticalLayoutBody();
                    DrawProperty(label);
                    CloseVerticalLayoutBody();
                }
            }

            //close all needed decorator drawers in the proper order
            CloseDecoratorDrawers();
        }

        /// <summary>
        /// Draws property in the default way, without additional <see cref="ToolboxAttributeDrawer"/>s.
        /// </summary>
        public void OnGuiDefault()
        {
            OnGuiDefault(label);
        }

        /// <summary>
        /// Draws property in the default way, without additional <see cref="ToolboxAttributeDrawer"/>s.
        /// </summary>
        public void OnGuiDefault(GUIContent label)
        {
            //all "single" properties and native drawers should be drawn in the native way
            if (hasBuiltInPropertyDrawer)
            {
                ToolboxEditorGui.DrawNativeProperty(property, label);
                return;
            }

            //handles property in default native way but supports ToolboxDrawers in children
            ToolboxEditorGui.DrawDefaultProperty(property, label);
        }
    }
}