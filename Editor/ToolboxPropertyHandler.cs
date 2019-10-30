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
        /// First cached <see cref="ToolboxCollectionAttribute"/>.
        /// </summary>
        private readonly ToolboxCollectionAttribute collectionAttribute;


        /// <summary>
        /// Field info associated to <see cref="property"/>.
        /// </summary>
        private readonly FieldInfo propertyFieldInfo;

        /// <summary>
        /// This flag determines whenever property has custom <see cref="PropertyDrawer"/>.
        /// </summary>
        private readonly bool hasNativePropertyDrawer;
        /// <summary>
        /// This flag determines whenever property has custom <see cref="ToolboxPropertyDrawer{T}"/> or <see cref="ToolboxCollectionDrawer{T}"/>.
        /// </summary>
        private readonly bool hasToolboxPropertyDrawer;


        public ToolboxPropertyHandler(SerializedProperty property)
        {
            this.property = property;

            //get field info associated with this property
            propertyFieldInfo = property.GetFieldInfo(out Type propertyType);

            if (propertyFieldInfo == null)
            {
                return;
            }

            //check if this property has built-in property drawer
            if (!(hasNativePropertyDrawer = property.HasCustomDrawer(propertyType)))
            {
                var propertyAttributes = propertyFieldInfo.GetCustomAttributes<PropertyAttribute>();
                foreach (var attribute in propertyAttributes)
                {
                    if (hasNativePropertyDrawer = property.HasCustomDrawer(attribute.GetType()))
                    {
                        break;
                    }
                }
            }

            //specify drawer attribute 
            if (property.isArray)
            {
                //get collection drawer associated to this array field
                collectionAttribute = propertyFieldInfo.GetCustomAttribute<ToolboxCollectionAttribute>();
            }
            else
            {
                //get property drawer associated to this property
                propertyAttribute = propertyFieldInfo.GetCustomAttribute<ToolboxPropertyAttribute>();
            }

            hasToolboxPropertyDrawer = propertyAttribute != null || collectionAttribute != null;

            //validate child property using associated field info
            if (propertyFieldInfo == null || propertyFieldInfo.Name != property.name)
            {
                return;
            }

            //get only one condition attribute to valdiate state of this property
            conditionAttribute = propertyFieldInfo.GetCustomAttribute<ToolboxConditionAttribute>();

            //get all available area attributes
            areaAttributes = propertyFieldInfo.GetCustomAttributes<ToolboxAreaAttribute>().ToArray();
            //keep area attributes in proper order
            Array.Sort(areaAttributes, (a1, a2) => a1.Order.CompareTo(a2.Order));
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
                    ToolboxDrawerUtility.GetAreaDrawer(areaAttributes[i])?.OnGuiBegin(areaAttributes[i]);
                }
            }

            //handle condition attribute(only one allowed)
            var conditionState = PropertyCondition.Valid;
            if (conditionAttribute != null)
            {
                conditionState = ToolboxDrawerUtility.GetConditionDrawer(conditionAttribute)?.OnGuiValidate(property, conditionAttribute) ?? conditionState;
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

            //get property drawer for single property
            if (hasToolboxPropertyDrawer)
            {
                if (property.isArray)
                {
                    ToolboxDrawerUtility.GetCollectionDrawer(collectionAttribute)?.OnGui(property, collectionAttribute);
                }
                else
                {
                    ToolboxDrawerUtility.GetPropertyDrawer(propertyAttribute)?.OnGui(property, propertyAttribute);
                }
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
                    ToolboxDrawerUtility.GetAreaDrawer(areaAttributes[i])?.OnGuiEnd(areaAttributes[i]);
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
            if (!property.hasVisibleChildren || hasNativePropertyDrawer)
            {
                EditorGUILayout.PropertyField(property, property.isExpanded);
                return;
            }

            //draw standard foldout for children-based properties       
            if (!(property.isExpanded =
                EditorGUILayout.Foldout(property.isExpanded, new GUIContent(property.displayName), true)))
            {
                return;
            }

            var iterateThroughChildren = true;

            //handle property references
            var iterProperty = property.Copy();
            var lastProperty = iterProperty.GetEndProperty();

            EditorGUI.indentLevel++;

            //iterate over all children(but only one level depth)
            while (iterProperty.NextVisible(iterateThroughChildren))
            {
                if (SerializedProperty.EqualContents(iterProperty, lastProperty))
                {
                    break;
                }

                iterateThroughChildren = false;

                //handle current property using Toolbox drawers
                ToolboxEditorGui.DrawLayoutToolboxProperty(iterProperty.Copy());
            }

            //restore old indent level
            EditorGUI.indentLevel--;
        }
    }
}