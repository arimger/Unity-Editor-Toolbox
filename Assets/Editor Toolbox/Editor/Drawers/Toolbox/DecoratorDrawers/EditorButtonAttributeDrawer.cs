﻿using System.Collections;
using System.Reflection;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class EditorButtonAttributeDrawer : ToolboxDecoratorDrawer<EditorButtonAttribute>
    {
        private bool IsCoroutine(MethodInfo method)
        {
            return method.ReturnType == typeof(IEnumerator);
        }

        private void CallMethods(EditorButtonAttribute attribute, Object[] targetObjects)
        {
            var methodInfo = ReflectionUtility.GetObjectMethod(attribute.MethodName, targetObjects);
            //validate method name (check if method exists)
            if (methodInfo == null)
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute,
                    string.Format("{0} method not found.", attribute.MethodName));
                return;
            }

            //validate parameters count and log warning
            var parameters = methodInfo.GetParameters();
            if (parameters.Length > 0)
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute,
                    string.Format("{0} method has to be parameterless.", attribute.MethodName));
                return;
            }

            //invoke method for all selected components
            var isCoroutine = IsCoroutine(methodInfo);
            for (var i = 0; i < targetObjects.Length; i++)
            {
                var target = targetObjects[i];
                if (target == null)
                {
                    continue;
                }

                var result = methodInfo.Invoke(target, null);
                //additionaly run Coroutine if possible
                if (isCoroutine)
                {
                    EditorCoroutineUtility.StartCoroutineOwnerless((IEnumerator)result);
                }
            }
        }


        protected override void OnGuiCloseSafe(EditorButtonAttribute attribute)
        {
            var targetObjects = InspectorUtility.CurrentTargetObjects;
            if (targetObjects == null || targetObjects.Length == 0)
            {
                //NOTE: something went really wrong, internal bug or OnGuiBeginSafe was called out of the Toolbox scope
                return;
            }

            using (new EditorGUI.DisabledScope(IsInteractive(attribute, targetObjects)))
            {
                var label = string.IsNullOrEmpty(attribute.ExtraLabel)
                    ? attribute.MethodName
                    : attribute.ExtraLabel;
                var tooltip = attribute.Tooltip;
                var content = new GUIContent(label, tooltip);

                if (GUILayout.Button(content, Style.buttonStyle))
                {
                    CallMethods(attribute, targetObjects);
                }
            }
        }

        bool IsInteractive(EditorButtonAttribute attribute, params Object[] targetObjects)
        {
            bool interactive = true;
            if (!string.IsNullOrEmpty(attribute.InteractionMethodName))
            {
                var interactionMethod =
                    ReflectionUtility.GetObjectMethod(attribute.InteractionMethodName, targetObjects);
                if (interactionMethod != null)
                {
                    for (int i = 0; i < targetObjects.Length; i++)
                    {
                        interactive &= (bool)interactionMethod.Invoke(targetObjects[i], null);
                    }
                }
            }

            return !interactive;
        }


        private static class Style
        {
            internal static readonly GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                richText = true
            };
        }
    }
}