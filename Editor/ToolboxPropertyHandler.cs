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


        public ToolboxPropertyHandler(SerializedProperty property)
        {
            this.property = property;

            //get all available area attributes
            areaAttributes = property.GetAttributes<ToolboxAreaAttribute>();
            //keep area attributes in proper order
            System.Array.Sort(areaAttributes, (a1, a2) => a1.Order.CompareTo(a2.Order));

            //get only one attribute per type
            groupAttribute = property.GetAttribute<ToolboxGroupAttribute>();
            propertyAttribute = property.GetAttribute<ToolboxPropertyAttribute>();
            conditionAttribute = property.GetAttribute<ToolboxConditionAttribute>();
        }


        /// <summary>
        /// Draw property using Unity's layouting system and created <see cref="ToolboxDrawer"/>s.
        /// </summary>
        public void OnGui()
        {
            //begin all needed area drawers in proper order
            for (var i = 0; i < areaAttributes.Length; i++)
            {
                ToolboxEditorUtility.GetAreaDrawer(areaAttributes[i])?.OnGuiBegin(areaAttributes[i]);
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
                EditorGUILayout.PropertyField(property, property.isExpanded);
            }

            //end disabled state check
            if (conditionState == PropertyCondition.Disabled)
            {
                EditorGUI.EndDisabledGroup();
            }

            //end all needed area drawers in proper order
            for (var i = areaAttributes.Length - 1; i >= 0; i--)
            {
                ToolboxEditorUtility.GetAreaDrawer(areaAttributes[i])?.OnGuiEnd(areaAttributes[i]);
            }
        }
    }
}