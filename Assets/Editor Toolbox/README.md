# Unity Editor Toolbox

## Introduction
TODO

## System Requirements
Unity 2018.x or newer

## Table Of Contents

- [Attributes](#attributes)
- [ReorderableList](#reorderable-list)
- [Tools and Editors](#tools-and-editors)

## Attributes

### Standard Property Drawers

Editor Toolbox/Scripts/Attributes/\
Editor Toolbox/Editor/Drawers/

#### HelpAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc1.png)

#### TagSelectorAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc2.png)

#### SeparatorAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc3.png)

#### ProgressBarAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc4.png)
![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc5.png)

#### NewLabelAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc12.png)

#### MinMaxSliderAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc6.png)

#### IndentAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc7.png)

#### ConditionalFieldAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc9.png)
![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc8.png)

#### AssetPreviewAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc10.png)

#### HideLabelAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc11.png)

#### SuffixAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc13.png)

#### TypeConstraintAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc14.png)

#### ReadOnlyFieldAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc15.png)

#### BoxedHeaderAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc16.png)

#### BoxedToggleAttribute

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc17.png)

#### EnumFlagAttribute


### Ordered Property Drawers

Editor Toolbox/Scripts/Attributes/OrderedAttributes\
Editor Toolbox/Editor/Drawers/OrderedDrawers

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/doc5.png)

#### GroupAttribute
```
[Group("Group1")]
public int var1;
[Group("Group1")]
public int var2;
public int var3;
[Group("Group1")]
public int var4;
```
![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc18.png)

#### ReorderableListAttribute

```
[ReorderableList(ListStyle.Round)]
public List<string> standardStyleList;
```

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/doc7.png)

#### DrawIfAttribute

Same like standard PropertyDrawer for **ConditionalFieldAttribute** but works with Enum types and arrays/lists.

#### ReadOnlyAttribute

Same like standard PropertyDrawer for **ReadOnlyFieldAttribute** but works with arrays and lists.

```
[ReadOnly]
public int[] vars1 = new [] { 1, 2, 3, 4 };
```

![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/Attributes/doc19.png)

## Reorderable List

Editor Toolbox/Editor/Internal/ReorderableList.cs

Custom implementation of standard ReorderableList(UnityEditorInternal). Useable as attribute in inspector fields or single object in custom editors.

```
[ReorderableList(ListStyle.Lined, "Item")]
public List<int> linedStyleList;
```
![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/doc6.png)
```
[ReorderableList(ListStyle.Round)]
public List<string> standardStyleList;
```
![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/doc7.png)
```
[ReorderableList(ListStyle.Boxed, fixedSize: true)]
public GameObject[] boxedStyleList = new GameObject[4];
```
![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/doc8.png)

## Tools and Editors

### Terrain Editor
![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/doc1.png)
### Hierarchy Editor
![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/doc2.png)
### Construction Editor
TODO
### Field of View Generator
![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/doc3.png)
### Grid Generator
![inspector](https://github.com/arimger/HighPolis/blob/develop/Documentation/doc4.png)
