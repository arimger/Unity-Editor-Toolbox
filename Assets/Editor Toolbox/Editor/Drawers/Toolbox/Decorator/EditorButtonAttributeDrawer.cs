using System.Collections;
using System.Reflection;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class EditorButtonAttributeDrawer : ToolboxDecoratorDrawer<EditorButtonAttribute>
    {
        private void DrawButton(EditorButtonAttribute attribute)
        {
            var declaringObjects = GetDeclaringObjects();
            if (declaringObjects == null || declaringObjects.Length == 0)
            {
                //NOTE: something went really wrong, internal bug or OnGuiBeginSafe was called out of the Toolbox scope
                return;
            }

            var disable = !IsClickable(attribute, declaringObjects);
            using (new EditorGUI.DisabledScope(disable))
            {
                var label = string.IsNullOrEmpty(attribute.ExtraLabel)
                            ? ObjectNames.NicifyVariableName(attribute.MethodName)
                            : attribute.ExtraLabel;
                var tooltip = attribute.Tooltip;
                var content = new GUIContent(label, tooltip);

                if (GUILayout.Button(content, Style.buttonStyle))
                {
                    CallMethods(attribute, declaringObjects);
                }
            }
        }

        private MethodInfo GetMethod(EditorButtonAttribute attribute, object[] targetObjects, string methodName)
        {
            var methodInfo = ReflectionUtility.GetMethod(methodName, targetObjects);
            if (methodInfo == null)
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, string.Format("{0} method not found.", methodName));
                return null;
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length > 0)
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, string.Format("{0} method has to be parameterless.", methodName));
                return null;
            }

            return methodInfo;
        }

        private static bool IsCoroutine(MethodInfo methodInfo)
        {
            return methodInfo.ReturnType == typeof(IEnumerator);
        }

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

        private bool IsClickable(EditorButtonAttribute attribute, object[] targetObjects)
        {
            if (!IsClickable(attribute.ActivityType))
            {
                return false;
            }

            var validateMethodName = attribute.ValidateMethodName;
            if (validateMethodName == null)
            {
                return true;
            }

            var validateMethodInfo = GetMethod(attribute, targetObjects, validateMethodName);
            if (validateMethodInfo == null)
            {
                return true;
            }

            var returnType = validateMethodInfo.ReturnType;
            if (returnType != typeof(bool))
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, "Validation method is invalid, return type has to be 'bool'.");
                return true;
            }

            for (var i = 0; i < targetObjects.Length; i++)
            {
                var target = targetObjects[i];
                if (target == null)
                {
                    continue;
                }

                if (!(bool)validateMethodInfo.Invoke(target, null))
                {
                    return false;
                }
            }

            return true;
        }

        private void CallMethods(EditorButtonAttribute attribute, object[] targetObjects)
        {
            var methodInfo = GetMethod(attribute, targetObjects, attribute.MethodName);
            if (methodInfo == null)
            {
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

        protected override void OnGuiBeginSafe(EditorButtonAttribute attribute)
        {
            if (attribute.PositionType != ButtonPositionType.Above)
            {
                return;
            }

            DrawButton(attribute);
        }

        protected override void OnGuiCloseSafe(EditorButtonAttribute attribute)
        {
            if (attribute.PositionType != ButtonPositionType.Default &&
                attribute.PositionType != ButtonPositionType.Below)
            {
                return;
            }

            DrawButton(attribute);
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