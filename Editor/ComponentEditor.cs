using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using UnityEditor;
using UnityEngine;
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

        protected static Dictionary<Type, ToolboxPropertyDrawerBase> propertyDrawers = new Dictionary<Type, ToolboxPropertyDrawerBase>();
 

        /// <summary>
        /// Initializes all needed <see cref="ToolboxPropertyDrawer{T}"/>s using EditorSettings asset.
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

            //local method used in search for proper target attributes
            Type GetTargetType(Type drawerType)
            {
                return drawerType.GetMethod("GetAttributeType", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, null) as Type;
            }

            //iterate over all assigned property drawers, create them and store
            for (var i = 0; i < settings.AreaDrawersCount; i++)
            {
                var type = settings.GetAreaDrawerTypeAt(i);
                if (type == null) continue;
                var drawer = Activator.CreateInstance(type) as ToolboxAreaDrawerBase;
                var targetType = GetTargetType(type);
                if (propertyDrawers.ContainsKey(targetType))
                {
                    Debug.LogWarning(targetType + " is already associated to more than one area drawer.");
                    continue;
                }
                areaDrawers.Add(targetType, drawer);
            }
            //iterate over all assigned property drawers, create them and store
            for (var i = 0; i < settings.PropertyDrawersCount; i++)
            {
                var type = settings.GetPropertyDrawerTypeAt(i);
                if (type == null) continue;
                var drawer = Activator.CreateInstance(type) as ToolboxPropertyDrawerBase;
                var targetType = GetTargetType(type);
                if (propertyDrawers.ContainsKey(targetType))
                {
                    Debug.LogWarning(targetType + " is already associated to more than one property drawer.");
                    continue;
                }
                propertyDrawers.Add(targetType, drawer);
            }
        }


        /// <summary>
        /// All available drawers setted from <see cref="ComponentEditorSettings"/>.
        /// </summary>
        protected List<OrderedDrawerBase> drawers = new List<OrderedDrawerBase>();

        /// <summary>
        /// All available and serialized fields(excluding children).
        /// </summary>
        protected List<SerializedProperty> properties = new List<SerializedProperty>();


        /// <summary>
        /// Editor initialization.
        /// </summary>
        protected virtual void OnEnable()
        {
            ReflectionUtility.GetAllFields(target, f => serializedObject.FindProperty(f.Name) != null)
                .ToList()
                .ForEach(f => properties.Add(serializedObject.FindProperty(f.Name)));
        }

        /// <summary>
        /// Editor deinitialization.
        /// </summary>
        protected virtual void OnDisable()
        { }

        /// <summary>
        /// Handles desired property display process. Starts drawing using all known and needed
        /// <see cref="ToolboxPropertyDrawer{T}"/>s or standard
        /// <see cref="EditorGUI.PropertyField(Rect, SerializedProperty)"/> method.
        /// </summary>
        /// <param name="property">Property to display.</param>
        protected virtual void HandleProperty(SerializedProperty property)
        {
            //use only standard property drawing system
            if (!settings || !settings.UseToolboxDrawers)
            {
                EditorGUILayout.PropertyField(property, property.isExpanded);
                return;
            }

            var areaAttributes = property.GetAttributes<ToolboxAreaAttribute>();
            Array.Sort(areaAttributes, (a1, a2) => a1.Order.CompareTo(a2.Order));
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
            //get property attribute(only one allowed)
            var propertyAttribute = property.GetAttribute<ToolboxPropertyAttribute>();
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

            //end all needed area drawers in proper order
            for (var i = areaAttributes.Length - 1; i >= 0; i--)
            {
                if (areaDrawers.ContainsKey(areaAttributes[i].GetType()))
                {
                    areaDrawers[areaAttributes[i].GetType()].OnGuiEnd(areaAttributes[i]);
                }
            }
        }


        /// <summary>
        /// Inspector GUI re-draw event.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Component Editor", EditorStyles.centeredGreyMiniLabel);

            Stopwatch sw = Stopwatch.StartNew();
            serializedObject.Update();
            var property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(property);
                EditorGUI.EndDisabledGroup();

                while (property.NextVisible(false))
                {
                    HandleProperty(property.Copy());
                }
            }
            sw.Stop();
            //Debug.Log(sw.ElapsedTicks);
            serializedObject.ApplyModifiedProperties();
        }
    }
}