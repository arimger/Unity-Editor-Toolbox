using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;

//TODO: handling children;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    /// <summary>
    /// Base editor class.
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(Object), true, isFallback = true)]
    public class ToolboxEditor : UnityEditor.Editor
    {
        /// <summary>
        /// All available and serialized properties(excluding children).
        /// </summary>
        protected Dictionary<string, PropertyHandler> propertyHandlers = new Dictionary<string, PropertyHandler>();


        /// <summary>
        /// Editor initialization.
        /// </summary>
        protected virtual void OnEnable()
        { }

        /// <summary>
        /// Editor deinitialization.
        /// </summary>
        protected virtual void OnDisable()
        { }

        /// <summary>
        /// Handles desired property display process using <see cref="ToolboxDrawer"/>s.
        /// </summary>
        /// <param name="property">Property to display.</param>
        protected virtual void HandleProperty(SerializedProperty property)
        {
            if (!propertyHandlers.TryGetValue(property.name, out PropertyHandler propertyHandler))
            {
                //initialize and store new property handler
                propertyHandlers[property.name] = propertyHandler = new PropertyHandler(property);
            }

            propertyHandler.OnGui();
        }


        /// <summary>
        /// Inspector GUI re-draw call.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (!ToolboxEditorUtility.ToolboxDrawersAllowed)
            {
                DrawDefaultInspector();
                return;
            }

            //begin Toolbox inspector
            EditorGUILayout.LabelField("Component Editor", EditorStyles.centeredGreyMiniLabel);

            serializedObject.Update();
            var property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                //draw standard script property
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(property);
                EditorGUI.EndDisabledGroup();

                //draw every property using ToolboxAttributes&Drawers
                while (property.NextVisible(false))
                {
                    HandleProperty(property.Copy());
                }
            }
            serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// Helper class used in <see cref="SerializedProperty"/> display process.
        /// </summary>
        protected class PropertyHandler
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


            public PropertyHandler(SerializedProperty property)
            {
                this.property = property;

                //get all available area attributes
                areaAttributes = property.GetAttributes<ToolboxAreaAttribute>();
                //keep area attributes in proper order
                Array.Sort(areaAttributes, (a1, a2) => a1.Order.CompareTo(a2.Order));

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
}