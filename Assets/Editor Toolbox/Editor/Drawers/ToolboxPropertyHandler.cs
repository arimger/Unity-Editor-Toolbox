using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Attributes.Property;

    /// <summary>
    /// Helper class used in <see cref="SerializedProperty"/> display process.
    /// </summary>
    internal class ToolboxPropertyHandler : ISerializedPropertyContext
    {
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
        /// Temporary solution to cache all attributes responsible for overriding label.
        /// This will be replaced with validation attributes.
        /// </summary>
        private ILabelProcessorAttribute labelProcessorAttribute;

        /// <summary>
        /// Constructor prepares all property-related data for custom drawing.
        /// </summary>
        internal ToolboxPropertyHandler(SerializedProperty property)
        {
            this.Property = property;

            //here starts preparation of all needed data for this handler
            //first of all we have to retrieve the native data like FieldInfo, custom native drawer, etc.
            //after this we have to retrieve (if possible) all Toolbox-related data - ToolboxAttributes

            label = new GUIContent(property.displayName);
            //get FieldInfo associated to this property, it is needed to cache custom attributes
            if ((FieldInfo = property.GetFieldInfo(out var type)) == null)
            {
                return;
            }

            Type = type;
            //initialize basic information about property
            isArray = property.isArray && property.propertyType == SerializedPropertyType.Generic;
            isChild = property.name != FieldInfo.Name;

            //try to fetch additional data about drawers
            ProcessBuiltInData();
            ProcessToolboxData();
        }

        private void ProcessBuiltInData()
        {
            var attributes = FieldInfo.GetCustomAttributes<PropertyAttribute>();
            foreach (var attribute in attributes)
            {
                HandleNewAttribute(attribute);
            }

            CheckIfPropertyHasPropertyDrawer(Type);
        }

        /// <summary>
        /// Process all Toolbox-related attributes.
        /// </summary>
        private void ProcessToolboxData()
        {
            //get all possible attributes and handle each directly by type
            var attributes = FieldInfo.GetCustomAttributes<ToolboxAttribute>();
            foreach (var attribute in attributes)
            {
                HandleNewAttribute(attribute);
            }

            //check if property has a custom attribute or target type drawer
            hasToolboxPropertyAssignableDrawer = propertyAttribute != null;
            hasToolboxPropertyTargetTypeDrawer = ToolboxDrawersManager.HasTargetTypeDrawer(Type);
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

        private void CheckIfPropertyHasPropertyDrawer(Type type)
        {
            //NOTE: arrays cannot have built-in property drawers
            if (hasBuiltInPropertyDrawer || isArray)
            {
                return;
            }

            hasBuiltInPropertyDrawer = ToolboxDrawersManager.HasNativeTypeDrawer(type);
        }

        private void HandleNewAttribute(PropertyAttribute attribute)
        {
            //NOTE: setting tooltip and labels is valid only for parent or single properties
            //it's a bit ugly but, it's the only semi-acceptable way to support built-in TooltipAttribute
            //TODO: move these attributes to validation drawers
            switch (attribute)
            {
                case TooltipAttribute a:
                    label.tooltip = a.tooltip;
                    return;
                case NewLabelAttribute a:
                    if (!isChild)
                    {
                        labelProcessorAttribute = a;
                    }

                    return;
                case LabelByChildAttribute a:
                    if (!isArray)
                    {
                        labelProcessorAttribute = a;
                    }

                    return;
            }

            var attributeType = attribute.GetType();
            CheckIfPropertyHasPropertyDrawer(attributeType);
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

        private void DrawProperty(SerializedProperty property, GUIContent label)
        {
            ProcessLabel(property, label);

            //get toolbox drawer for the property or draw it in the default way
            if (hasToolboxPropertyDrawer && (!hasBuiltInPropertyDrawer || isArray))
            {
                //NOTE: attribute-related drawers have priority over type
                if (hasToolboxPropertyAssignableDrawer)
                {
                    //draw target property using the associated attribute
                    var propertyDrawer = isArray
                        ? ToolboxDrawersManager.GetListPropertyDrawer(propertyAttribute.GetType())
                        : ToolboxDrawersManager.GetSelfPropertyDrawer(propertyAttribute.GetType());
                    propertyDrawer?.OnGui(property, label, propertyAttribute);
                }
                else
                {
                    //draw target property using the associated type drawer
                    ToolboxDrawersManager.GetTargetTypeDrawer(Type)?.OnGui(property, label);
                }

                return;
            }

            OnGuiDefault(property, label);
        }

        private void BeginDecoratorDrawers(PropertyCondition conditionState = PropertyCondition.Valid)
        {
            if (!hasToolboxDecoratorDrawer)
            {
                return;
            }

            for (var i = 0; i < decoratorAttributes.Count; i++)
            {
                HandleDecorator(decoratorAttributes[i], true, conditionState);
            }
        }

        private void CloseDecoratorDrawers(PropertyCondition conditionState = PropertyCondition.Valid)
        {
            if (!hasToolboxDecoratorDrawer)
            {
                return;
            }

            for (var i = decoratorAttributes.Count - 1; i >= 0; i--)
            {
                HandleDecorator(decoratorAttributes[i], false, conditionState);
            }
        }

        private void HandleDecorator(ToolboxDecoratorAttribute attribute, bool onBegin, PropertyCondition conditionState = PropertyCondition.Valid)
        {
            var drawer = ToolboxDrawersManager.GetDecoratorDrawer(attribute);
            if (drawer == null)
            {
                return;
            }

            if (!attribute.ApplyCondition)
            {
                conditionState = PropertyCondition.Valid;
            }

            var isValid = conditionState != PropertyCondition.NonValid;
            var disable = conditionState == PropertyCondition.Disabled;
            if (isValid)
            {
                using (new EditorGUI.DisabledScope(disable))
                {
                    if (onBegin)
                    {
                        drawer.OnGuiBegin(attribute, this);
                    }
                    else
                    {
                        drawer.OnGuiClose(attribute, this);
                    }
                }
            }
        }

        private PropertyCondition Validate(SerializedProperty property)
        {
            if (!hasToolboxConditionDrawer)
            {
                return PropertyCondition.Valid;
            }

            return ToolboxDrawersManager.GetConditionDrawer(conditionAttribute)?.OnGuiValidate(property, conditionAttribute) ?? PropertyCondition.Valid;
        }

        //TODO: replace this method with validation attributes
        private void ProcessLabel(SerializedProperty property, GUIContent label)
        {
            if (labelProcessorAttribute == null)
            {
                return;
            }

            switch (labelProcessorAttribute)
            {
                case NewLabelAttribute a:
                    label.text = a.NewLabel;
                    return;
                case LabelByChildAttribute a:
                    PropertyUtility.OverrideLabelByChild(label, property, a.ChildName);
                    return;
            }
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
            OnGuiLayout(Property, label);
        }

        /// <summary>
        /// Draw property using built-in layout system and cached <see cref="ToolboxAttributeDrawer"/>s.
        /// </summary>
        public void OnGuiLayout(SerializedProperty property)
        {
            OnGuiLayout(property, label);
        }

        /// <summary>
        /// Draw property using built-in layout system and cached <see cref="ToolboxAttributeDrawer"/>s.
        /// </summary>
        public void OnGuiLayout(SerializedProperty property, GUIContent label)
        {
            //depending on previously gained data we can provide more action
            //using custom attributes and information about native drawers
            //we can use all associated and allowed ToolboxDrawers (for each type)

            //handle condition attribute and draw property if possible
            var conditionState = Validate(property);
            //begin all needed decorator drawers in the proper order
            BeginDecoratorDrawers(conditionState);
            var isValid = conditionState != PropertyCondition.NonValid;
            var disable = conditionState == PropertyCondition.Disabled;
            if (isValid)
            {
                using (new EditorGUI.DisabledScope(disable))
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        DrawProperty(property, label);
                    }
                }
            }

            //close all needed decorator drawers in the proper order
            CloseDecoratorDrawers(conditionState);
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
            OnGuiDefault(Property, label);
        }

        /// <summary>
        /// Draws property in the default way, without additional <see cref="ToolboxAttributeDrawer"/>s.
        /// </summary>
        public void OnGuiDefault(SerializedProperty property)
        {
            OnGuiDefault(property, label);
        }

        /// <summary>
        /// Draws property in the default way, without additional <see cref="ToolboxAttributeDrawer"/>s.
        /// </summary>
        public void OnGuiDefault(SerializedProperty property, GUIContent label)
        {
#if TOOLBOX_FORCE_DEFAULT_LISTS
            if (isArray)
            {
                EditorGUILayout.PropertyField(property, label, true);
                return;
            }
#endif
            //all "single" properties and native drawers should be drawn in the native way
            if (hasBuiltInPropertyDrawer)
            {
                ToolboxEditorGui.DrawNativeProperty(property, label);
            }
            //handles property in default native way but supports ToolboxDrawers in children
            else
            {
                ToolboxEditorGui.DrawDefaultProperty(property, label);
            }
        }

        /// <summary>
        /// Target property which contains all useful data about the associated field. 
        /// </summary>
        public SerializedProperty Property { get; }

        /// <summary>
        /// Info associated to the <see cref="Property"/>.
        /// </summary>
        public FieldInfo FieldInfo { get; }

        /// <summary>
        /// Type associated to the <see cref="Property"/>.
        /// </summary>
        public Type Type { get; }
    }
}