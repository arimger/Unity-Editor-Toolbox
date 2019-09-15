using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

//TODO: handling children;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    /// <summary>
    /// Base editor class.
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(Object), true, isFallback = true)]
    public class ComponentEditor : UnityEditor.Editor
    {
        protected static ComponentEditorSettings settings;


        protected static Dictionary<Type, ToolboxAreaDrawerBase> areaDrawers = new Dictionary<Type, ToolboxAreaDrawerBase>();

        protected static Dictionary<Type, ToolboxGroupDrawerBase> groupDrawers = new Dictionary<Type, ToolboxGroupDrawerBase>();

        protected static Dictionary<Type, ToolboxPropertyDrawerBase> propertyDrawers = new Dictionary<Type, ToolboxPropertyDrawerBase>();

        protected static Dictionary<Type, ToolboxConditionDrawerBase> conditionDrawers = new Dictionary<Type, ToolboxConditionDrawerBase>();


        /// <summary>
        /// Initializes all needed <see cref="ToolboxDrawer"/>s using EditorSettings asset.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void InitializeEditor()
        {
            //find settings asset in whole asset database if needed
            var guids = AssetDatabase.FindAssets("t:ComponentEditorSettings");
            if (guids == null || guids.First() == null) return;
            var path = AssetDatabase.GUIDToAssetPath(guids.First());

            settings = AssetDatabase.LoadAssetAtPath(path, typeof(ComponentEditorSettings)) as ComponentEditorSettings;

            if (!settings) return;

            //local method used in drawer creation
            void CreateDrawer<T>(Type drawerType, Dictionary<Type, T> drawersCollection) where T : ToolboxDrawer
            {
                if (drawerType == null) return;
                //create desired drawer instance
                var drawer = Activator.CreateInstance(drawerType) as T;
                var targetType = GetTargetType(drawerType);
                if (drawersCollection.ContainsKey(targetType))
                {
                    Debug.LogWarning(targetType + " is already associated to more than one editor drawer.");
                    return;
                }
                drawersCollection.Add(targetType, drawer);
            }

            //local method used in search for proper target attributes
            Type GetTargetType(Type drawerType)
            {
                return drawerType.GetMethod("GetAttributeType", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, null) as Type;
            }

            //iterate over all assigned area drawers, create them and store
            for (var i = 0; i < settings.AreaDrawersCount; i++)
            {
                CreateDrawer(settings.GetAreaDrawerTypeAt(i), areaDrawers);
            }

            //iterate over all assigned group drawers, create them and store
            for (var i = 0; i < settings.GroupDrawersCount; i++)
            {
                CreateDrawer(settings.GetGroupDrawerTypeAt(i), groupDrawers);
            }

            //iterate over all assigned property drawers, create them and store
            for (var i = 0; i < settings.PropertyDrawersCount; i++)
            {
                CreateDrawer(settings.GetPropertyDrawerTypeAt(i), propertyDrawers);
            }

            //iterate over all assigned condition drawers, create them and store
            for (var i = 0; i < settings.ConditionDrawersCount; i++)
            {
                CreateDrawer(settings.GetConditionDrawerTypeAt(i), conditionDrawers);
            }
        }


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
        /// Handles desired property display process using standard way.
        /// </summary>
        /// <param name="property">Property to display.</param>
        protected virtual void HandleStandardProperty(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property, property.isExpanded);
        }

        /// <summary>
        /// Handles desired property display process using <see cref="ToolboxDrawer"/>s.
        /// </summary>
        /// <param name="property">Property to display.</param>
        protected virtual void HandleToolboxProperty(SerializedProperty property)
        {
            if (!propertyHandlers.ContainsKey(property.name))
            {
                propertyHandlers[property.name] = new PropertyHandler(property);
            }

            propertyHandlers[property.name].OnGui();
        }


        /// <summary>
        /// Inspector GUI re-draw call.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Component Editor", EditorStyles.centeredGreyMiniLabel);

            serializedObject.Update();
            var property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(property);
                EditorGUI.EndDisabledGroup();

                if (!settings || !settings.UseToolboxDrawers)
                {
                    while (property.NextVisible(false))
                    {
                        HandleStandardProperty(property.Copy());
                    }
                }
                else
                {
                    while (property.NextVisible(false))
                    {
                        HandleToolboxProperty(property.Copy());
                    }
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
                    if (areaDrawers.ContainsKey(areaAttributes[i].GetType()))
                    {
                        areaDrawers[areaAttributes[i].GetType()].OnGuiBegin(areaAttributes[i]);
                    }
                    else
                    {
                        Debug.LogError("Error - " + areaAttributes[i].GetType() + " is not supported. Assign it in ComponentEditorSettings.");
                    }
                }

                //handle condition attribute(only one allowed)
                var conditionState = PropertyCondition.Valid;
                if (conditionAttribute != null)
                {
                    if (conditionDrawers.ContainsKey(conditionAttribute.GetType()))
                    {
                        conditionState = conditionDrawers[conditionAttribute.GetType()].OnGuiValidate(property, conditionAttribute);
                    }
                    else
                    {
                        Debug.LogError("Error - " + conditionAttribute.GetType() + " is not supported. Assign it in ComponentEditorSettings.");
                    }
                }

                if (conditionState == PropertyCondition.NonValid)
                {
                    //end all area drawers without drawing property
                    for (var i = areaAttributes.Length - 1; i >= 0; i--)
                    {
                        if (areaDrawers.ContainsKey(areaAttributes[i].GetType()))
                        {
                            areaDrawers[areaAttributes[i].GetType()].OnGuiEnd(areaAttributes[i]);
                        }
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
                    if (propertyDrawers.ContainsKey(propertyAttribute.GetType()))
                    {
                        propertyDrawers[propertyAttribute.GetType()].OnGui(property, propertyAttribute);
                    }
                    else
                    {
                        Debug.LogError("Error - " + propertyAttribute.GetType() + " is not supported. Assign it in ComponentEditorSettings.");
                        EditorGUILayout.PropertyField(property, property.isExpanded);
                    }
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
                    if (areaDrawers.ContainsKey(areaAttributes[i].GetType()))
                    {
                        areaDrawers[areaAttributes[i].GetType()].OnGuiEnd(areaAttributes[i]);
                    }
                }
            }
        }
    }
}