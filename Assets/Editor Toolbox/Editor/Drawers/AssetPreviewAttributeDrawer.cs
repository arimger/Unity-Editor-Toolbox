using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(AssetPreviewAttribute))]
    public class AssetPreviewAttributeDrawer : ToolboxNativePropertyDrawer
    {
        /// <summary>
        /// Creates optional property label and asset preview if it is possible.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
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

            if (property.objectReferenceValue)
            {
                var previewTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);

                if (!previewTexture) return;

                //cache indent difference
                var indent = position.width - EditorGUI.IndentedRect(position).width;
                //set image style
                var width = Mathf.Clamp(Attribute.Width, 0, previewTexture.width);
                var height = Mathf.Clamp(Attribute.Height, 0, previewTexture.height);

                Style.textureStyle.normal.background = previewTexture;
                //set additional height as preview + 2x spacing + 2x frame offset
                position.height = height + Style.frameSize;
                position.width = width + Style.frameSize + indent;
                position.y += Style.height + Style.spacing;
                //draw frame
                EditorGUI.LabelField(position, GUIContent.none, Style.backgroundStyle);
                position.height = height;
                position.width = width + indent;
                //adjust image to frame center
                position.y += Style.frameSize / 2;
                position.x += Style.frameSize / 2;
                //draw preview texture
                EditorGUI.LabelField(position, GUIContent.none, Style.textureStyle);
            }
        }


        /// <summary>
        /// Checks if provided property is valid.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }

        /// <summary>
        /// Overall property height, depending on label visibility + preview height.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //return native height 
            if (!IsPropertyValid(property) || property.objectReferenceValue == null)
            {
                return Style.height;
            }

            //set additional height as preview + 2x spacing + 2x frame offset
            var additionalHeight = Attribute.Height + Style.frameSize * 2 + Style.spacing * 2;
            if (!Attribute.UseLabel)
            {
                //adjust height to old label position
                additionalHeight -= Style.height;
            }

            return Style.height + additionalHeight;
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="AssetPreviewAttribute"/>.
        /// </summary>
        private AssetPreviewAttribute Attribute => attribute as AssetPreviewAttribute;


        /// <summary>
        /// Static representation of asset preview style.
        /// </summary>
        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float frameSize = 6.0f;

            internal static readonly GUIStyle textureStyle;
            internal static readonly GUIStyle backgroundStyle;

            static Style()
            {
                textureStyle = new GUIStyle();
                backgroundStyle = new GUIStyle(GUI.skin.box);
            }
        }
    }
}