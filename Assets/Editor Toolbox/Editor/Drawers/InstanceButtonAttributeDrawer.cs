using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(InstanceButtonAttribute))]
    public class InstanceButtonAttributeDrawer : ButtonAttributeDrawer
    {
        public override float GetHeight()
        {
            return Style.height + Style.spacing;
        }

        public override void OnButtonClick()
        {
            var attribute = Attribute;

            var method = attribute.InstanceType.GetMethod(attribute.MethodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (method != null)
            {
                foreach (var target in Selection.gameObjects)
                {
                    if (target == null)
                    {
                        continue;
                    }

                    var targetComponent = target.GetComponent(attribute.InstanceType);
                    if (targetComponent == null)
                    {
                        ToolboxEditorLog.AttributeUsageWarning(attribute,
                            attribute.InstanceType + " component not found in selected GameObject(" + target.name + ").");
                        continue;
                    }

                    method.Invoke(targetComponent, null);
                }
            }
            else
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute,
                    attribute.MethodName + " method not found inside " + attribute.InstanceType + " type.");
            }
        }


        private InstanceButtonAttribute Attribute => attribute as InstanceButtonAttribute;


        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight * 1.25f;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
        }
    }
}