using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;
    using Object = UnityEngine.Object;

    [InitializeOnLoad]
    internal static partial class InspectorUtility
    {
        static InspectorUtility()
        {
            //we can use each new Editor to check if 'OnEditorReload' should be called
            ToolboxEditor.OnBeginToolboxEditor += CheckReloads;

            //we can use 'OnBeginToolboxEditor' and 'OnCloseToolboxEditor' to cache 
            //processed Editors and determine the real context of the serialization
            ToolboxEditor.OnBeginToolboxEditor += OnBeginEditor;
            ToolboxEditor.OnBreakToolboxEditor += OnBreakEditor;
            ToolboxEditor.OnCloseToolboxEditor += OnCloseEditor;
        }


        private static int lastCachedEditorId;
        private static Editor lastCachedEditor;
        private static readonly Stack<Editor> cachedEditors = new Stack<Editor>();


        private static void OnBeginEditor(Editor editor)
        {
            cachedEditors.Push(editor);
        }

        private static void OnBreakEditor(Editor editor)
        {
            cachedEditors.Clear();
        }

        private static void OnCloseEditor(Editor editor)
        {
            if (InToolboxEditor)
            {
                cachedEditors.Pop();
            }
        }

        private static void CheckReloads(Editor editor)
        {
            //NOTE: it means that last Editor was null or disposed, anyway we probably want to reload drawers-related cache
            if (lastCachedEditor == null || lastCachedEditorId != lastCachedEditor.GetInstanceID())
            {
                lastCachedEditor = editor;
                lastCachedEditorId = editor.GetInstanceID();
                OnEditorReload?.Invoke();
            }
        }


        /// <summary>
        /// Forces available Inspector Windows to repaint. 
        /// </summary>
        internal static void RepaintInspectors()
        {
            var windows = Resources.FindObjectsOfTypeAll(typeof(Editor)
                                   .Assembly
                                   .GetType("UnityEditor.InspectorWindow"));
            if (windows == null || windows.Length == 0)
            {
                return;
            }

            for (var i = 0; i < windows.Length; i++)
            {
                var window = windows[i] as EditorWindow;
                if (window)
                {
                    window.Repaint();
                }
            }

            //NOTE: none reflection way
            //UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        /// <summary>
        /// Returns the visibility of the <see cref="Editor"/>.
        /// </summary>
        internal static bool GetIsEditorExpanded(Editor editor)
        {
            return InternalEditorUtility.GetIsInspectorExpanded(editor.target);
        }

        /// <summary>
        /// Adjusts the visibility of the <see cref="Editor"/> using provided value.
        /// </summary>
        internal static void SetIsEditorExpanded(Editor editor, bool value)
        {
            InternalEditorUtility.SetIsInspectorExpanded(editor.target, true);
            //NOTE: in older versions Editor's foldouts are based on the m_IsVisible field and the Awake() method
#if !UNITY_2019_1_OR_NEWER
            const string isVisibleFieldName = "m_IsVisible";
            var isVisible = editor.GetType().GetField(isVisibleFieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (isVisible != null)
            {
                isVisible.SetValue(editor, true);
            }
#endif
        }

        /// <summary>
        /// Simulates OnValidate broadcast call on the target object.
        /// </summary>
        internal static void SimulateOnValidate(Object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var methodInfo = target.GetType().GetMethod("OnValidate",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null, CallingConventions.Any, new Type[0], null);
            if (methodInfo != null)
            {
                methodInfo.Invoke(target, null);
            }
        }


        /// <summary>
        /// Event fired every time when <see cref="ToolboxEditor"/>s were re-created.
        /// </summary>
        internal static event Action OnEditorReload;


        /// <summary>
        /// Last cached targetObjects from the currently processed <see cref="ToolboxEditor"/>.
        /// </summary>
        internal static Object[] CurrentTargetObjects
        {
            get => cachedEditors.Count > 0 ? cachedEditors.Peek().targets : new Object[0];
        }

        internal static bool InToolboxEditor
        {
            get => cachedEditors.Count > 0;
        }
    }

    internal static partial class InspectorUtility
    {
        private static readonly List<Component> copiedComponents = new List<Component>();


        [MenuItem("CONTEXT/Component/Copy Components", false, priority = 200)]
        private static void Copy()
        {
            copiedComponents.Clear();
            var selectedGameObjects = Selection.gameObjects;

            for (var i = 0; i < selectedGameObjects.Length; i++)
            {
                var componentsOfSelectedTransforms = selectedGameObjects[i].GetComponents<Component>();

                for (var j = 0; j < componentsOfSelectedTransforms.Length; j++)
                {
                    if (componentsOfSelectedTransforms[j] is Transform)
                    {
                        continue;
                    }

                    if (componentsOfSelectedTransforms[j] == null || copiedComponents.Contains(componentsOfSelectedTransforms[j]))
                    {
                        continue;
                    }

                    copiedComponents.Add(componentsOfSelectedTransforms[j]);
                }
            }
        }

        [MenuItem("CONTEXT/Component/Copy Components", true)]
        private static bool ValidateCopy()
        {
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem("CONTEXT/Component/Paste Components", false, priority = 201)]
        private static void Paste()
        {
            var selectedGameobjects = Selection.gameObjects;

            for (int i = 0; i < selectedGameobjects.Length; i++)
            {
                var gameObject = selectedGameobjects[i];
                Undo.RecordObjects(selectedGameobjects, "Copy components to selected GameObjects");

                for (int j = 0; j < copiedComponents.Count; j++)
                {
                    var component = copiedComponents[j];
                    ComponentUtility.CopyComponent(component);
                    ComponentUtility.PasteComponentAsNew(gameObject);
                }
            }

            copiedComponents.Clear();
        }

        [MenuItem("CONTEXT/Component/Paste Components", true)]
        private static bool ValidatePaste()
        {
            return Selection.gameObjects.Length > 0 && copiedComponents.Count > 0;
        }

        [MenuItem("CONTEXT/Component/Hide Component", false, priority = 300)]
        private static void Hide(MenuCommand menuCommand)
        {
            var component = menuCommand.context as Component;
            var components = component.gameObject.GetComponents<Component>();

            //validate if this is the last visible component
            var visibleCount = 0;
            for (var i = 0; i < components.Length; i++)
            {
                var c = components[i];
                if (!c.hideFlags.HasFlag(HideFlags.HideInInspector))
                {
                    visibleCount++;
                }
            }

            //show display dialog and make sure user knows consequences 
            if (visibleCount <= 1 && !EditorUtility.DisplayDialog("Hide Warning",
                                                                  "Do you really want to hide the last visible component?", "Yes", "Cancel"))
            {
                return;
            }

            component.hideFlags |= HideFlags.HideInInspector;

#if UNITY_2019_3_OR_NEWER
            //force component to be dirst (will cause additional repaint for new releases)
            EditorUtility.SetDirty(component);
#endif
            //repaint the Editor
            InternalEditorUtility.RepaintAllViews();
        }

        [Obsolete]
        private static void HideAll(MenuCommand menuCommand)
        {
            var gameObject = (menuCommand.context as Component).gameObject;
            var components = (menuCommand.context as Component).GetComponents<Component>();

            Undo.RecordObject(gameObject, "Hide all components in " + gameObject.name);

            for (var i = 0; i < components.Length; i++)
            {
                components[i].hideFlags |= HideFlags.HideInInspector;
            }

            InternalEditorUtility.RepaintAllViews();
        }

        [Obsolete]
        private static void ShowAll(MenuCommand menuCommand)
        {
            var gameObject = (menuCommand.context as Component).gameObject;
            var components = (menuCommand.context as Component).GetComponents<Component>();

            Undo.RecordObject(gameObject, "Show all components in " + gameObject.name);

            for (var i = 0; i < components.Length; i++)
            {
                components[i].hideFlags &= ~HideFlags.HideInInspector;
            }

            InternalEditorUtility.RepaintAllViews();
        }
    }
}