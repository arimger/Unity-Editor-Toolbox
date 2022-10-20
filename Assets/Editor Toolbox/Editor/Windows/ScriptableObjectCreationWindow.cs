using System;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor.Windows
{
    using Toolbox.Editor.Internal;

    public class ScriptableObjectCreationWindow : EditorWindow
    {
        private class TypeConstraintScriptableObject : TypeConstraintStandard
        {
            public TypeConstraintScriptableObject() : base(typeof(ScriptableObject), TypeSettings.Class, false, false)
            { }


            public override bool IsSatisfied(Type type)
            {
                return Attribute.IsDefined(type, typeof(CreateAssetMenuAttribute)) && base.IsSatisfied(type);
            }
        }


        private static readonly TypeConstraintContext sharedConstraint = new TypeConstraintScriptableObject();
        private static readonly TypeAppearanceContext sharedAppearance = new TypeAppearanceContext(sharedConstraint, TypeGrouping.None, true);
        private static readonly TypeField typeField = new TypeField(sharedConstraint, sharedAppearance);

        private Type activeType;
        private int instancesCount = 1;
        private string baseName;
        private Object defaultObject;

        [MenuItem("Assets/Create/Toolbox/ScriptableObject Creation Window", priority = 5)]
        internal static void Initialize()
        {
            var window = GetWindow<ScriptableObjectCreationWindow>();
            window.titleContent = new GUIContent("ScriptableObject Creation Window");
            window.Show();
        }


        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawSettingsPanel();
            }

            DrawCreatePanel();
        }


        private void DrawSettingsPanel()
        {
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            var rect = EditorGUILayout.GetControlRect(true);
            typeField.OnGui(rect, true, OnTypeSelected, activeType);
            if (activeType == null)
            {
                return;
            }

            instancesCount = EditorGUILayout.IntField("Instances To Create", instancesCount);
            instancesCount = Mathf.Min(instancesCount, 1);
            baseName = EditorGUILayout.TextField("Base Name", baseName);
            EditorGUI.BeginChangeCheck();
            defaultObject = EditorGUILayout.ObjectField(new GUIContent("Default Object"), defaultObject, activeType, false);
            if (EditorGUI.EndChangeCheck())
            {
                //TODO: validate default Object
            }
        }

        private void DrawCreatePanel()
        {
            if (GUILayout.Button("Create"))
            {
                CreateObjects();
            }
        }

        private void CreateObjects()
        {
            CreateObjects(activeType, defaultObject);
        }

        private void CreateObjects(Type targetType, Object defaultObject)
        {
            //TODO: validate default Object
            for (var i = 0; i < instancesCount; i++)
            {
                //TODO: instantiate SOs
            }
        }

        private Object CreateObject(Type targetType, Object defaultObject)
        {
            return defaultObject != null ? Instantiate(defaultObject) : CreateInstance(targetType);
        }

        private static string GetActiveFolderPath()
        {
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            string pathToCurrentFolder = obj.ToString();
            return pathToCurrentFolder;
        }

        private void OnTypeSelected(Type type)
        {
            activeType = type;
            var attribute = type?.GetCustomAttribute<CreateAssetMenuAttribute>();
            if (attribute != null)
            {
                baseName = attribute.fileName;
            }
            else
            {
                baseName = string.Empty;
            }
        }
    }
}