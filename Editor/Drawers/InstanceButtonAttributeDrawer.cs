using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(InstanceButtonAttribute))]
    public class InstanceButtonAttributeDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return Style.height + Style.spacing;
        }

        public override void OnGUI(Rect position)
        {
            position.height = Style.height;

            var disable = false;
            switch (Attribute.Type)
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
            if (GUI.Button(position, string.IsNullOrEmpty(Attribute.Label) ? Attribute.MethodName : Attribute.Label))
            {

                var method = Attribute.InstanceType.GetMethod(Attribute.MethodName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (method != null)
                {
                    foreach (var target in Selection.gameObjects)
                    {
                        if (target == null)
                        {
                            continue;
                        }

                        var targetComponent = target.GetComponent(Attribute.InstanceType);
                        if (targetComponent == null)
                        {
                            Debug.LogWarning("ButtonAttribute - " + Attribute.InstanceType + " component not found in selected GameObject(" + target.name + ").");
                            continue;
                        }

                        method.Invoke(targetComponent, null);
                    }
                }
                else
                {
                    Debug.LogWarning("ButtonAttribute - " + Attribute.MethodName + " method not found inside " + Attribute.InstanceType + " type.");
                }
            }
            EditorGUI.EndDisabledGroup();
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

            internal static GUIStyle buttonStyle;

            static Style()
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
            }
        }
    }
}