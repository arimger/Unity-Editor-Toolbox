using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(AssetPreviewAttribute))]
    public class AssetPreviewAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Needed height for asset preview texture.
        /// </summary>
        private float additionalHeight;


        /// <summary>
        /// Overall property height, depending on label visibility + preview height.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Style.height + additionalHeight;
        }

        /// <summary>
        /// Creates optional property label and asset preview if it is possible.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on reference value properties.");
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            additionalHeight = 0;

            if (AssetPreviewAttribute.UseLabel)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                //setting additional height as opposite label
                additionalHeight -= Style.height;
                //adjusting OY position since we need no label
                position.y -= Style.height;
            }

            if (property.objectReferenceValue)
            {
                var previewTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);

                if (!previewTexture) return;

                //caching indent difference
                var indent = position.width - EditorGUI.IndentedRect(position).width;
                //setting image style
                var width = Mathf.Clamp(AssetPreviewAttribute.Width, 0, previewTexture.width);
                var height = Mathf.Clamp(AssetPreviewAttribute.Height, 0, previewTexture.height);

                Style.textureStyle.normal.background = previewTexture;
                //setting additional height as preview + 2x spacing + 2x frame offset
                additionalHeight += height + Style.frameSize * 2 + Style.spacing * 2;
                position.height = height + Style.frameSize;
                position.width = width + Style.frameSize + indent;
                position.y += position.height / 2 - Style.height;
                //drawing frame
                EditorGUI.LabelField(position, GUIContent.none, Style.backgroundStyle);
                position.height = height;
                position.width = width + indent;
                //adjusting image to frame center
                position.y += Style.frameSize / 2;
                position.x += Style.frameSize / 2;
                //drawing preview texture
                EditorGUI.LabelField(position, GUIContent.none, Style.textureStyle);
            }
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="global::AssetPreviewAttribute"/>.
        /// </summary>
        private AssetPreviewAttribute AssetPreviewAttribute => attribute as AssetPreviewAttribute;


        /// <summary>
        /// Static representation of asset preview style.
        /// </summary>
        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float frameSize = 6.0f;

            internal static GUIStyle textureStyle;
            internal static GUIStyle backgroundStyle;

            static Style()
            {
                textureStyle = new GUIStyle();
                backgroundStyle = new GUIStyle(GUI.skin.box);
            }
        }
    }
}