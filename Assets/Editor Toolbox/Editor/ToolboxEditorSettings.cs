using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Serialization;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;
    using Toolbox.Editor.Hierarchy;
    using Toolbox.Editor.Folders;

    internal interface IToolboxGeneralSettings
    { }

    internal interface IToolboxHierarchySettings
    {
        bool UseToolboxHierarchy { get; }
        bool DrawHorizontalLines { get; }
        bool ShowSelectionsCount { get; }

        List<HierarchyItemDataType> RowDataTypes { get; }
    }

    internal interface IToolboxProjectSettings
    {
        bool UseToolboxProject { get; }

        float LargeIconScale { get; }
        float SmallIconScale { get; }
        Vector2 LargeIconPadding { get; }
        Vector2 SmallIconPadding { get; }

        List<FolderData> CustomFolders { get; }
    }

    internal interface IToolboxInspectorSettings
    {
        void SetAllPossibleDecoratorDrawers();
        void SetAllPossibleConditionDrawers();
        void SetAllPossibleSelfPropertyDrawers();
        void SetAllPossibleListPropertyDrawers();
        void SetAllPossibleTargetTypeDrawers();

        bool UseToolboxDrawers { get; }

        List<SerializedType> DecoratorDrawerHandlers { get; }
        List<SerializedType> ConditionDrawerHandlers { get; }
        List<SerializedType> SelfPropertyDrawerHandlers { get; }
        List<SerializedType> ListPropertyDrawerHandlers { get; }
        List<SerializedType> TargetTypeDrawerHandlers { get; }
    }

    [CreateAssetMenu(fileName = "Editor Toolbox Settings", menuName = "Editor Toolbox/Settings")]
    internal class ToolboxEditorSettings : ScriptableObject, IToolboxGeneralSettings, IToolboxHierarchySettings, IToolboxProjectSettings, IToolboxInspectorSettings
    {
        [SerializeField]
        private bool useToolboxHierarchy = true;
        [SerializeField]
        private bool drawHorizontalLines = true;
        [SerializeField]
        private bool showSelectionsCount;

        [SerializeField, ReorderableList(ListStyle.Boxed)]
        [FormerlySerializedAs("rowDataItems")]
        private List<HierarchyItemDataType> rowDataTypes = Defaults.rowDataTypes;

        [SerializeField]
        private bool useToolboxFolders = true;

        [SerializeField, Clamp(0.0f, float.MaxValue)]
        private float largeIconScale = Defaults.largeFolderIconScaleDefault;
        [SerializeField, Clamp(0.0f, float.MaxValue)]
        private float smallIconScale = Defaults.smallFolderIconScaleDefault;

        [SerializeField]
        private Vector2 largeIconPadding = new Vector2(Defaults.largeFolderIconXPaddingDefault, Defaults.largeFolderIconYPaddingDefault);
        [SerializeField]
        private Vector2 smallIconPadding = new Vector2(Defaults.smallFolderIconXPaddingDefault, Defaults.smallFolderIconYPaddingDefault);

        [SerializeField, ReorderableList(ListStyle.Boxed)]
        private List<FolderData> customFolders = new List<FolderData>();

        [SerializeField]
        private bool useToolboxDrawers = true;

        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxDecoratorDrawer<>))]
        private List<SerializedType> decoratorDrawerHandlers = new List<SerializedType>();
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxConditionDrawer<>))]
        private List<SerializedType> conditionDrawerHandlers = new List<SerializedType>();
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxSelfPropertyDrawer<>))]
        private List<SerializedType> selfPropertyDrawerHandlers = new List<SerializedType>();
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxListPropertyDrawer<>))]
        private List<SerializedType> listPropertyDrawerHandlers = new List<SerializedType>();

        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxTargetTypeDrawer))]
        private List<SerializedType> targetTypeDrawerHandlers = new List<SerializedType>();

        private bool hierarchySettingsDirty;
        private bool projectSettingsDirty;
        private bool inspectorSettingsDirty;

        internal event Action<IToolboxHierarchySettings> OnHierarchySettingsChanged;
        internal event Action<IToolboxProjectSettings> OnProjectSettingsChanged;
        internal event Action<IToolboxInspectorSettings> OnInspectorSettingsChanged;


        #region Methods: Internal/data validation

        /// <summary>
        /// Forces Hierarchy settings validation in the next <see cref="OnValidate"/> call.
        /// </summary>
        internal void SetHierarchySettingsDirty()
        {
            hierarchySettingsDirty = true;
        }

        /// <summary>
        /// Forces Project settings validation in the next <see cref="OnValidate"/> call.
        /// </summary>
        internal void SetProjectSettingsDirty()
        {
            projectSettingsDirty = true;
        }

        /// <summary>
        /// Forces Inspector settings validation in the next <see cref="OnValidate"/> call.
        /// </summary>
        internal void SetInspectorSettingsDirty()
        {
            inspectorSettingsDirty = true;
        }


        internal void ValidateHierarchySettings()
        {
            OnHierarchySettingsChanged?.Invoke(this);
        }

        internal void ValidateProjectSettings()
        {
            OnProjectSettingsChanged?.Invoke(this);
        }

        internal void ValidateInspectorSettings()
        {
            OnInspectorSettingsChanged?.Invoke(this);
        }

        internal void Validate()
        {
            ValidateHierarchySettings();
            ValidateProjectSettings();
            ValidateInspectorSettings();
        }


        /// <summary>
        /// Called internally by the Editor after any value change or the Undo/Redo operation.
        /// </summary>
        private void OnValidate()
        {
            //determine if any section was changed within the Editor
            var settingsDirty = hierarchySettingsDirty || projectSettingsDirty || inspectorSettingsDirty;
            if (settingsDirty)
            {
                //check exactly what settings are changed and apply them
                if (hierarchySettingsDirty)
                {
                    ValidateHierarchySettings();
                }

                if (projectSettingsDirty)
                {
                    ValidateProjectSettings();
                }

                if (inspectorSettingsDirty)
                {
                    ValidateInspectorSettings();
                }
            }
            else
            {
                //otherwise, that means:
                // - Undo/Redo action is performed
                // - this is the very first event
                // - called internally by any class
                Validate();
            }

            //clear additional flags
            hierarchySettingsDirty = false;
            projectSettingsDirty = false;
            inspectorSettingsDirty = false;
        }

        #endregion


        public void SetAllPossibleDecoratorDrawers()
        {
            decoratorDrawerHandlers.Clear();
            var types = ToolboxDrawerModule.GetAllPossibleDecoratorDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                decoratorDrawerHandlers.Add(new SerializedType(types[i]));
            }
        }

        public void SetAllPossibleConditionDrawers()
        {
            conditionDrawerHandlers.Clear();
            var types = ToolboxDrawerModule.GetAllPossibleConditionDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                conditionDrawerHandlers.Add(new SerializedType(types[i]));
            }
        }

        public void SetAllPossibleSelfPropertyDrawers()
        {
            selfPropertyDrawerHandlers.Clear();
            var types = ToolboxDrawerModule.GetAllPossibleSelfPropertyDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                selfPropertyDrawerHandlers.Add(new SerializedType(types[i]));
            }
        }

        public void SetAllPossibleListPropertyDrawers()
        {
            listPropertyDrawerHandlers.Clear();
            var types = ToolboxDrawerModule.GetAllPossibleListPropertyDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                listPropertyDrawerHandlers.Add(new SerializedType(types[i]));
            }
        }

        public void SetAllPossibleTargetTypeDrawers()
        {
            targetTypeDrawerHandlers.Clear();
            var types = ToolboxDrawerModule.GetAllPossibleTargetTypeDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                targetTypeDrawerHandlers.Add(new SerializedType(types[i]));
            }
        }


        public void ResetIconRectProperties()
        {
            largeIconScale = Defaults.largeFolderIconScaleDefault;
            smallIconScale = Defaults.smallFolderIconScaleDefault;

            largeIconPadding = new Vector2(Defaults.largeFolderIconXPaddingDefault,
                Defaults.largeFolderIconYPaddingDefault);
            smallIconPadding = new Vector2(Defaults.smallFolderIconXPaddingDefault,
                Defaults.smallFolderIconYPaddingDefault);
        }


        public bool UseToolboxHierarchy
        {
            get => useToolboxHierarchy;
            set => useToolboxHierarchy = value;
        }

        public List<HierarchyItemDataType> RowDataTypes
        {
            get => rowDataTypes;
            set => rowDataTypes = value;
        }

        public bool DrawHorizontalLines
        {
            get => drawHorizontalLines;
            set => drawHorizontalLines = value;
        }

        public bool ShowSelectionsCount
        {
            get => showSelectionsCount;
            set => showSelectionsCount = value;
        }

        public bool UseToolboxProject
        {
            get => useToolboxFolders;
            set => useToolboxFolders = value;
        }

        public float LargeIconScale
        {
            get => largeIconScale;
            set => largeIconScale = value;
        }

        public float SmallIconScale
        {
            get => smallIconScale;
            set => smallIconScale = value;
        }

        public Vector2 LargeIconPadding
        {
            get => largeIconPadding;
            set => largeIconPadding = value;
        }

        public Vector2 SmallIconPadding
        {
            get => smallIconPadding;
            set => smallIconPadding = value;
        }

        public List<FolderData> CustomFolders
        {
            get => customFolders;
            set => customFolders = value;
        }

        public bool UseToolboxDrawers
        {
            get => useToolboxDrawers;
            set => useToolboxDrawers = value;
        }

        public List<SerializedType> DecoratorDrawerHandlers
        {
            get => decoratorDrawerHandlers;
            set => decoratorDrawerHandlers = value;
        }

        public List<SerializedType> ConditionDrawerHandlers
        {
            get => conditionDrawerHandlers;
            set => conditionDrawerHandlers = value;
        }

        public List<SerializedType> SelfPropertyDrawerHandlers
        {
            get => selfPropertyDrawerHandlers;
            set => selfPropertyDrawerHandlers = value;
        }

        public List<SerializedType> ListPropertyDrawerHandlers
        {
            get => listPropertyDrawerHandlers;
            set => listPropertyDrawerHandlers = value;
        }

        public List<SerializedType> TargetTypeDrawerHandlers
        {
            get => targetTypeDrawerHandlers;
            set => targetTypeDrawerHandlers = value;
        }


        private static class Defaults
        {
            internal const float largeFolderIconScaleDefault = 0.8f;
            internal const float smallFolderIconScaleDefault = 0.7f;

            internal const float largeFolderIconXPaddingDefault = 0.0f;
            internal const float largeFolderIconYPaddingDefault = 0.15f;
            internal const float smallFolderIconXPaddingDefault = 0.15f;
            internal const float smallFolderIconYPaddingDefault = 0.15f;

            internal readonly static List<HierarchyItemDataType> rowDataTypes = new List<HierarchyItemDataType>()
            {
                HierarchyItemDataType.Icon,
                HierarchyItemDataType.Toggle,
                HierarchyItemDataType.Tag,
                HierarchyItemDataType.Layer,
                HierarchyItemDataType.Script
            };
        }
    }
}