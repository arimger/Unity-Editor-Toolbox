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
        private static readonly List<EnumTogglesDrawer> toggleDrawers = new List<EnumTogglesDrawer>()
        {
            new BasicEnumTogglesDrawer(),
            new FlagsEnumTogglesDrawer()
        };


        /// <summary>
        /// Returns prepared <see cref="EnumTogglesDrawer"/> associated to the given property.
        /// </summary>
        private EnumTogglesDrawer GetDrawer(SerializedProperty property, FieldInfo fieldInfo)
        {
            var enumType = GetEnumType(property, fieldInfo);
            return GetDrawer(enumType);
        }

        private EnumTogglesDrawer GetDrawer(Type enumType)
        {
            var drawer = toggleDrawers.Find(d => d.IsValid(enumType));
            drawer.Prepare(enumType);
            return drawer;
        }

        private Type GetEnumType(SerializedProperty property, FieldInfo fieldInfo)
        {
            return property.GetProperType(fieldInfo);
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


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            var drawer = GetDrawer(property, fieldInfo);
            var input = GetInputData(Attribute);
            return drawer.GetHeight(input);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            var prefixPosition = EditorGUI.PrefixLabel(position, label);

            var drawer = GetDrawer(property, fieldInfo);
            var input = GetInputData(Attribute, position, prefixPosition);
            drawer.OnGui(input, property);
            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Enum;
        }


        public EnumTogglesAttribute Attribute => attribute as EnumTogglesAttribute;


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

        private abstract class EnumTogglesDrawer
        {
            /// <summary>
            /// Enum-related metadatas mapped to all previously supported types.
            /// </summary>
            private readonly Dictionary<Type, EnumData> cachedEnumDatas = new Dictionary<Type, EnumData>();

            protected EnumData enumData;


            protected bool IsFlagsEnum(Type type)
            {
                return System.Attribute.IsDefined(type, typeof(FlagsAttribute));
            }

            protected DraftInfo GetDraftInfo(InputData input, in EnumData enumData)
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

            protected int GetMaxTogglesCount(float stripWidth, float toggleWidth)
            {
                return Mathf.Max(1, Mathf.FloorToInt(stripWidth / toggleWidth));
            }

            protected virtual void DrawGroup(InputData input, in EnumData enumData, ref int? mask)
            {
                var draft = GetDraftInfo(input, enumData);
                var strip = new StripInfo(input.firstRowPosition, input.toggleSpacing, draft.togglesInFirstRow);
                var index = 0;

                DrawStrip(strip, enumData, ref mask, ref index);
                strip.position = input.otherRowPosition;
                strip.division = draft.togglesInOtherRow;

                for (var i = 0; i < draft.otherRowsCount; i++)
                {
                    DrawStrip(strip, enumData, ref mask, ref index);
                    strip.position.y += input.toggleHeight + input.spacing;
                }
            }

            protected virtual void DrawStrip(StripInfo strip, in EnumData enumData, ref int? mask, ref int index)
            {
                var enumLabels = enumData.labels;
                var enumValues = enumData.values;

                var division = strip.division;
                var maxRowToggles = Mathf.Min(enumLabels.Count, index + division);
                var togglesToDraw = maxRowToggles - index;
                var position = strip.position;
                position.width = (position.width - strip.spacing * (togglesToDraw - 1)) / togglesToDraw;

                for (; index < maxRowToggles; index++)
                {
                    var enumValue = enumValues[index];
                    var enumLabel = enumLabels[index];
                    mask = DrawToggle(position, mask, (int)enumValue, enumLabel);
                    position.x += position.width + strip.spacing;
                }
            }

            protected abstract EnumData GetEnumData(string[] rawLabels, Array rawValues);
            protected abstract int? DrawToggle(Rect position, int? mask, int enumValue, string label);
            protected abstract int? GetMaskValue(SerializedProperty property, in EnumData data);
            protected abstract void SetMaskValue(SerializedProperty property, in EnumData data, int? maskValue);


            public virtual bool IsValid(Type enumType)
            {
                return true;
            }

            public virtual void Prepare(Type enumType)
            {
                if (cachedEnumDatas.TryGetValue(enumType, out var data))
                {
                    enumData = data;
                    return;
                }

                var rawLabels = Enum.GetNames(enumType);
                var rawValues = Enum.GetValues(enumType);
                enumData = cachedEnumDatas[enumType] = GetEnumData(rawLabels, rawValues);
            }

            public virtual float GetHeight(InputData input)
            {
                var draft = GetDraftInfo(input, enumData);
                return ((input.toggleHeight + input.spacing) * draft.otherRowsCount) + input.toggleHeight;
            }

            public virtual void OnGui(InputData input, SerializedProperty property)
            {
                var mask = GetMaskValue(property, enumData);
                EditorGUI.BeginChangeCheck();
                DrawGroup(input, enumData, ref mask);
                if (EditorGUI.EndChangeCheck())
                {
                    SetMaskValue(property, enumData, mask);
                }
            }


            protected struct EnumData
            {
                public List<string> labels;
                public List<object> values;
                public int valuesSum;
                public bool isFlags;
                public Type enumType;
            }

            /// <summary>
            /// Concept how toggles group should look like.
            /// </summary>
            protected struct DraftInfo
            {
                public int togglesInFirstRow;
                public int togglesInOtherRow;
                public int otherRowsCount;
            }

            /// <summary>
            /// Metadata about a single strip in the group.
            /// </summary>
            protected struct StripInfo
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
        }

        private sealed class BasicEnumTogglesDrawer : EnumTogglesDrawer
        {
            protected override EnumData GetEnumData(string[] rawLabels, Array rawValues)
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

            protected override int? DrawToggle(Rect position, int? mask, int enumValue, string label)
            {
                EditorGUI.BeginChangeCheck();
                var value = mask.HasValue && mask.Value == enumValue;
                var result = GUI.Toggle(position, value, label, Style.toggleStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    if (result)
                    {
                        mask = enumValue;
                    }
                }

                return mask;
            }

            protected override int? GetMaskValue(SerializedProperty property, in EnumData data)
            {
                return property.hasMultipleDifferentValues ? (int?)null : property.intValue;
            }

            protected override void SetMaskValue(SerializedProperty property, in EnumData data, int? maskValue)
            {
                if (maskValue.HasValue)
                {
                    property.intValue = maskValue.Value;
                }
            }


            public override bool IsValid(Type enumType)
            {
                return !IsFlagsEnum(enumType);
            }
        }

        private sealed class FlagsEnumTogglesDrawer : EnumTogglesDrawer
        {
            protected override EnumData GetEnumData(string[] rawLabels, Array rawValues)
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

            protected override int? DrawToggle(Rect position, int? mask, int enumValue, string label)
            {
                EditorGUI.BeginChangeCheck();
                var value = mask.HasValue && mask.Value == (mask.Value | enumValue);
                var result = GUI.Toggle(position, value, label, Style.toggleStyle);
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

            protected override int? GetMaskValue(SerializedProperty property, in EnumData data)
            {
                //we have to handle situation when mask == ~0 == -1, what means every flag is attached
                //in this case mask value will be treated as "summary" to simplify additions/substractions

                //we use nullable values in case of multiple different values to indicate this situation
                if (property.hasMultipleDifferentValues)
                {
                    return null;
                }
                else
                {
                    return property.intValue == ~0 ? data.valuesSum : property.intValue;
                }
            }

            protected override void SetMaskValue(SerializedProperty property, in EnumData data, int? maskValue)
            {
                if (maskValue.HasValue)
                {
                    property.intValue = maskValue.Value == data.valuesSum ? ~0 : maskValue.Value;
                }
            }


            public override bool IsValid(Type enumType)
            {
                return IsFlagsEnum(enumType);
            }
        }

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