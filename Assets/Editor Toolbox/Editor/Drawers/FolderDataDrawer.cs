using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(FolderData))]
    public class FolderDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label) + (property.isExpanded ? Style.iconHeight : 0.0f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //draw property field in native way 
            EditorGUI.PropertyField(position, property, label, property.isExpanded);

            if (!property.isExpanded)
            {
                //adjust position for small icon and draw it in label field
                var smallIconRect = new Rect(position.xMax - Style.smallIconWidth, position.yMin - Style.spacing, Style.smallIconWidth, Style.smallIconHeight);

                GUI.DrawTexture(smallIconRect, Style.folderTexture);
                return;
            }

            var rawPropertyHeight = EditorGUI.GetPropertyHeight(property);
            //get icons properties
            var bigIconProperty = property.FindPropertyRelative("icon");
            var smallIconProperty = property.FindPropertyRelative("smallIcon");
       
            //get specific rects for folder icons
            var bigFolderIconRect = new Rect(position.x, position.yMin + rawPropertyHeight, Style.iconWidth, Style.iconHeight);
            var smallFolderIconRect = new Rect(position.x + Style.iconWidth,
                bigFolderIconRect.y + Style.smallIconHeight / 2 - Style.spacing, Style.smallIconWidth, Style.smallIconHeight);

            //draw folder icons using desired content
            GUI.DrawTexture(bigFolderIconRect, Style.folderTexture);
            GUI.DrawTexture(smallFolderIconRect, Style.folderTexture);

            //handle big icon texture preview if exist
            if (bigIconProperty.objectReferenceValue)
            {
                var previewTexture = bigIconProperty.objectReferenceValue as Texture;

                //adjust big icon rect using predefined style properties
                bigFolderIconRect.x += bigFolderIconRect.width * ToolboxEditorProject.Style.xToWidthRatio;
                bigFolderIconRect.y += bigFolderIconRect.height * ToolboxEditorProject.Style.yToHeightRatio;
                bigFolderIconRect.width = ToolboxEditorProject.Style.iconWidth;
                bigFolderIconRect.height = ToolboxEditorProject.Style.iconHeight;

                //draw big preview icon
                GUI.DrawTexture(bigFolderIconRect, previewTexture);
            }

            //handle small icon texture preview if exist
            if (smallIconProperty.objectReferenceValue)
            {
                var previewTexture = smallIconProperty.objectReferenceValue as Texture;

                //adjust small icon rect using predefined style properties
                smallFolderIconRect.x += smallFolderIconRect.width * ToolboxEditorProject.Style.xToWidthRatioSmall;
                smallFolderIconRect.y += smallFolderIconRect.height * ToolboxEditorProject.Style.yToHeightRatioSmall;
                smallFolderIconRect.width = ToolboxEditorProject.Style.iconWidthSmall;
                smallFolderIconRect.height = ToolboxEditorProject.Style.iconHeightSmall;

                //draw small preview icon
                GUI.DrawTexture(smallFolderIconRect, previewTexture);
            }
        }


        private static class Style
        {
            internal static float height = EditorGUIUtility.singleLineHeight;
            internal static float spacing = EditorGUIUtility.standardVerticalSpacing;

            internal const float iconWidth = ToolboxEditorProject.Style.folderIconWidth;
            internal const float iconHeight = ToolboxEditorProject.Style.folderIconHeight;
            internal const float smallIconWidth = ToolboxEditorProject.Style.folderIconWidthSmall;
            internal const float smallIconHeight = ToolboxEditorProject.Style.folderIconHeightSmall;

            internal static readonly Texture2D folderTexture;

            static Style()
            {
                folderTexture = EditorGUIUtility.FindTexture("Folder Icon");
            }
        }
    }
}