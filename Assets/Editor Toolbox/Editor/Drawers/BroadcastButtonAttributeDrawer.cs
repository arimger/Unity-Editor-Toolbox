using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(BroadcastButtonAttribute))]
    public class BroadcastButtonAttributeDrawer : ButtonAttributeDrawer
    {
        public override float GetHeight()
        {
            return Style.height + Style.spacing;
        }

        public override void OnButtonClick()
        {
            foreach (var target in Selection.gameObjects)
            {
                if (target == null)
                {
                    return;
                }

                target.SendMessage(Attribute.MethodName, SendMessageOptions.RequireReceiver);
            }
        }


        private BroadcastButtonAttribute Attribute => attribute as BroadcastButtonAttribute;


        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight * 1.25f;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
        }
    }
}