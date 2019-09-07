using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(BrushPrefab))]
    public class BrushPrefabDrawer : PropertyDrawer
    {
        private float additionalHeight;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, property.isExpanded) + additionalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            additionalHeight = 0;

            var basePosition = position;

            var targetProperty = property.FindPropertyRelative("target");
            var useRandomRotationProperty = property.FindPropertyRelative("useRandomRotation");
            var minRandomRotation = property.FindPropertyRelative("minRotation");
            var maxRandomRotation = property.FindPropertyRelative("maxRotation");
            var useRandomScaleProperty = property.FindPropertyRelative("useRandomScale");
            var minRandomScale = property.FindPropertyRelative("minScale");
            var maxRandomScale = property.FindPropertyRelative("maxScale");

            EditorGUI.BeginProperty(position, label, property);
            position.height = Style.height;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            if (property.isExpanded)
            {
                //draw vertical line
                EditorGUI.DrawRect(new Rect(position.x, position.y + position.height, Style.lineWidth, basePosition.height - position.height), Style.lineColor);
                EditorGUI.indentLevel++;
                position.y += position.height;
                position.height = EditorGUI.GetPropertyHeight(targetProperty, true);
                EditorGUI.PropertyField(position, targetProperty);
                //add additional spacing for horizontal line
                additionalHeight += Style.spacing + Style.spacing;
                position.y += position.height + Style.spacing + Style.spacing;
                position.height = EditorGUI.GetPropertyHeight(useRandomRotationProperty);
                //draw horizontal line
                EditorGUI.DrawRect(new Rect(position.x, position.y - Style.spacing / 2, position.width, Style.lineWidth), Style.lineColor);
                EditorGUI.PropertyField(position, useRandomRotationProperty);
                EditorGUI.BeginDisabledGroup(!useRandomRotationProperty.boolValue);
                //if (useRandomRotationProperty.boolValue)
                {
                    EditorGUI.indentLevel++;
                    position.y += position.height;
                    position.height = EditorGUI.GetPropertyHeight(minRandomRotation);
                    EditorGUI.PropertyField(position, minRandomRotation);
                    position.y += position.height;
                    position.height = EditorGUI.GetPropertyHeight(maxRandomRotation);
                    EditorGUI.PropertyField(position, maxRandomRotation);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.EndDisabledGroup();
                //add additional spacing for horizontal line
                additionalHeight += Style.spacing + Style.spacing;
                position.y += position.height + Style.spacing + Style.spacing;
                position.height = EditorGUI.GetPropertyHeight(useRandomScaleProperty);
                //draw horizontal line
                EditorGUI.DrawRect(new Rect(position.x, position.y - Style.spacing / 2, position.width, Style.lineWidth), Style.lineColor);
                EditorGUI.PropertyField(position, useRandomScaleProperty);
                EditorGUI.BeginDisabledGroup(!useRandomScaleProperty.boolValue);
                //if (useRandomRotationProperty.boolValue)
                {
                    EditorGUI.indentLevel++;
                    position.y += position.height;
                    position.height = EditorGUI.GetPropertyHeight(minRandomScale);
                    EditorGUI.PropertyField(position, minRandomScale);
                    position.y += position.height;
                    position.height = EditorGUI.GetPropertyHeight(maxRandomScale);
                    EditorGUI.PropertyField(position, maxRandomScale);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
        }


        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float lineWidth = 1.0f;

            internal static readonly Color lineColor = new Color(0.3f, 0.3f, 0.3f);
        }
    }
}