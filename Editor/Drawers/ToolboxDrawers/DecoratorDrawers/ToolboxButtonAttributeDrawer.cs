using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [System.Obsolete]
    public class ToolboxButtonAttributeDrawer : ToolboxDecoratorDrawer<ToolboxButtonAttribute>
    {
        protected override void OnGuiBeginSafe(ToolboxButtonAttribute attribute)
        {
            var targetObjects = InspectorUtility.CurrentTargetObjects;
            if (targetObjects == null || targetObjects.Length == 0)
            {
                return;
            }

            var isValid = true;
            var label = string.IsNullOrEmpty(attribute.Label) ? attribute.MethodName : attribute.Label;

            switch (attribute.Type)
            {
                case ButtonActivityType.Everything:
                    isValid = true;
                    break;
                case ButtonActivityType.Nothing:
                    isValid = false;
                    break;
                case ButtonActivityType.OnEditMode:
                    isValid = !Application.isPlaying;
                    break;
                case ButtonActivityType.OnPlayMode:
                    isValid = Application.isPlaying;
                    break;
            }

            EditorGUI.BeginDisabledGroup(!isValid);
            if (GUILayout.Button(label))
            {
                var targetType = targetObjects[0].GetType();
                var method = targetType.GetMethod(attribute.MethodName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (method != null)
                {
                    for (var i = 0; i < targetObjects.Length; i++)
                    {
                        var target = targetObjects[i];
                        if (target == null)
                        {
                            continue;
                        }

                        method.Invoke(target, null);
                    }
                }
                else
                {
                    ToolboxEditorLog.AttributeUsageWarning(attribute,
                        attribute.MethodName + " method not found inside " + targetType + " type.");
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}