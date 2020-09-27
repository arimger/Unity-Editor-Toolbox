using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public abstract class ButtonAttributeDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            var isValid = false;
            var attribute = this.attribute as ButtonAttribute;

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

            position.yMin += EditorGUIUtility.standardVerticalSpacing / 2;
            position.yMax -= EditorGUIUtility.standardVerticalSpacing / 2;

            EditorGUI.BeginDisabledGroup(!isValid);
            if (GUI.Button(position, GetLabel()))
            {
                OnButtonClick();
            }
            EditorGUI.EndDisabledGroup();
        }

        public virtual string GetLabel()
        {
            var attribute = this.attribute as ButtonAttribute;
            return string.IsNullOrEmpty(attribute.Label) ? attribute.MethodName : attribute.Label;
        }

        public abstract void OnButtonClick();
    }
}
