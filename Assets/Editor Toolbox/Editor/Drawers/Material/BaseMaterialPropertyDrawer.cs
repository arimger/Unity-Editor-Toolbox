using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class BaseMaterialPropertyDrawer : MaterialPropertyDrawer
    {
        protected virtual float GetPropertyHeightSafe(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return MaterialEditor.GetDefaultPropertyHeight(prop);
        }

        protected virtual void OnGUISafe(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            editor.DefaultShaderProperty(position, prop, label);
        }

        protected virtual bool IsPropertyValid(MaterialProperty prop)
        {
            return true;
        }

        protected virtual void DrawInvalidDrawerLabel(Rect position, string label)
        {
            var content = new GUIContent(label);
#if UNITY_2019_1_OR_NEWER
            content.image = EditorGuiUtility.GetHelpIcon(MessageType.Warning);
#endif
            EditorGUI.LabelField(position, content);
        }


        public override sealed float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return IsPropertyValid(prop)
                ? GetPropertyHeightSafe(prop, label, editor)
                : base.GetPropertyHeight(prop, label, editor);
        }

        public override sealed void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (IsPropertyValid(prop))
            {
                OnGUISafe(position, prop, label, editor);
                return;
            }

            DrawInvalidDrawerLabel(position, string.Format("{0} has invalid drawer", prop.displayName));
        }
    }
}