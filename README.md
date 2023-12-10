# Unity Editor Toolbox

## Introduction

Improve usability and clarity of key features in Unity Editor for better workflow!

This Toolbox not only extends functionalities, it does so with the user in mind.
Written to be as flexible and optimized as possible. Now you and other programming professionals will be able to create a readable and useful component editor simply by using attributes. You’ll get fast and clear access to data from GameObjects placed in the Scene. Lastly, you’ll gain more control over the Project window. Go ahead, customize those folder icons.

It's worth to mention that prepared drawers are based on the custom, layout-based system. Additionally, I’m leaving you some useful scripts, classes, and functions that facilitate Editor extensions development.

Learn all the details about the main features below.

## System Requirements
Unity 2018.x or newer

## Dependencies

[EditorCoroutines](https://docs.unity3d.com/Packages/com.unity.editorcoroutines@0.0/api/Unity.EditorCoroutines.Editor.html)

## Installation

- Install Editor Toolbox package:
	- 1 way: Find Unity Package Manager (Window/Package Manager) and add package using this git URL:
	```
	https://github.com/arimger/Unity-Editor-Toolbox.git#upm
	```
	- 2 way: Copy and paste `Assets/Editor Toolbox` directory into your project (Assets/...) + add dependencies
	- 3 way: Install via [OpenUPM registry](https://openupm.com):
	```
	openupm add com.browar.editor-toolbox
	```
- Open Edit/Project Settings/Editor Toolbox window
- If settings file is not found, press the "Refresh" button or create a new one
- Manage settings in your way
	- Enable/disable Hierarchy overlay, choose allowed information
	- Enable/disable Project icons or/and assign own directories
	- Enable/disable Toolbox drawers or/and assign custom drawers
	- Enable/disable Toolbox Scene View and assign hotkeys
	
## Table Of Contents

- [Attributes & Drawers](#drawers)
	- [Regular Drawers](#regulardrawers)
	- [Toolbox Drawers](#toolboxdrawers)
		- [Toolbox Decorator Attributes](#toolboxdecorator)
		- [Toolbox Condition Attributes](#toolboxcondition)
		- [Toolbox Property (Self/List) Attributes](#toolboxproperty)
		- [Toolbox Special Attributes](#toolboxspecial)
		- [Toolbox Archetype Attributes](#toolboxarchetype)
		- [SerializeReference (ReferencePicker)](#toolboxreference)
		- [Toolbox Custom Editors](#toolboxeditors)
	- [Material Drawers](#materialdrawers)
- [Serialized Types](#serialized-types)
- [Editor Extensions](#editor-extensions)
	- [Hierarchy](#hierarchy)
	- [Project](#project)
	- [Toolbar](#toolbar)
	- [Utilities](#utilities)
	- [SceneView](#sceneview)

## Settings

The most important file, allows the user to manage all available features. Can be accessed from the Project Settings window (Edit/Project Settings.../Editor Toolbox) or directly inside the Project window. Make sure to have one valid settings file per project.

Available features are divided into four groups:
- Hierarchy
- Project
- Inspector
- SceneView

Each module is described in its respective section.

If you want to keep your custom settings between UET versions, create your own settings file:
```
Create/Editor Toolbox/Settings
```

## Attributes & Drawers <a name="drawers"></a>

### Regular Drawers <a name="regulardrawers"></a>

Drawers based on built-in classes **PropertyDrawer/DecoratorDrawer** and associated **PropertyAttribute**.
Regular drawers have priority over Toolbox drawers and they cannot be mixed.

#### TagSelectorAttribute

Supported types: **string**.

```csharp
[TagSelector]
public string var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/tagselector.png)

#### ProgressBarAttribute

Supported types: **int**, **float**, **double**.

```csharp
[ProgressBar("Name", minValue: 0.0f, maxValue: 100.0f, HexColor = "#EB7D34", IsInteractable = true)]
public float var1 = 80.0f;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/progressbar1.png)\
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/progressbar2.png)

#### MinMaxSliderAttribute

Supported types: **Vector2, Vector2Int**.

```csharp
[MinMaxSlider(0.5f, 71.7f)]
public Vector2 var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/minmaxslider.png)

#### AssetPreviewAttribute

Supported types: UnityEngine.**Object**.

```csharp
[AssetPreview]
public GameObject var1;
[AssetPreview(useLabel: false)]
public Component var2;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/assetpreview.png)

#### SuffixAttribute

Supported types: **all**.

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/suffix.png)


#### EnumTogglesAttribute

Supported types: **Enums**.

```csharp
[System.Flags]
public enum FlagExample
{
	Nothing = 0,
	Flag1 = 1,
	Flag2 = 2,
	Flag3 = 4,
	Flag4 = 8,
	Flag5 = 16,
	Flag6 = 32,
	Flag7 = 64,
	Flag8 = 128,
	Flag9 = 256,
	Everything = ~0
}

[EnumToggles]
public FlagExample enumFlag = FlagExample.Flag1 | FlagExample.Flag2 | FlagExample.Flag6;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/enumtoggles.png)

#### NotNullAttribute

Supported types: UnityEngine.**Object**.

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/notnull1.png)\
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/notnull2.png)


#### DirectoryAttribute

Supported types: **string**.

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/directory1.png)\
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/directory2.png)

#### SceneNameAttribute

Supported types: **string**.

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/scenename1.png)\
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/scenename2.png)

#### PresetAttribute

Supported types: **all**.

Remark: can be used only within classes, structs are not supported.
```csharp
private readonly int[] presetValues = new[] { 1, 2, 3, 4, 5 };

[Preset(nameof(presetValues))]
public int presetTarget;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/preset.png)

```csharp
private readonly int[] presetValues = new[] { 1, 2, 3, 4, 5 };
private readonly string[] optionLabels = new[] { "a", "b", "c", "d", "e" };

[Preset(nameof(presetValues), nameof(optionLabels))]
public int presetTarget;
```

#### SearchableEnumAttribute

Supported types: **Enums**.

```csharp
[SearchableEnum]
public KeyCode enumSearch;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/enumsearch.png)

#### ClampAttribute

Supported types: **int, float, double**.

```csharp
[Clamp(minValue = 1.5f, maxValue = 11.3f)]
public double var1;
```

#### PasswordAttribute

Supported types: **string**.

```csharp
[Password]
public string password;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/password.png)

#### ChildObjectOnlyAttribute

Supported types: **GameObject, Component**.

#### SceneObjectOnlyAttribute

Supported types: **GameObject, Component**.

#### PrefabObjectOnlyAttribute

Supported types: **GameObject, Component**.

#### LeftToggleAttribute

Supported types: **bool**.

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/lefttoggle.png)

#### FormattedNumberAttribute

Supported types: **int, float, double**.

```csharp
[FormattedNumber]
public int bigNumber;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/formattednumber.png)

#### LabelWidthAttribute

Supported types: **all**.

```csharp
[LabelWidth(220.0f)]
public int veryVeryVeryVeryVeryLongName;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/labelwidth.png)

#### LayerAttribute

Supported types: **int**.

```csharp
[Layer]
public int var1;
```

---

### Toolbox Drawers <a name="toolboxdrawers"></a>

Drawers are based on classes inherited from the **ToolboxDrawer** class and associated **ToolboxAttribute**. With this powerful custom system you are able to create really flexible drawers. You can use them without limitations (they work with sub-classes and as array children). Every ToolboxDrawer is layout-based. For proper work they need at least one settings file located in your project. You can find predefined one here - `Editor Toolbox/EditorSettings.asset`.

Examples **'How to'** create custom ToolboxDrawers you can find [HERE](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Assets/Editor%20Toolbox/HOWTO.md).

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/inspector.png)

#### Toolbox Decorator Attributes <a name="toolboxdecorator"></a>

Display/create something before and after property in the desired order (using Order property).   
In fact **ToolboxDecoratorDrawers** are like extended version of built-in **DecoratorDrawers**. 
Unfortunately, standard decorators won't always work with ToolboxDrawers so try to use this replacement instead.

Each **ToolboxDecoratorAttribute** has two basic properties **Order** (indicates the drawing order) and **ApplyCondition** (determines if decorator will be disabled/hidden along with associated property).

```csharp
[BeginGroup("Group1")]
public int var1;
public int var2;
public int var3;
[EndGroup]
public int var4;
```
```csharp
[BeginHorizontal(labelToWidthRatio: 0.1f)]
public int var1;
public int var2;
[EndHorizontal]
public int var3;

[BeginHorizontalGroup(label: "Horizontal Group")]
public GameObject gameObject;
[SpaceArea]
[EndHorizontalGroup]
[ReorderableList]
public int[] ints;
```
```csharp
[BeginIndent]
public int var1;
public int var2;
public int var3;
[EndIndent]
public int var4;

[IndentArea(4)]
public int var5;
```
```csharp
[SpaceArea(spaceBefore = 10.0f, spaceAfter = 5.0f, Order = 1)]
public int var1;
```
```csharp
[Label("My Custom Header", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
public int var1;
```
```csharp
[EditorButton(nameof(MyMethod), "<b>My</b> Custom Label", activityType: ButtonActivityType.OnPlayMode, ValidateMethodName = nameof(ValidationMethod))]
public int var1;

private void MyMethod()
{
	Debug.Log("MyMethod is invoked");
}

private bool ValidationMethod()
{
	return var1 == 0;
}
```
```csharp
[Highlight(0, 1, 0)]
public int var1;
```
```csharp
[Help("Help information", UnityMessageType.Warning, Order = -1)]
public int var1;
[DynamicHelp(nameof(Message), UnityMessageType.Error)]
public int var2;

public string Message => "Dynamic Message";
```
```csharp
[ImageArea("https://img.itch.zone/aW1nLzE5Mjc3NzUucG5n/original/Viawjm.png", 150.0f)]
public int var1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/decorators.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/horizontal.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/imagearea.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/helpbox.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/button.png)

#### Toolbox Condition Attributes <a name="toolboxcondition"></a>

Enable/disable or show/hide properties using custom conditions. You can use them together with any other type of drawer.
Every ToolboxConditionDrawer supports boolean, int, string, UnityEngine.Object and enum types and works even with array/list properties.
You are able to pass values from fields, properties, and methods.

```csharp
public string StringValue => "Sho";
[ShowIf(nameof(StringValue), "show")]
public int var1;

public GameObject objectValue;
[HideIf(nameof(objectValue), false)]
public int var2;
```

```csharp
public KeyCode enumValue = KeyCode.A;
[EnableIf(nameof(enumValue), KeyCode.A)]
public int var1;

[DisableIf(nameof(GetFloatValue), 2.0f, Comparison = UnityComparisonMethod.GreaterEqual)]
public int var2;

public float GetFloatValue()
{
	return 1.6f;
}
```

```csharp
[DisableInPlayMode]
public int var1;
```

```csharp
public int var1;

[ShowDisabledIf(nameof(var1), 3, Comparison = UnityComparisonMethod.LessEqual)]
public int var2;

[HideDisabledIf(nameof(var1), 3, Comparison = UnityComparisonMethod.GreaterEqual)]
public int var3;
```

```csharp
public bool boolValue = true;
[ShowWarningIf(nameof(boolValue), false, "Message", DisableField = true)]
public int var1;
```

```csharp
[Disable, ReorderableList]
public int[] vars1 = new [] { 1, 2, 3, 4 };
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/disabled.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/enableif1.png)\
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/enableif2.png)

#### Toolbox Property Attributes <a name="toolboxproperty"></a>

##### InLineEditorAttribute

This attribute gives a great possibility to extend all reference-related (UnityEngine.Object) fields. 
Using it you are able to 'inline' Editors for: components, ScriptableObjects, Materials, Renderers, MeshFilters, Textures, AudioClips, etc.

```csharp
[InLineEditor(DisableEditor = false)]
public Transform var1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/inlined3.png)
```csharp
[InLineEditor]
public AudioClip var1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/inlined2.png)
```csharp
[InLineEditor(true, true)]
public Material var1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/inlined1.png)

```csharp
[InLineEditor(HideScript = false)]
public MyCustomType var1;
```

##### Reorderable List

Custom implementation of standard ReorderableList (UnityEditorInternal). Usable as an attribute in serialized fields or a single object in custom Editors.

```csharp
var list = new ReorderableList(SerializedProperty property, string elementLabel, bool draggable, bool hasHeader, bool fixedSize);
```
```csharp
[ReorderableList, InLineEditor]
public Canvas[] vars1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/list4.png)
```csharp
[ReorderableList(ListStyle.Lined, "Item", Foldable = false)]
public List<int> linedStyleList;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/list1.png)
```csharp
[ReorderableList(ListStyle.Round)]
public List<string> standardStyleList;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/list2.png)
```csharp
[ReorderableList(ListStyle.Boxed, fixedSize: true)]
public GameObject[] boxedStyleList = new GameObject[4];
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/list3.png)


```csharp
[ReorderableListExposed(OverrideNewElementMethodName = nameof(GetValue))]
public int[] list;

private int GetValue()
{
	return list.Length + Random.Range(0, 4);
}
```

##### ScrollableItemsAttribute

It's a perfect solution to inspect large arrays/lists and optimize displaying them within the Inspector window.

```csharp
[ScrollableItems(defaultMinIndex: 0, defaultMaxIndex: 5)]
public GameObject[] largeArray = new GameObject[19];
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/scrollableitems.png)

##### Other ToolboxProperty attributes

```csharp
[IgnoreParent]
public Quaternion q;
```

```csharp
[DynamicMinMaxSlider(nameof(minValue), nameof(MaxValue))]
public Vector2 vec2;

public float minValue;
public float MaxValue => 15.0f;
```

#### Toolbox Special Attributes <a name="toolboxspecial"></a>

Attributes handled internally by the ToolboxEditor. You can combine them with any other attributes.

##### NewLabelAttribute

```csharp
[NewLabel("Custom Label")]
public float var1 = 25.4f;
```

##### HideLabelAttribute

```csharp
[HideLabel]
public float var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/hidelabel.png)

##### LabelByChildAttribute

```csharp
[System.Serializable]
public class SampleClass1
{
	public Material var1;
	public KeyCode var2;
	public SampleClass2 var3;
}

[System.Serializable]
public class SampleClass2
{
	public int var1;
	public string var2;
}

[LabelByChild("var2")]
public SampleClass1[] vars1;
[LabelByChild("var3.var2")]
public SampleClass1 var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/labelbychild1.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/labelbychild2.png)

#### Toolbox Archetype Attributes <a name="toolboxarchetype"></a>

Using this attribute you are able to implement custom patterns of frequently grouped **ToolboxAttributes**.


```csharp
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class TitleAttribute : ToolboxArchetypeAttribute
{
	public TitleAttribute(string label)
	{
		Label = label;
	}


	public override ToolboxAttribute[] Process()
	{
		return new ToolboxAttribute[]
		{
			new LabelAttribute(Label),
			new LineAttribute(padding: 0)
			{
				ApplyIndent = true
			}
		};
	}


	public string Label { get; private set; }
}
```

```csharp
[Title("Header")]
public int var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/title.png)

#### SerializeReference (ReferencePicker) <a name="toolboxreference"></a>

You can draw properties marked with the **[SerializeReference]** attribute with an additional type picker that allows you to manipulate what managed type will be serialized.

To prevent issues after renaming types use `UnityEngine.Scripting.APIUpdating.MovedFromAttribute`.

```csharp
[SerializeReference, ReferencePicker(TypeGrouping = TypeGrouping.ByFlatName)]
public Interface1 var1;
[SerializeReference, ReferencePicker]
public Interface1 var1;
[SerializeReference, ReferencePicker(ParentType = typeof(ClassWithInterface2)]
public ClassWithInterfaceBase var2;

public interface Interface1 { }

[Serializable]
public struct Struct : Interface1
{
	public bool var1;
	public bool var2;
}

public abstract class ClassWithInterfaceBase : Interface1 { }

[Serializable]
public class ClassWithInterface1 : ClassWithInterfaceBase
{
	public GameObject go;
}

[Serializable]
public class ClassWithInterface2 : ClassWithInterfaceBase
{
	[LeftToggle]
	public bool var1;
}

[Serializable]
public class ClassWithInterface3 : ClassWithInterfaceBase
{
	public int var1;
}
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/referencepicker.png)

#### Custom Editors <a name="toolboxeditors"></a>

If you want to create a custom **UnityEditor.Editor** for your components and still use Toolbox-related features be sure to inherit from the **Toolbox.Editor.ToolboxEditor** class.
More details (e.g. how to customize properties drawing) you can find in the [HOWTO](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Assets/Editor%20Toolbox/HOWTO.md) document.

```csharp
using UnityEditor;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif
using Toolbox.Editor;

[CustomEditor(typeof(SampleBehaviour))]
public class SampleEditor : ToolboxEditor
{
	private void OnEnable()
	{ }

	private void OnDisable()
	{ }

	public override void DrawCustomInspector()
	{
		base.DrawCustomInspector();
		
		//for custom properties:
		// - ToolboxEditorGui.DrawToolboxProperty(serializedObject.FindProperty("myProperty"));
	}
}
```

##### Custom Editor Implementation
- **Toolbox.Editor.ToolboxEditor** - default class, override it if you want to implement a custom Editor for your components and ScriptableObjects
- **Toolbox.Editor.ToolboxScriptedImporterEditor** - override it if you want to implement a custom Editor for your custom importers

### Material Drawers <a name="materialdrawers"></a>

```
[CompactTexture]
_MainTex ("Texture", 2D) = "white" {}
[Vector2]
_Vector1 ("Vector2", Vector) = (0.0, 0.0, 0.0)
[Vector3]
_Vector2 ("Vector3", Vector) = (0.0, 0.0, 0.0)
[MinMaxSlider(20.0, 165.0)]
_Vector3 ("MinMax Vector", Vector) = (50.0, 55.0, 0.0)
[Indent(3)]
_Float1 ("Float1", Float) = 0.0
[Help(Custom Help Box , 1)]
_Float2 ("Float2", Float) = 0.0
_Float3 ("Float3", Float) = 0.0
[Title(Custom Title, 4)]
_Float ("Float", Float) = 0.5
[Toggle][Space]
_ToggleProperty ("Toggle", Int) = 0
[ShowIfToggle(_ToggleProperty)]
_ShowIfExample ("Texture", 2D) = "White" {}
[HideIfToggle(_ToggleProperty)]
_HideIfExample ("Range", Range(0, 1)) = 0.75
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/customshader.png)

## Serialized Types

#### SerializedType

Allows to serialize Types and pick them through a dedicated picker.

```csharp
[TypeConstraint(typeof(Collider), AllowAbstract = false, AllowObsolete = false, TypeSettings = TypeSettings.Class, TypeGrouping = TypeGrouping.None)]
public SerializedType var1;

public void Usage()
{
	var type = var1.Type;
}
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/serializedtype.png)

#### SerializedScene

Allows to serialize SceneAssets and use them in Runtime.

```csharp
public SerializedScene scene;

public void Usage()
{
	UnityEngine.SceneManagement.SceneManager.LoadScene(scene.BuildIndex);
}
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/serializedscene.png)

```csharp
[SceneDetails]
public SerializedScene scene;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/scenedetails.png)

Keep in mind that SerializedScene stores Scene's index, name and path. These properties are updated each time scenes collection in the Build Settings is updated or any SceneAsset is created/removed/reimported. 
Unfortunately, you need to handle associated objects reserialization by yourself, otherwise e.g. updated indexes won't be saved. I prepared for you a static event `SceneSerializationUtility.OnCacheRefreshed` that can be used to validate SerializedScenes in your project. 
You can link SerializedScene in a ScriptableObject and trigger reserialization (`EditorUtility.SetDirty()`) if needed, it's really convinient approach.

#### SerializedDictionary<TK, TV>

Allows to serialize and use Dictionaries. The presented class implements the IDictionary interface, so it can be easily used like the standard version.

Requires at least Unity 2020.1.x because of generic serialization and has to be assigned in the Settings file.

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/dictionary3.png)

```csharp
#if UNITY_2020_1_OR_NEWER
public SerializedDictionary<int, GameObject> dictionary;

public void Usage()
{
	dictionary.Add(3, new GameObject("TestObject"));
	dictionary.ContainsKey(2);
	//etc. like standard System.Collections.Generic.Dictionary<>
	var nativeDictionary = dictionary.BuildNativeDictionary();
}
#endif
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/dictionary1.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/dictionary2.png)

#### SerializedDateTime

Allows to serialize DateTime.

#### SerializedDirectory

Allows to serialize folders in form of assets and retrieve direct paths in runtime.

## Editor Extensions

### Hierarchy <a name="hierarchy"></a>

Enable and customize the presented hierarchy overlay in the **ToolboxEditorSettings**. Basically it provides more data about particular GameObjects directly within the Hierarchy window. Additionally, you can create special 'Header' objects using the '#h' prefix or Create menu: **GameObject/Editor Toolbox/Hierarchy Header** (by default created object will have **EditorOnly** tag).

Each row can contain:
- Scripts information
- Layer
- Tag
- Toggle to enable/disable GameObject
- Icon

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/hierarchy.png)

### Project <a name="project"></a>

Set custom folder icons in the **ToolboxEditorSettings**.

Properties that can be edited include:
- XY position and scale of the large icon
- XY position and scale of the small icon
- Path to directory or name (depends on picked item type)
- Optional tooltip
- Large icon
- Small icon

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/project1.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/project2.png)

### Toolbar <a name="toolbar"></a>

> Editor Toolbox/Editor/ToolboxEditorToolbar.cs

Check **Examples** for more details.

> Examples/Editor/SampleToolbar.cs

```csharp
using Toolbox.Editor;

[UnityEditor.InitializeOnLoad]
public static class MyEditorUtility
{
	static MyEditorUtility()
	{
		ToolboxEditorToolbar.OnToolbarGui += OnToolbarGui;
	}
	
	private static void OnToolbarGui()
	{
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("1", Style.commandLeftStyle))
		{
			Debug.Log("1");
		}
		if (GUILayout.Button("2", Style.commandMidStyle))
		{
			Debug.Log("2");
		}
		if (GUILayout.Button("3", Style.commandMidStyle))
		{
			Debug.Log("3");
		}
		if (GUILayout.Button("4", Style.commandMidStyle))
		{
			Debug.Log("4");
		}
		if (GUILayout.Button("5", Style.commandRightStyle))
		{
			Debug.Log("5");
		}
	}
}
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/toolbar.png)

### Utilities <a name="utilities"></a>

Copy and paste all components from/to particular GameObject.

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/utils.png)

Create multiple ScriptableObjects at once.
```
Assets/Create/Editor Toolbox/ScriptableObject Creation Wizard
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/createso.png)

### SceneView <a name="sceneview"></a>

Select a specific object that is under the cursor (default key: left control).

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/sceneview.png)