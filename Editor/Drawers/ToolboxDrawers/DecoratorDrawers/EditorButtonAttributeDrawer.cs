using System;
using System.Collections;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;

namespace Toolbox.Editor.Drawers
{
    public class EditorButtonAttributeDrawer : ToolboxDecoratorDrawer<EditorButtonAttribute>
    {
        private bool IsClickable(ButtonActivityType activityType)
        {
            switch (activityType)
            {
                case ButtonActivityType.Everything:
                    return true;
                case ButtonActivityType.Nothing:
                    return false;
                case ButtonActivityType.OnEditMode:
                    return !Application.isPlaying;
                case ButtonActivityType.OnPlayMode:
                    return Application.isPlaying;
            }

            return true;
        }

        private bool IsCoroutine(Type targetType, MethodInfo method)
        {

            return method.ReturnType == typeof(IEnumerator);
        }


        protected override void OnGuiCloseSafe(EditorButtonAttribute attribute)
        {
            var targetObjects = InspectorUtility.CurrentTargetObjects;
            if (targetObjects == null || targetObjects.Length == 0)
            {
                //NOTE: something went really wrong, internal bug or OnGuiBeginSafe was called out of the Toolbox scope
                return;
            }

            var disable = !IsClickable(attribute.ActivityType);
            using (new EditorGUI.DisabledScope(disable))
            {
                var label = string.IsNullOrEmpty(attribute.ExtraLabel)
                    ? attribute.MethodName
                    : attribute.ExtraLabel;

                if (GUILayout.Button(label, Style.buttonStyle))
                {
                    var targetType = targetObjects[0].GetType();
                    var method = targetType.GetMethod(attribute.MethodName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (method == null)
                    {
                        ToolboxEditorLog.AttributeUsageWarning(attribute, attribute.MethodName + " method not found.");
                        return;
                    }

                    var parameters = method.GetParameters();
                    if (parameters.Length > 0)
                    {
                        ToolboxEditorLog.AttributeUsageWarning(attribute, attribute.MethodName + " method has to be parameterless.");
                        return;
                    }

                    var isCoroutine = IsCoroutine(targetType, method);
                    for (var i = 0; i < targetObjects.Length; i++)
                    {
                        var target = targetObjects[i];
                        if (target == null)
                        {
                            continue;
                        }

                        var result = method.Invoke(target, null);
                        if (isCoroutine) 
                        {
                            EditorCoroutineUtility.StartCoroutineOwnerless((IEnumerator)result);
                        }
                    }
                }
            }
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