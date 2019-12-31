//Custom reimplementation of an idea orginally provided here - https://github.com/marijnz/unity-toolbar-extender, 2019

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

//NOTE: since everything in this class is reflection-based it is a little bit "hacky"

namespace Toolbox.Editor
{
    /// <summary>
    /// Helper class to store toolbar button data.
    /// </summary>
    public class ToolbarButton
    {
        public ToolbarButton(Action onClickAction, GUIContent labelContent) : this(onClickAction, labelContent, 0)
        { }

        public ToolbarButton(Action onClickAction, GUIContent labelContent, int order)
        {
            Order = order;
            OnClickAction = onClickAction;
            LabelContent = labelContent;
        }


        public int Order { get; set; }

        public Action OnClickAction { get; private set; }

        public GUIContent LabelContent { get; private set; }
    }

    /// <summary>
    /// Toolbar extension which provides new funtionalites into classic Unity's scene toolbar.
    /// </summary>
    [InitializeOnLoad]
    public static class ToolboxEditorToolbar
    {
        private static readonly List<ToolbarButton> buttons = new List<ToolbarButton>();

        private static readonly Type containterType = typeof(IMGUIContainer);
        private static readonly Type toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static readonly Type guiViewType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GUIView");

        private static readonly FieldInfo onGuiHandler = containterType.GetField("m_OnGUIHandler",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly PropertyInfo visualTree = guiViewType.GetProperty("visualTree",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


        private static Object toolbar;


        static ToolboxEditorToolbar()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }


        private static void OnUpdate()
        {
            if (toolbar != null) return;

            //try to find toolbar object
            var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);

            toolbar = toolbars.Length > 0 ? toolbars[0] : null;

            if (toolbar == null) return;

            //get current toolbar containter using reflection
            var elements = visualTree.GetValue(toolbar, null) 
                as VisualElement;
#if UNITY_2019_1_OR_NEWER
            var container = elements[0];
#else
            var container = elements.First()
                as IMGUIContainer;
#endif

            //create additional gui handler for new elements
            var handler = onGuiHandler.GetValue(container) as Action;
            handler -= OnGuiHandler;
            handler += OnGuiHandler;
            onGuiHandler.SetValue(container, handler);
        }

        private static void OnGuiHandler()
        {
            const float fromToolsOffsetX = 400.0f;
#if UNITY_2019_1_OR_NEWER
            const float fromStripOffsetX = 150.0f;
#else
            const float fromStripOffsetX = 100.0f;
#endif

            if (buttons.Count == 0) return;

            var screenWidth = EditorGUIUtility.currentViewWidth;
            var screenHeight = Screen.height;

            var buttonsRect = new Rect(0, 0, screenWidth, screenHeight);
            //random calculations known from UnityCsReference
            buttonsRect.xMin += fromToolsOffsetX;
            buttonsRect.xMax = (screenWidth - fromStripOffsetX) / 2;
            //additional rect styling
            buttonsRect.xMin += Style.spacing;
            buttonsRect.xMax -= Style.spacing;
            buttonsRect.y += Style.padding;

            if (buttonsRect.width <= 0) return;

            //begin right drawing in calculated area
            GUILayout.BeginArea(buttonsRect);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //draw just one button in standard command style
            if (buttons.Count == 1)
            {
                var button = buttons[0];
                if (GUILayout.Button(button.LabelContent, Style.commandStyle))
                {
                    button.OnClickAction();
                }
            }
            //draw whole button strip
            else
            {
                var buttonLeft = buttons[0];
                var buttonRight = buttons[buttons.Count - 1];
                if (GUILayout.Button(buttonLeft.LabelContent, Style.commandLeftStyle))
                {
                    buttonLeft.OnClickAction();
                }
                for (var i = 1; i < buttons.Count - 1; i++)
                {
                    var button = buttons[i];
                    if (GUILayout.Button(button.LabelContent, Style.commandMidStyle))
                    {
                        button.OnClickAction();
                    }
                }
                if (GUILayout.Button(buttonRight.LabelContent, Style.commandRightStyle))
                {
                    buttonRight.OnClickAction();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }


        public static void AddToolbarButton(ToolbarButton button)
        {
            buttons.Add(button);
            buttons.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        public static void RemoveToolbarButton(ToolbarButton button)
        {
            buttons.Remove(button);
            buttons.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        public static void RemoveToolbarButtons()
        {
            buttons.Clear();
        }


        private static class Style
        {
            internal static readonly float padding = 5.0f;
            internal static readonly float spacing = 15.0f;

            internal static readonly GUIStyle commandStyle = new GUIStyle("Command")
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove
            };
            internal static readonly GUIStyle commandMidStyle = new GUIStyle("CommandMid")
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove
            };            
            internal static readonly GUIStyle commandLeftStyle = new GUIStyle("CommandLeft")
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove
            };
            internal static readonly GUIStyle commandRightStyle = new GUIStyle("CommandRight")
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove
            };
        }
    }
}