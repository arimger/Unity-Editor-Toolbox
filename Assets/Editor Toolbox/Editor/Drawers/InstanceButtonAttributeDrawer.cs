using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(InstanceButtonAttribute))]
    public class InstanceButtonAttributeDrawer : ToolboxNativeDecoratorDrawer
    {
        public override float GetHeight()
        {
            return Style.height + Style.spacing;
        }

        public override void OnGUI(Rect position)
        {
            position.height = Style.height;

            var attribute = Attribute;
            var disable = false;
            switch (attribute.Type)
            {
                case ButtonActivityType.Everything:
                    break;
                case ButtonActivityType.Nothing:
                    disable = true;
                    break;
                case ButtonActivityType.OnEditMode:
                    disable = Application.isPlaying;
                    break;
                case ButtonActivityType.OnPlayMode:
                    disable = !Application.isPlaying;
                    break;
            }

            EditorGUI.BeginDisabledGroup(disable);
            if (GUI.Button(position, string.IsNullOrEmpty(attribute.Label) ? attribute.MethodName : attribute.Label))
            {
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
                            LogWarning(attribute.InstanceType + " component not found in selected GameObject(" + target.name + ").");
                            continue;
                        }

                        method.Invoke(targetComponent, null);
                    }
                }
                else
                {
                    LogWarning(attribute.MethodName + " method not found inside " + attribute.InstanceType + " type.");
                }
            }
            EditorGUI.EndDisabledGroup();
        }


        /// <summary>
        /// Logs warning message to debug console and associates it to a given attribute.
        /// </summary>
        /// <param name="message"></param>
        private void LogWarning(string message)
        {
            Debug.LogWarning(attribute.GetType().Name + ": " + message);
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="BoxedHeaderAttribute"/>.
        /// </summary>
        private InstanceButtonAttribute Attribute => attribute as InstanceButtonAttribute;


        /// <summary>
        /// Static representation of button style.
        /// </summary>
        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight * 1.25f;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;

            internal static readonly GUIStyle buttonStyle;

            static Style()
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
            }
        }
    }
}