using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(EnumTogglesAttribute))]
    public class EnumTogglesAttributeDrawer : PropertyDrawerBase
    {
        private static List<EnumTogglesDrawer> toggleDrawers = new List<EnumTogglesDrawer>()
        {
            new BasicEnumTogglesDrawer(),
            new FlagsEnumTogglesDrawer()
        };

        /// <summary>
        /// Enum-related metadatas mapped to all previously supported types.
        /// </summary>
        private readonly Dictionary<Type, EnumData> cachedEnumDatas = new Dictionary<Type, EnumData>();


        private Type GetEnumType(SerializedProperty property, FieldInfo fieldInfo)
        {
            var instance = property.GetDeclaringObject();
            var enumType = property.GetProperType(fieldInfo, instance);
            return enumType;
        }

        private bool IsFlagsEnum(Type type)
        {
            return System.Attribute.IsDefined(type, typeof(FlagsAttribute));
        }

        private EnumData GetEnumData(SerializedProperty property, FieldInfo fieldInfo)
        {
            var type = GetEnumType(property, fieldInfo);
            if (cachedEnumDatas.TryGetValue(type, out var data))
            {
                return data;
            }

            var rawLabels = Enum.GetNames(type);
            var rawValues = Enum.GetValues(type);
            data = cachedEnumDatas[type] = IsFlagsEnum(type)
                ? GetFlagsEnumData(rawLabels, rawValues)
                : GetBasicEnumData(rawLabels, rawValues);
            data.enumType = type;
            return data;
        }

        private EnumData GetBasicEnumData(string[] rawLabels, Array rawValues)
        {
            var labels = new List<string>();
            var values = new List<object>();
            var enumData = new EnumData
            {
                labels = labels,
                values = values,
                isFlags = false
            };
            labels.AddRange(rawLabels);
            for (var i = 0; i < rawValues.Length; i++)
            {
                values.Add(rawValues.GetValue(i));
            }

            return enumData;
        }

        private EnumData GetFlagsEnumData(string[] rawLabels, Array rawValues)
        {
            var labels = new List<string>();
            var values = new List<object>();
            var enumData = new EnumData
            {
                isFlags = true,
                labels = labels,
                values = values
            };

            labels.Add("Everything");
            for (var i = 0; i < rawValues.Length; i++)
            {
                var value = (int)rawValues.GetValue(i);
                if (value == 0)
                {
                    continue;
                }

                if (value == ~0)
                {
                    labels[0] = rawLabels[i];
                    continue;
                }

                labels.Add(rawLabels[i]);
                values.Add(value);
                enumData.valuesSum += value;
            }

            values.Insert(0, enumData.valuesSum);
            return enumData;
        }

        /// <summary>
        /// Creates potential <see cref="InputData"/> based only on static Window properties and given attribute.
        /// </summary>
        private InputData GetInputData(EnumTogglesAttribute attribute)
        {
            var inspectorStyle = EditorStyles.inspectorDefaultMargins;
            var leftPadding = inspectorStyle.padding.left;
            var otherRowWidth = EditorGUIUtility.currentViewWidth - 2 * leftPadding;
            var firstRowWidth = EditorGUIUtility.currentViewWidth - 2 * leftPadding - EditorGUIUtility.labelWidth;

            return new InputData()
            {
                toggleWidth = attribute.ToggleWidth,
                toggleHeight = attribute.ToggleHeight,
                toggleSpacing = attribute.ToggleSpacing,
                spacing = EditorGUIUtility.standardVerticalSpacing,
                firstRowPosition = new Rect(0, 0, firstRowWidth, 0),
                otherRowPosition = new Rect(0, 0, otherRowWidth, 0)
            };
        }

        private InputData GetInputData(EnumTogglesAttribute attribute, Rect position, Rect prefixPosition)
        {
            var toggleWidth = attribute.ToggleWidth;
            var toggleHeight = attribute.ToggleHeight;
            var toggleSpacing = attribute.ToggleSpacing;
            var spacing = EditorGUIUtility.standardVerticalSpacing;

            var firstRowPosition = new Rect(prefixPosition);
            firstRowPosition.height = toggleHeight;
            var otherRowPosition = new Rect(position);
            otherRowPosition.yMin = firstRowPosition.yMax + spacing;
            otherRowPosition.yMax = otherRowPosition.yMin + toggleHeight;

            return new InputData()
            {
                toggleWidth = toggleWidth,
                toggleHeight = toggleHeight,
                toggleSpacing = toggleSpacing,
                spacing = spacing,
                firstRowPosition = firstRowPosition,
                otherRowPosition = otherRowPosition
            };
        }

        private DraftInfo GetDraftInfo(InputData input, EnumData enumData)
        {
            var draft = new DraftInfo
            {
                togglesInFirstRow = GetMaxTogglesCount(input.firstRowPosition.width, input.toggleWidth)
            };
            var togglesInRows = enumData.values.Count - draft.togglesInFirstRow;
            if (togglesInRows > 0)
            {
                draft.togglesInOtherRow = GetMaxTogglesCount(input.otherRowPosition.width, input.toggleWidth);
                draft.otherRowsCount = Mathf.CeilToInt(togglesInRows / (float)draft.togglesInOtherRow);
            }

            return draft;
        }

        private int GetMaxTogglesCount(float stripWidth, float toggleWidth)
        {
            return Mathf.Max(1, Mathf.FloorToInt(stripWidth / toggleWidth));
        }

        private void HandleBasicEnum(InputData input, EnumData enumData, SerializedProperty property)
        {
            var maskValue = property.hasMultipleDifferentValues ? (int?)null : property.intValue;
            EditorGUI.BeginChangeCheck();
            DrawTogglesGroup(input, enumData, ref maskValue, HandleBasicToggle);
            if (EditorGUI.EndChangeCheck())
            {
                if (maskValue.HasValue)
                {
                    property.intValue = maskValue.Value;
                }
            }
        }

        private void HandleFlagsEnum(InputData input, EnumData enumData, SerializedProperty property)
        {
            //we have to handle situation when mask == ~0 == -1, what means every flag is attached
            //in this case mask value will be treated as "summary" to simplify additions/substractions

            //we use nullable values in case of multiple different values to indicate this situation
            int? maskValue;
            if (property.hasMultipleDifferentValues)
            {
                maskValue = null;
            }
            else
            {
                maskValue = property.intValue == ~0 ? enumData.valuesSum : property.intValue;
            }

            EditorGUI.BeginChangeCheck();
            DrawTogglesGroup(input, enumData, ref maskValue, HandleFlagsToggle);
            if (EditorGUI.EndChangeCheck())
            {
                if (maskValue.HasValue)
                {
                    property.intValue = maskValue.Value == enumData.valuesSum ? ~0 : maskValue.Value;
                }
            }
        }

        private void DrawTogglesGroup(InputData input, EnumData enumData, ref int? mask, Func<Rect, int?, int, string, int?> toggleDrawer)
        {
            var draft = GetDraftInfo(input, enumData);
            var strip = new StripInfo(input.firstRowPosition, input.toggleSpacing, draft.togglesInFirstRow);
            var index = 0;

            //draw first strip in the "prefix" row
            DrawTogglesStrip(strip, enumData, ref mask, toggleDrawer, ref index);
            strip.position = input.otherRowPosition;
            strip.division = draft.togglesInOtherRow;

            for (var i = 0; i < draft.otherRowsCount; i++)
            {
                //draw each next strip based on the full width
                DrawTogglesStrip(strip, enumData, ref mask, toggleDrawer, ref index);
                strip.position.y += input.toggleHeight + input.spacing;
            }
        }

        private void DrawTogglesStrip(StripInfo strip, EnumData enumData, ref int? mask, Func<Rect, int?, int, string, int?> toggleDrawer, ref int index)
        {
            var enumLabels = enumData.labels;
            var enumValues = enumData.values;

            //setup drawing fields for the whole strip
            var division = strip.division;
            var maxRowToggles = Mathf.Min(enumLabels.Count, index + division);
            var togglesToDraw = maxRowToggles - index;
            var position = strip.position;
            position.width = (position.width - strip.spacing * (togglesToDraw - 1)) / togglesToDraw;

            for (; index < maxRowToggles; index++)
            {
                //NOTE: dedicated convert method from Enum value to int would be really helpful
                var enumValue = enumValues[index];
                var enumLabel = enumLabels[index];
                mask = toggleDrawer(position, mask, (int)enumValue, enumLabel);
                position.x += position.width + strip.spacing;
            }
        }

        private int? HandleBasicToggle(Rect position, int? mask, int enumValue, string enumLabel)
        {
            EditorGUI.BeginChangeCheck();
            var value = mask.HasValue && mask.Value == enumValue;
            var result = GUI.Toggle(position, value, enumLabel, Style.toggleStyle);
            if (EditorGUI.EndChangeCheck())
            {
                if (result)
                {
                    mask = enumValue;
                }
            }

            return mask;
        }

        private int? HandleFlagsToggle(Rect position, int? mask, int enumValue, string enumLabel)
        {
            EditorGUI.BeginChangeCheck();
            var value = mask.HasValue && mask.Value == (mask.Value | enumValue);
            var result = GUI.Toggle(position, value, enumLabel, Style.toggleStyle);
            if (EditorGUI.EndChangeCheck())
            {
                if (!mask.HasValue)
                {
                    mask = 0;
                }

                mask = result ? mask | enumValue : mask & ~enumValue;
            }

            return mask;
        }


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            var input = GetInputData(Attribute);
            var cache = GetEnumData(property, fieldInfo);
            var draft = GetDraftInfo(input, cache);
            return ((input.toggleHeight + input.spacing) * draft.otherRowsCount) + input.toggleHeight;
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            var prefixPosition = EditorGUI.PrefixLabel(position, label);

            //try to retrieve all drawing data and enum cache needed by the drawer
            var input = GetInputData(Attribute, position, prefixPosition);
            var cache = GetEnumData(property, fieldInfo);
            if (cache.isFlags)
            {
                HandleFlagsEnum(input, cache, property);
            }
            else
            {
                HandleBasicEnum(input, cache, property);
            }

            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Enum;
        }


        public EnumTogglesAttribute Attribute => attribute as EnumTogglesAttribute;


        private struct EnumData
        {
            public List<string> labels;
            public List<object> values;
            public int valuesSum;
            public bool isFlags;
            public Type enumType;
        }

        /// <summary>
        /// General drawing data needed by the drawer.
        /// </summary>
        private struct InputData
        {
            public Rect firstRowPosition;
            public Rect otherRowPosition;
            public float toggleWidth;
            public float toggleHeight;
            public float toggleSpacing;
            public float spacing;
        }

        /// <summary>
        /// Concept how toggles group should look like.
        /// </summary>
        private struct DraftInfo
        {
            public int togglesInFirstRow;
            public int togglesInOtherRow;
            public int otherRowsCount;
        }

        /// <summary>
        /// Metadata about a single strip in the group.
        /// </summary>
        private struct StripInfo
        {
            public Rect position;
            public int division;
            public float spacing;

            public StripInfo(Rect position, float spacing, int division)
            {
                this.position = position;
                this.spacing = spacing;
                this.division = division;
            }
        }

        //TODO:
        private abstract class EnumTogglesDrawer
        { }

        private sealed class BasicEnumTogglesDrawer : EnumTogglesDrawer
        { }

        private sealed class FlagsEnumTogglesDrawer : EnumTogglesDrawer
        { }

        private static class Style
        {
            internal static readonly GUIStyle toggleStyle;

            static Style()
            {
                toggleStyle = new GUIStyle(GUI.skin.button)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 10
#else
                    fontSize = 9
#endif
                };
            }
        }
    }
}