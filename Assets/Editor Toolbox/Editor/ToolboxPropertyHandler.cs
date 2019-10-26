using System;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEditor;

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
        /// All associated <see cref="ToolboxAreaAttribute"/>s.
        /// </summary>
        private readonly ToolboxAreaAttribute[] areaAttributes;

        /// <summary>
        /// First cached <see cref="ToolboxPropertyAttribute"/>.
        /// </summary>
        private readonly ToolboxPropertyAttribute propertyAttribute;
        /// <summary>
        /// First cached <see cref="ToolboxConditionAttribute"/>.
        /// </summary>
        private readonly ToolboxConditionAttribute conditionAttribute;


        /// <summary>
        /// Field info associated to <see cref="property"/>.
        /// </summary>
        private readonly FieldInfo propertyFieldInfo;

        /// <summary>
        /// This flag determines  whenever property has custom <see cref="PropertyDrawer"/>.
        /// </summary>
        private readonly bool hasCustomPropertyDrawer;


        public ToolboxPropertyHandler(SerializedProperty property)
        {
            this.property = property;

            //get field info associated with this property
            propertyFieldInfo = property.GetFieldInfo(out Type propertyType);

            if (propertyFieldInfo == null)
            {
                return;
            }

            //check if this property has built-in drawer
            if (!(hasCustomPropertyDrawer = property.HasCustomDrawer(propertyType)))
            {
                var propertyAttributes = propertyFieldInfo.GetCustomAttributes<PropertyAttribute>();
                foreach (var attribute in propertyAttributes)
                {
                    if (hasCustomPropertyDrawer = property.HasCustomDrawer(attribute.GetType()))
                    {
                        break;
                    }
                }
            }

            //validate property using associated field info
            if (propertyFieldInfo == null || propertyFieldInfo.Name != property.name)
            {
                return;
            }

            //get all available area attributes
            areaAttributes = propertyFieldInfo.GetCustomAttributes<ToolboxAreaAttribute>().ToArray();     
            //keep area attributes in proper order
            Array.Sort(areaAttributes, (a1, a2) => a1.Order.CompareTo(a2.Order));

            //get only one attribute per type
            propertyAttribute = propertyFieldInfo.GetCustomAttribute<ToolboxPropertyAttribute>();
            conditionAttribute = propertyFieldInfo.GetCustomAttribute<ToolboxConditionAttribute>();
        }


        /// <summary>
        /// Draw property using Unity's layouting system and cached <see cref="ToolboxDrawer"/>s.
        /// </summary>
        public void OnGuiLayout()
        {               
            //begin all needed area drawers in proper order
            if (areaAttributes != null)
            {            
                for (var i = 0; i < areaAttributes.Length; i++)
                {
                    ToolboxEditorUtility.GetAreaDrawer(areaAttributes[i])?.OnGuiBegin(areaAttributes[i]);
                }
            }

            //handle condition attribute(only one allowed)
            var conditionState = PropertyCondition.Valid;
            if (conditionAttribute != null)
            {
                conditionState = ToolboxEditorUtility.GetConditionDrawer(conditionAttribute)?.OnGuiValidate(property, conditionAttribute) ?? conditionState;
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

            //get property attribute(only one allowed)
            if (propertyAttribute != null)
            {
                ToolboxEditorUtility.GetPropertyDrawer(propertyAttribute)?.OnGui(property, propertyAttribute);
            }
            else
            {
                OnGuiDefault();
            }

            //end disabled state check
            if (conditionState == PropertyCondition.Disabled)
            {
                EditorGUI.EndDisabledGroup();
            }

            Finish:
            //end all needed area drawers in proper order
            if (areaAttributes != null)
            {
                for (var i = areaAttributes.Length - 1; i >= 0; i--)
                {
                    ToolboxEditorUtility.GetAreaDrawer(areaAttributes[i])?.OnGuiEnd(areaAttributes[i]);
                }
            }
        }

        /// <summary>
        /// Draws property in default way, without additional <see cref="ToolboxDrawer"/>s.
        /// </summary>
        /// <param name="property"></param>
        public void OnGuiDefault()
        {
            //all "single" and all properties with custom drawers should be drawn in standard way
            if (!property.hasChildren || hasCustomPropertyDrawer)
            {
                EditorGUILayout.PropertyField(property, property.isExpanded);
                return;
            }

            //draw standard foldout for children-based properties
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, new GUIContent(property.displayName), true);

            if (!property.isExpanded)
            {
                return;
            }

            //cache current indent
            var orginalIndent = EditorGUI.indentLevel;
            var relativeIndent = orginalIndent - property.depth;

            var iterateThroughChildren = true;

            //handle property references
            var iterProperty = property.Copy();
            var lastProperty = iterProperty.GetEndProperty();

            EditorGUI.indentLevel = iterProperty.depth + relativeIndent;

            //iterate over all children(but only one level depth)
            while (iterProperty.NextVisible(iterateThroughChildren))
            {
                if (SerializedProperty.EqualContents(iterProperty, lastProperty))
                {
                    break;
                }

                EditorGUI.indentLevel = iterProperty.depth + relativeIndent;

                iterateThroughChildren = false;

                //handle current property using Toolbox drawers
                ToolboxEditorGui.DrawToolboxProperty(iterProperty.Copy());
            }

            //restore old indent level
            EditorGUI.indentLevel = orginalIndent;
        }
    }
}