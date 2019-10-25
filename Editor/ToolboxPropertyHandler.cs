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
        /// First cached <see cref="ToolboxGroupAttribute"/>.
        /// </summary>
        private readonly ToolboxGroupAttribute groupAttribute;
        /// <summary>
        /// First cached <see cref="ToolboxPropertyAttribute"/>.
        /// </summary>
        private readonly ToolboxPropertyAttribute propertyAttribute;
        /// <summary>
        /// First cached <see cref="ToolboxConditionAttribute"/>.
        /// </summary>
        private readonly ToolboxConditionAttribute conditionAttribute;


        private readonly FieldInfo fieldInfo;

        private readonly bool hasCustomPropertyDrawer;


        public ToolboxPropertyHandler(SerializedProperty property)
        {
            this.property = property;

            fieldInfo = ToolboxDrawerUtility.GetFieldInfoFromProperty(property, out Type propertyType);

            if (fieldInfo == null)
            {
                return;
            }

            //get all available area attributes
            areaAttributes = fieldInfo.GetCustomAttributes<ToolboxAreaAttribute>().ToArray();
            //keep area attributes in proper order
            Array.Sort(areaAttributes, (a1, a2) => a1.Order.CompareTo(a2.Order));

            //get only one attribute per type
            groupAttribute = fieldInfo.GetCustomAttribute<ToolboxGroupAttribute>();
            propertyAttribute = fieldInfo.GetCustomAttribute<ToolboxPropertyAttribute>();
            conditionAttribute = fieldInfo.GetCustomAttribute<ToolboxConditionAttribute>();

            //check if this property has built-in drawer
            hasCustomPropertyDrawer = ToolboxDrawerUtility.PropertyHasCustomDrawer(property, propertyType);
        }


        /// <summary>
        /// Draw property using Unity's layouting system and cached <see cref="ToolboxDrawer"/>s.
        /// </summary>
        public void OnGuiLayout()
        {
            if (areaAttributes != null)
            {            
                //begin all needed area drawers in proper order
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
                //end all area drawers without drawing property
                for (var i = areaAttributes.Length - 1; i >= 0; i--)
                {
                    ToolboxEditorUtility.GetAreaDrawer(areaAttributes[i])?.OnGuiEnd(areaAttributes[i]);
                }

                return;
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

            if (areaAttributes != null)
            {
                //end all needed area drawers in proper order
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
            //all "single" and  all properties with custom drawers should be drawn in standard way
            if (hasCustomPropertyDrawer || !property.hasChildren)
            {
                EditorGUILayout.PropertyField(property, property.isExpanded);
                return;
            }

            //draw standard foldout for children-based property
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
                EditorGUI.BeginChangeCheck();
                ToolboxEditorGui.DrawToolboxProperty(iterProperty.Copy());
                //NOTE: changing child properties (like array size) may invalidate the iterator, so stop now, or we may get errors
                if (EditorGUI.EndChangeCheck())
                {
                    break;
                };
            }

            //restore old indent level
            EditorGUI.indentLevel = orginalIndent;
        }
    }
}