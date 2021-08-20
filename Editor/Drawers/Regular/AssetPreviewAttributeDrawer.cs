using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(AssetPreviewAttribute))]
    public class AssetPreviewAttributeDrawer : PropertyDrawerBase
    {
        /// <summary>
        /// Returns proper <see cref="Object"/> depending on the <see cref="referenceValue"/> type.
        /// </summary>
        private Object GetValidTarget(Object referenceValue)
        {
            if (referenceValue)
            {
                switch (referenceValue)
                {
                    //NOTE: if target reference is a component we want to retrieve a preview from the associated GameObject
                    case Component component:
                        return component.gameObject;
                    default:
                        return referenceValue;
                }
            }

            return null;
        }

        /// <summary>
        /// Draws asset preview using previously created <see cref="Texture2D"/> and base rect.
        /// </summary>
        private void DrawAssetPreview(Rect rect, Texture2D previewTexture)
        {
            //cache indent difference
            var indent = rect.width - EditorGUI.IndentedRect(rect).width;
            //set image base properties
            var width = Mathf.Clamp(Attribute.Width, 0, previewTexture.width);
            var height = Mathf.Clamp(Attribute.Height, 0, previewTexture.height);

            Style.textureStyle.normal.background = previewTexture;
            //set additional height as preview + 2x spacing + 2x frame offset
            rect.width = width + Style.offset + indent;
            rect.height = height + Style.offset;
            rect.y += Style.height + Style.spacing;
            //draw background frame
            EditorGUI.LabelField(rect, GUIContent.none, Style.previewStyle);
            rect.width = width + indent;
            rect.height = height;
            //adjust image to frame center
            rect.y += Style.offset / 2;
            rect.x += Style.offset / 2;
            //draw texture without label
            EditorGUI.LabelField(rect, GUIContent.none, Style.textureStyle);
        }


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            //return native height 
            if (!property.objectReferenceValue)
            {
                return base.GetPropertyHeightSafe(property, label);
            }

            //set additional height as preview + 2x spacing + 2x frame offset
            var additionalHeight = Attribute.Height + Style.offset * 2 + Style.spacing * 2;
            if (!Attribute.UseLabel)
            {
                additionalHeight -= Style.height;
            }

            return Style.height + additionalHeight;
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Attribute.UseLabel)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                //adjust OY position since we need no label
                position.y -= Style.height;
            }

            var target = GetValidTarget(property.objectReferenceValue);
            if (target)
            {
                var previewTexture = AssetPreview.GetAssetPreview(target);
                if (previewTexture == null)
                {
                    return;
                }

                //finally draw preview texture of the target
                DrawAssetPreview(position, previewTexture);
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }


        private AssetPreviewAttribute Attribute => attribute as AssetPreviewAttribute;


        private static class Style
        {
            internal static readonly float offset = 6.0f;
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = 2.0f;

            internal static readonly GUIStyle textureStyle;
            internal static readonly GUIStyle previewStyle;

            static Style()
            {
                textureStyle = new GUIStyle();
#if UNITY_2019_3_OR_NEWER
                previewStyle = new GUIStyle("helpBox");
#else
                previewStyle = new GUIStyle("box");
#endif
            }
        }
    }
}