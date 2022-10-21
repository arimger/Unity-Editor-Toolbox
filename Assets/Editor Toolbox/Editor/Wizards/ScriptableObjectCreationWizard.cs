using System;
using System.IO;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor.Wizards
{
    using Toolbox.Editor.Internal;

    /// <summary>
    /// Utility window responsible for creation of <see cref="ScriptableObject"/>s.
    /// Allows to create multiple <see cref="ScriptableObject"/>s of the same <see cref="Type"/>.
    /// </summary>
    public class ScriptableObjectCreationWizard : ToolboxWizard
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

        [Serializable]
        private class CreationData
        {
            private bool IsDefaultObjectValid()
            {
                return DefaultObject != null && DefaultObject.GetType() == InstanceType;
            }

            public void Validate()
            {
                if (string.IsNullOrEmpty(InstanceName))
                {
                    InstanceName = InstanceType?.Name;
                }

                InstancesCount = Mathf.Max(InstancesCount, 1);
                if (!IsDefaultObjectValid())
                {
                    DefaultObject = null;
                }
            }

            [field: SerializeField]
            public Type InstanceType { get; set; }
            [field: SerializeField]
            public string InstanceName { get; set; }
            [field: SerializeField]
            public int InstancesCount { get; set; } = 1;
            [field: SerializeField, InLineEditor]
            [field: Tooltip("Will be used as a blueprint for all created ScriptableObjects.")]
            public Object DefaultObject { get; set; }
        }

        private static readonly TypeConstraintContext sharedConstraint = new TypeConstraintScriptableObject();
        private static readonly TypeAppearanceContext sharedAppearance = new TypeAppearanceContext(sharedConstraint, TypeGrouping.None, true);
        private static readonly TypeField typeField = new TypeField(sharedConstraint, sharedAppearance);

        private readonly CreationData data = new CreationData();

        [MenuItem("Assets/Create/Toolbox/ScriptableObject Creation Wizard", priority = 9)]
        internal static void Initialize()
        {
            var window = GetWindow<ScriptableObjectCreationWizard>();
            window.titleContent = new GUIContent("ScriptableObject Creation Window");
            window.Show();
        }

        private void DrawSettingsPanel()
        {
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            var rect = EditorGUILayout.GetControlRect(true);
            typeField.OnGui(rect, true, OnTypeSelected, data.InstanceType);
            if (data.InstanceType == null)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            data.InstanceName = EditorGUILayout.TextField("Instance Name", data.InstanceName);
            data.InstancesCount = EditorGUILayout.IntField("Instances To Create", data.InstancesCount);
            var content = new GUIContent("Default Object", "Will be used as a blueprint for all created ScriptableObjects.");
            data.DefaultObject = EditorGUILayout.ObjectField(content, data.DefaultObject, data.InstanceType, false);
            if (EditorGUI.EndChangeCheck())
            {
                OnWizardUpdate();
            }
        }

        private void CreateObjects()
        {
            CreateObjects(data);
        }

        private void CreateObjects(CreationData data)
        {
            data.Validate();
            if (data.InstanceType == null)
            {
                ToolboxEditorLog.LogWarning("Cannot create ScriptableObjects, picked type is null.");
                return;
            }

            var assetPath = GetActiveFolderPath();
            if (string.IsNullOrEmpty(assetPath))
            {
                ToolboxEditorLog.LogWarning("Cannot create ScriptableObjects, path cached from the Project Window is invalid.");
                return;
            }

            var instancesCount = data.InstancesCount;
            for (var i = 0; i < instancesCount; i++)
            {
                var instance = CreateObject(data.InstanceType, data.DefaultObject);
                CreateAsset(instance, data.InstanceName, assetPath, i);
            }

            AssetDatabase.SaveAssets();
            ToolboxEditorLog.LogInfo($"New ScriptableObjects created ({instancesCount}), at path: {assetPath}.");
        }

        private Object CreateObject(Type targetType, Object defaultObject)
        {
            return defaultObject != null ? Instantiate(defaultObject) : CreateInstance(targetType);
        }

        private void CreateAsset(Object asset, string assetName, string assetPath, int index)
        {
            var instanceName = assetName + (index > 0 ? $" [{index}]" : string.Empty);
            var instancePath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(assetPath, $"{instanceName}.asset"));
            AssetDatabase.CreateAsset(asset, instancePath);
        }

        private void OnTypeSelected(Type type)
        {
            data.InstanceType = type;
            var attribute = type?.GetCustomAttribute<CreateAssetMenuAttribute>();
            if (attribute != null)
            {
                data.InstanceName = attribute.fileName;
            }
            else
            {
                data.InstanceName = string.Empty;
            }
        }

        private static string GetActiveFolderPath()
        {
            var projectWindowUtilType = typeof(ProjectWindowUtil);
            var getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            var obj = getActiveFolderPath.Invoke(null, new object[0]);
            var pathToCurrentFolder = obj.ToString();
            return pathToCurrentFolder;
        }

        protected override void OnWizardCreate()
        {
            base.OnWizardCreate();
            CreateObjects();
        }

        protected override void OnWizardUpdate()
        {
            base.OnWizardUpdate();
            data.Validate();
        }

        protected override void OnWizardGui()
        {
            base.OnWizardGui();
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawSettingsPanel();
            }
        }

        protected override bool CloseOnCreate => false;
    }
}