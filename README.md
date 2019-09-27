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

### Standard Drawers

Drawers based on build-in classes **PropertyDrawer/DecoratorDrawer** and associated **PropertyAttribute**.

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

#### ConditionalHideAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc9.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc8.png)

#### ConditionalDisableAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc24.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc25.png)

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

```csharp
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

#### RandomAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc23.png)

#### DirectoryAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc26.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc27.png)

#### BroadcastButtonAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc28.png)

#### InstanceButtonAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc29.png)

#### SceneNameAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc30.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc31.png)

#### PresetAttribute

```csharp
private readonly int[] presetValues = new[] { 1, 2, 3, 4, 5 };

[Preset("presetValues")]
public int presetTarget;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc33.png)

#### SearchableEnum

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc34.png)

### Toolbox Drawers

Drawers based on classes **ToolboxDrawer** and associated **ToolboxAttribute**.  
For proper work they need at least one settings file located in your project.  
Predefined one - `Editor Toolbox/EditorSettings.asset`.

&nbsp;

> Editor Toolbox/Scripts/Attributes/ToolboxAttributes\
> Editor Toolbox/Editor/Drawers/ToolboxDrawers

&nbsp;

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc5.png)

#### AreaAttributes

Display/create something before and after property in desired order(using Order property).   
In fact **ToolboxAreaDrawers** are like extended version of **DecoratorDrawers**. 

```csharp
[BeginGroup("Group1")]
public int var1;
public int var2;
public int var3;
[EndGroup]
public int var4;
```
```csharp
[BeginIndent]
public int var1;
public int var2;
public int var3;
[EndIndent]
public int var4;
```
```csharp
[SpaceArea(spaceBefore = 10.0f, spaceAfter = 5.0f, Order = 1)]
public int var1;
```

#### ReorderableListAttribute

```csharp
[ReorderableList(ListStyle.Round)]
public List<string> standardStyleList;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc7.png)

#### InLineEditorAttribute

```csharp
[InLineEditor]
public Transform var1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc32.png)

#### HideAttribute

Hides any property.

#### HideIfAttribute

Same like standard PropertyDrawer for **ConditionalHideAttribute** but works with Enum types and arrays/lists.   
Can be used additionally to any **PropertyDrawer** or **ToolboxPropertyDrawer**.

#### DisableAttribute

Disables any property.   
Can be used additionally to any **PropertyDrawer** or **ToolboxPropertyDrawer**.  

#### DisableIfAttribute

Same like standard PropertyDrawer for **ConditionalDisableAttribute** but works with Enum types and arrays/lists.   
Can be used additionally to any **PropertyDrawer** or **ToolboxPropertyDrawer**.

```csharp
[Disable, ReorderableList]
public int[] vars1 = new [] { 1, 2, 3, 4 };
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc19.png)

## Reorderable List

> Editor Toolbox/Editor/Internal/ReorderableList.cs

Custom implementation of standard ReorderableList(UnityEditorInternal). Useable as attribute in inspector fields or single object in custom editors.

```csharp
var list = new ReorderableList(SerializedProperty property, string elementLabel, bool draggable, bool hasHeader, bool fixedSize);
```
```csharp
[ReorderableList(ListStyle.Lined, "Item")]
public List<int> linedStyleList;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc6.png)
```csharp
[ReorderableList(ListStyle.Round)]
public List<string> standardStyleList;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc7.png)
```csharp
[ReorderableList(ListStyle.Boxed, fixedSize: true)]
public GameObject[] boxedStyleList = new GameObject[4];
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc8.png)

## Tools and Editors

### Terrain Editor

> Editor Toolbox/Editor/Tools/Editors/TerrainToolEditor.cs

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc1.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc9.png)

### Hierarchy 

> Editor Toolbox/Editor/ToolboxEditorHierarchy.cs

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc2.png)

### Toolbar

> Editor Toolbox/Editor/ToolboxEditorToolbar.cs

```csharp
using Toolbox.Editor;

[UnityEditor.InitializeOnLoad]
public static class MyEditorUtility
{
    static MyEditorUtility()
    {
        ToolboxEditorToolbar.AddToolbarButton(new ToolbarButton(() => Debug.Log("1"), new GUIContent("1")));
        ToolboxEditorToolbar.AddToolbarButton(new ToolbarButton(() => Debug.Log("2"), new GUIContent("2")));
        ToolboxEditorToolbar.AddToolbarButton(new ToolbarButton(() => Debug.Log("3"), new GUIContent("3")));
        ToolboxEditorToolbar.AddToolbarButton(new ToolbarButton(() => Debug.Log("4"), new GUIContent("4")));
        ToolboxEditorToolbar.AddToolbarButton(new ToolbarButton(() => Debug.Log("5"), new GUIContent("5")));
    }
}
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc10.png)

---


### Field of View Generator

> Editor Toolbox/Editor/Tools/Wizards/ViewGeneratorWizard.cs

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc3.png)

### Grid Generator

> Editor Toolbox/Editor/Tools/Wizards/GridGeneratorWizard.cs

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc4.png)
