using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(AssetPreviewAttribute))]
    public class AssetPreviewAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            //return native height 
            if (!property.objectReferenceValue)
            {
                return base.GetPropertyHeightSafe(property, label);
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

                if (!previewTexture)
                {
                    return;
                }

                //cache indent difference
                var indent = position.width - EditorGUI.IndentedRect(position).width;
                //set image style
                var width = Mathf.Clamp(Attribute.Width, 0, previewTexture.width);
                var height = Mathf.Clamp(Attribute.Height, 0, previewTexture.height);

                Style.textureStyle.normal.background = previewTexture;
                //set additional height as preview + 2x spacing + 2x frame offset
                position.width = width + Style.frameSize + indent;
                position.height = height + Style.frameSize;
                position.y += Style.height + Style.spacing;
                //draw frame
                EditorGUI.LabelField(position, GUIContent.none, Style.previewStyle);
                position.width = width + indent;
                position.height = height;
                //adjust image to frame center
                position.y += Style.frameSize / 2;
                position.x += Style.frameSize / 2;
                //draw preview texture
                EditorGUI.LabelField(position, GUIContent.none, Style.textureStyle);
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }


        private AssetPreviewAttribute Attribute => attribute as AssetPreviewAttribute;


        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float frameSize = 6.0f;

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