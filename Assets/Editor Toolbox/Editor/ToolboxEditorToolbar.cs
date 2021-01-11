//Custom reimplementation of an idea orginally provided here - https://github.com/marijnz/unity-toolbar-extender, 2019

using System;
using System.Collections;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Unity.EditorCoroutines.Editor;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

//NOTE: since everything in this class is reflection-based it is a little bit "hacky"

namespace Toolbox.Editor
{
    /// <summary>
    /// Toolbar extension which provides new funtionalites into classic Unity's scene toolbar.
    /// </summary>
    [InitializeOnLoad]
    public static class ToolboxEditorToolbar
    {
        static ToolboxEditorToolbar()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(Initialize());
        }


        private static readonly Type containterType = typeof(IMGUIContainer);
        private static readonly Type toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static readonly Type guiViewType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GUIView");
#if UNITY_2020_1_OR_NEWER
        private static readonly Type backendType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.IWindowBackend");

        private static readonly PropertyInfo guiBackend = guiViewType.GetProperty("windowBackend",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly PropertyInfo visualTree = backendType.GetProperty("visualTree",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
        private static readonly PropertyInfo visualTree = guiViewType.GetProperty("visualTree",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        private static readonly FieldInfo onGuiHandler = containterType.GetField("m_OnGUIHandler",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private static Object toolbar;


        private static IEnumerator Initialize()
        {
            while (toolbar == null)
            {
                //try to find aldready created Toolbar object
                var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
                if (toolbars == null || toolbars.Length == 0)
                {
                    yield return null;
                    continue;
                }
                else
                {
                    toolbar = toolbars[0];
                }
            }
#if UNITY_2020_1_OR_NEWER
            var backend = guiBackend.GetValue(toolbar);
            var elements = visualTree.GetValue(backend, null) as VisualElement;
#else
            var elements = visualTree.GetValue(toolbar, null) as VisualElement;
#endif

#if UNITY_2019_1_OR_NEWER
            var container = elements[0];
#else
            var container = elements[0] as IMGUIContainer;
#endif
            //create additional gui handler for new elements
            var handler = onGuiHandler.GetValue(container) as Action;
            handler -= OnGui;
            handler += OnGui;
            onGuiHandler.SetValue(container, handler);
        }

        private static void OnGui()
        {
            if (!IsToolbarAllowed || OnToolbarGui == null)
            {
                return;
            }

            var screenWidth = EditorGUIUtility.currentViewWidth;
            var toolbarRect = new Rect(0, 0, screenWidth, Style.rowHeight);
            //calculations known from UnityCsReference
            toolbarRect.xMin += FromToolsOffset;
            toolbarRect.xMax = (screenWidth - FromStripOffset) / 2;
            //additional rect styling
            toolbarRect.xMin += Style.spacing;
            toolbarRect.xMax -= Style.spacing;
            toolbarRect.yMin += Style.topPadding;
            toolbarRect.yMax -= Style.botPadding;

            if (toolbarRect.width <= 0)
            {
                return;
            }

            //begin drawing in calculated area
            using (new GUILayout.AreaScope(toolbarRect))
            {
                using (new GUILayout.HorizontalScope())
                {
                    OnToolbarGui?.Invoke();
                }
            }
        }


        public static bool IsToolbarAllowed { get; set; } = true;

        public static float FromToolsOffset { get; set; } = 400.0f;
        public static float FromStripOffset { get; set; } = 150.0f;


        public static event Action OnToolbarGui;


        private static class Style
        {
            internal static readonly float rowHeight = 30.0f;
            internal static readonly float spacing = 15.0f;
            internal static readonly float topPadding = 5.0f;
            internal static readonly float botPadding = 3.0f;
        }
    }
}