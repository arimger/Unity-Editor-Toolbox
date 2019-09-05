# Unity Editor Toolbox

## Introduction
TODO

## System Requirements
Unity 2018.x or newer

## Instalation
Copy and paste `Editor Toolbox` directory into your project (basically into `Asset` directory or somewhere deeper). 

## Table Of Contents

- [Attributes](#attributes)
- [ReorderableList](#reorderable-list)
- [Tools and Editors](#tools-and-editors)

## Attributes

### Standard Property Drawers

Drawers based on build-in classes **PropertyDrawer** and associated **PropertyAttribute**.

&nbsp;

> Editor Toolbox/Scripts/Attributes/\
> Editor Toolbox/Editor/Drawers/

&nbsp;

#### HelpAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc1.png)

#### TagSelectorAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc2.png)

#### SeparatorAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc3.png)

#### ProgressBarAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc4.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc5.png)

#### NewLabelAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc12.png)

#### MinMaxSliderAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc6.png)

#### IndentAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc7.png)

#### ConditionalFieldAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc9.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc8.png)

#### AssetPreviewAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc10.png)

#### HideLabelAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc11.png)

#### SuffixAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc13.png)

#### TypeConstraintAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc14.png)

#### ReadOnlyFieldAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc15.png)

#### BoxedHeaderAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc16.png)

#### BoxedToggleAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc17.png)

#### EnumFlagAttribute

```
[System.Flags]
public enum FlagExample
{
    Nothing = 0,
    Flag1 = 1,
    Flag2 = 2,
    Flag3 = 4,
    Everything = ~0
}

[EnumFlag]
public FlagExample enumFlag = FlagExample.Flag1 | FlagExample.Flag2;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc20.png)

#### NotNullAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc21.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc22.png)

### Ordered Property Drawers

Drawers based on classes **OrderedDrawer** and associated **OrderedAttribute**.  
For proper work they need at least one settings file located in your project.  
Predefined one - `Editor Toolbox/EditorSettings.asset`.

&nbsp;

> Editor Toolbox/Scripts/Attributes/OrderedAttributes\
> Editor Toolbox/Editor/Drawers/OrderedDrawers

&nbsp;

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc5.png)

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
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc18.png)

#### ReorderableListAttribute

```
[ReorderableList(ListStyle.Round)]
public List<string> standardStyleList;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc7.png)

#### DrawIfAttribute

Same like standard PropertyDrawer for **ConditionalFieldAttribute** but works with Enum types and arrays/lists.

#### ReadOnlyAttribute

Same like standard PropertyDrawer for **ReadOnlyFieldAttribute** but works with arrays and lists.

```
[ReadOnly]
public int[] vars1 = new [] { 1, 2, 3, 4 };
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc19.png)

## Reorderable List

Editor Toolbox/Editor/Internal/ReorderableList.cs

Custom implementation of standard ReorderableList(UnityEditorInternal). Useable as attribute in inspector fields or single object in custom editors.

```
[ReorderableList(ListStyle.Lined, "Item")]
public List<int> linedStyleList;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc6.png)
```
[ReorderableList(ListStyle.Round)]
public List<string> standardStyleList;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc7.png)
```
[ReorderableList(ListStyle.Boxed, fixedSize: true)]
public GameObject[] boxedStyleList = new GameObject[4];
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc8.png)

## Tools and Editors

### Terrain Editor
Editor Toolbox/Editor/Tools/TerrainToolEditor.cs

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc1.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc9.png)

### Hierarchy Editor
Editor Toolbox/Editor/HierarchyEditorWindow.cs

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc2.png)

### Construction Editor
TODO

---


### Field of View Generator
Editor Toolbox/Editor/Tools/AssetGenerators/ViewGenerator.cs

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc3.png)

### Grid Generator
Editor Toolbox/Editor/Tools/AssetGenerators/GridGenerator.cs

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc4.png)
