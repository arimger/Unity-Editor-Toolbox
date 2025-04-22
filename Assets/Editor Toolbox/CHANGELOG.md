## 0.14.1 [22.04.2025]

### Changed:
- Ability to define [EditorButton] position (below or above the target property)
- Improvements to the SerializedScene class & linked drawer
- Fix rare issue with nested layouts

## 0.14.0 [23.02.2025]

### Added:
- OnToolbarGuiRight callback (ability to draw GUI elements on the right side of the toolbar container); OnToolbarGui replaced with the OnToolbarGuiLeft callback

### Changed:
- Fix fetching private members from base classes in various cases (e.g. [EditorButton] or conditionals)
- Move FolderData to the Runtime assembly to fix issues caused by the Visual Scripting package
- Fix minor rendering issues caused by the ReoerdableList's footer position
- Fix clearing cached Editor instances in the [InLineEditor] (fix for the AudioClip still playing)
- Improve displaying [SerializeReference]-based properties in the multi-editing mode

## 0.13.2 [29.11.2024]

### Added:
- AnimationCurveSettingsAttribute

### Changed:
- Possibility to use [EditorButton] and [DynamicHelp] in nested types
- For now SerializeReference properties without children will always be folded
- Fix exception while building labels for generic types without arguments
- Fix drawing SerializedDictionary if value or key types cannot be serialized

## 0.13.1 [30.08.2024]

### Changed:
- Fix runtime compilation error caused by the SerializedDirectory

## 0.13.0 [28.08.2024]

### Added:
- Warning information if currently serialized type in TypeField-based drawers (SerializedType, [ReferencerPicker]) is not available in the filtered types collection
- Context menu operations for [SerializeReference] properties (Copy, Paste, Duplicate), all operations are based on a deep copy of the source reference
- Basic support for generic references while using [SerializeReference] & [ReferencePicker], can be utilized in Unity 2023.2.x+
- More unit tests (PropertyUtility, filtering generic types)
- Validation of assigned assets in the SerializedDirectory class

### Changed:
- Fix duplicated initialization process forced by the OnValidate call
- Better support for generic types for the SerializedType & associated drawer
- Hierarchy: For now 'Script' label displays maximum 5 scripts
- Improved types label generation for TypeField-based drawers (SerializedType, [ReferencerPicker])

## 0.12.13 [22.08.2024]

### Added:
- DisableInEditModeAttribute

### Changed:
- Hierarchy: Added Tree List renderer, which improves visual identification of parent and child gameobjects
- Hierarchy: For now 'Script' label will display all available components linked to a GameObject/Prefabs
- Hierarchy: For now 'Layer' label will display full layer name instead of the layer mask value
- Fix SceneView settings change events firing when they shouldn't
- Fix issue when trying to find private fields/properties from parent classes (e.g. while using conditional attributes)

## 0.12.12 [17.06.2024]

### Added:
- NotPrefabObjectOnlyAttribute

### Changed:
- LabelWidthAttribute is now part of the Toolbox decorator attributes (can be mixed with other attributes)
- Hierarchy: Script, Tag, and Layer columns are now excluded from the default settings
- Possibility to change style for groups ([BeginGroup] and [BeginHorizontalGroup] attributes)
- ScriptableObject Creation Wizard now accepts only ScriptableObjects marked with the [CreateInWizard] or [CreateAssetMenu] attributes

## 0.12.11 [05.04.2024]

### Changed:
- Fix handling the HasNativeTypeDrawer method in Unity 2022.3.23
- Ignore `m_SerializedDataModeController` in Toolbox wizards
- Change [Begin/EndHorizontal] behaviour (more customization options to handle available space)

## 0.12.10 [28.03.2024]

### Changed:
- Fix fetching and initializing Toolbox features when assets are updating
- Display ReorderableList element handle at the top of available space
- Display "Collection is Empty" label for empty ReorderableLists
- New visual appearance for the SerializedDictionary properties
- Fix using Toolbox drawers in the SerializedDictionary properties
- Change the default SceneView selector key to `Tab`

## 0.12.9 [27.01.2024]

### Changed:
- Fix rare invalid SerializedProperty iterator when editing child properties
- Extend objects creation behaviour while using the ReferencePickerAttribute (possibility to create uninitialized objects)
- Minor UX improvements in the ScriptableObjectCreationWizard

## 0.12.8 [29.12.2023]

### Changed:
- Fix calling the ScriptAttributeUtility.GetDrawerTypeForType method in Unity 2033.3+
- Fix getting and setting scripting defines in Unity 2023+

## 0.12.7 [10.12.2023]

### Added:
- 'Revert Prefab Name' option for prefabs in the GameObject/Prefabs context menu

### Changed:
- Possibility to interact with ProgressBarDrawer (added IsInteractable property to the ProgressBarAttribute)
- MinMaxAttribute now supports Vector2Int

## 0.12.6 [19.10.2023]

### Changed:
- Fix ToolboxEditorToolbar in Unity 2021.1+
- Minor API changes in the SearchablePopup class

## 0.12.5 [11.09.2023]

### Added:
- Add public OnCacheRefreshed event to the SceneSerializationUtility

### Changed:
- Make ToolboxEditorHandler public

## 0.12.4 [31.07.2023]

### Changed:
- Fix SceneView selection tool behaviour when selecting nested prefabs
- Fix InvalidOperationException when ExitGUIException is thrown inside PropertyScope
- Minor SerializedScene index calculation performance improvements
- Fix SearchablePopup styles initialization in Unity 2022+
- Minor visual improvements on how ReferencePicker label is rendered 

## 0.12.3 [17.06.2023]

### Added:
- SceneView extension: better way to select raycasted objects in the Scene view
- LabelWidthAttribute

### Changed:
- Fix updating SerializedScene index after deleting Scene
- Fix SerializedScene index calculation
- Fix NRE when deleted Scene was still included in Build Settings
- Fix compilation errors in Unity 2018.x

## 0.12.1 [12.04.2023]

### Changed:
- Fix LabelByChild usage within lists
- Possibility to foldout/in dictionaries

## 0.12.0 [10.12.2022]

### Changed:
- UX improvements for the ScriptableObjectCreationWizard
- New API for drawing custom Editors (check HOWTO.md for more details)
- Fix label drawing while using the [ReferencePickerAttribute]
- Fix searching for private methods in base classes while using the [EditorButtonAttribute]
- Fix minor issues while using conditional drawers during multi-selection
- Fix minor issues while using the [TitleAttribute] in groups

## 0.11.9 [22.10.2022]

### Added:
- ScriptableObjectCreationWizard (possibility to create multiple ScriptableObjects at once)

### Changed:
- Fix displaying empty fields marked with the [SerializeReference] attribute
- Fix Scene data serialization in the SerializedScene class
- Fix retrieving FieldInfo from nested [SerializeReference]-based fields

## 0.11.8 [15.09.2022]

### Added:
- Utility methods for the Rect type
- Utility methods for the EditorPrefs API

### Changed:
- Possibility to define custom validation method in the EditorButton attribute

## 0.11.7 [04.09.2022]

### Added:
- Possibility to force default (built-in) behaviour for arrays/list

### Changed:
- Improvements to EditorButton labels
- Improvements to type fields while drawing [ReferencePicker]-based properties
- Types returned by the TypeUtilities can now be sorted (additional property for the TypeConstraintContext class)

## 0.11.5 [18.08.2022]

### Added:
- Possibility to hide the m_Script property while using the InLineEditorAttribute (HideScript = true)
- Possibility to pick base type in the ReferencePickerAttribute
- Possibility to pick display options in the ReferencePickerAttribute

### Changed:
- Fix displaying custom labels in the ToolboxEditorList
- For now all methods placed in the PropertyUtility script are public

## 0.11.4 [17.07.2022]

### Added:
- ToolboxWizard, additional Toolbox-based equivalent for the ScriptableWizard class

### Changed:
- Fix caching FieldInfo for [SerializeReference] properties

## 0.11.3 [05.06.2022]

### Added:
- 'ApplyCondition' property to decorators (possibility to apply property condition state (disabled/hidden) to associated decorators
- Begin/EndVerticalAttribute
- Optional labels property ('OptionHandle') for the PresetAttribute

### Changed:
- Fix overriding labels in ReorderableLists
- Fix drawing MinMaxSlider in indentations

## 0.11.1 [08.05.2022]

### Added:
- ReferencePicker for properties with the [SerializeReference] attribute
- TypeField - dedicated and independent type picker
- SerializedDirectory
- .editorconfig for the sample project

### Changed:
- New sample classes & general improvements to the SampleScene
- Renamed 'Scripts' directory to 'Runtime'
- Path serialization in the SerializedScene class
- NewLabel & HideLabel attributes now can be combined with any other attributes
- Performance improvements related to types caching
- Fix drawing FormattedNumber within vertical groups
- Fix drawing tooltips [TooltipAttribute]
- Fix drawing AssetPreview

## 0.10.9 [02.03.2022]

### Added:
- SerializedDirectory

### Changed:
- Fix usage of Editor-only API in runtime scripts (TypeConstraintAttribute)

## 0.10.8 [26.02.2022]

### Added:
- [Conditional("UNITY_EDITOR")] attribute to all drawer attributes
- Possibility to ignore certain properties directly in the ToolboxEditor OnGUI callback

### Changed:
- Resizing "Size" field in the ReorderableLists depending on the content
- Serializing Scene name in the SerializedScene class
- Optimization of types caching (mostly related to the SerializedType class)

## 0.10.6 [30.11.2021]

### Added:
- DynamicRangeSliderAttribute
- SerializedDateTime

### Changed:
- Fix disposing inlined Editors in ScriptableObjects-based Inspectors
- Fix disabling "Edit" button when inlined Editor is disabled
- Fix relative path when ToolboxEditorSettings is created manually

## 0.10.4 [27.10.2021]

### Added:
- DynamicMinMaxSliderAttribute

### Changed:
- Fix dragging and dropping elements into arrays/list
- Fix behaviour of EditorButtons in nested Editors

## 0.10.2 [20.08.2021]

### Changed:
- Fix marking asset objects as dirty
- Fix built-in array drawers

## 0.10.1 [17.08.2021]

### Added:
- Reflection-based drawers: possibility to pass data from non-serialized fields, properties, and methods
- DynamicHelpAttribute
- Unit tests
- Possibility to disable content in inlined Editors

### Changed:
- Fix parent extraction from the SerializedProperty
- Fix popup indentations

## 0.9.4 [29.07.2021]

### Added:
- Pack of custom material drawers

## 0.9.2 [29.07.2021]

### Changed:
- Fix layout groups within nested Editors

## 0.9.1 [27.07.2021]

### Added:
- EnumTogglesAttribute
- SceneDetailsAttribute

### Changed:
- Deprecate EnumFlagAttribute

## 0.9.0 [19.07.2021]

### Added:
- FormattedNumberAttribute
- SceneAsset picker for the SceneNameAttribute
- Optional foldout for the ReorderableList and related attributes
- GuiColorAttribute

### Changed:
- Remove obsolete attributes
- Rename ToolboxCompositionAttribute class to ToolboxArchetypeAttribute

## 0.8.13 [04.07.2021]

### Added:
- Begin/EndHorizontalGroupAttribute

### Changed:
- Fix overall layouting issues
- Fix IgnoreParentAttribute issues

## 0.8.11 [02.07.2021]

### Added:
- IgnoreParentAttribute
- ShowDisabledIfAttribute
- HideDisabledIfAttribute
- Default constraint to the SerializedType drawer

### Changed:
- Fix assigning and clearing multiple drawers at once
- Fix drawing default numeric types (Vector2, Vector3, etc.)
- Fix drawing ReorderableList within horizontal groups

## 0.8.6 [13.06.2021]

### Added:
- Additional search field for the SerializedType drawer

## 0.8.5 [13.05.2021]

### Added:
- Composition attributes
- ReorderableListExposedAttribute
- OnValueChangedAttribute
- ShowIfWarningAttribute
- Equation test methods for comparison attributes

### Changed:
- Fix finding indexes of the serialized elements within reflection-based drawers
- Fix the 'Value' label appearance in the SerializedDictionary drawer when the Value property is an array

## 0.8.2 [03.04.2021]

### Added:
- Additional comparison methods for the built-in conditional attributes

## 0.8.1 [03.04.2021]

### Added:
- SerializedDictionary + associated drawer
- Layout-based ReorderableList implementation
- Support for ToolboxDrawers in the ReorderableList
- Possibility to hide labels in the ReorderableList

### Changed:
- Fix items focusing on nested lists
- Fix empty list drawing
- Fix scrolling behaviour outside SearchablePopup
- Unification of available attributes

## 0.7.5 [06.01.2021]

## 0.7.3 [28.12.2020]

## 0.7.1 [20.10.2020]

## 0.7.0 [29.09.2020]

## 0.6.5 [08.09.2020]

## 0.6.4 [05.09.2020]

## 0.6.3 [31.08.2020]

## 0.6.2 [28.08.2020]

## 0.6.1 [25.08.2020]

## 0.6.0 [21.08.2020]

## 0.5.9 [16.08.2020]

## 0.5.7 [05.08.2020]

### Added:
- Possibility to customize displayed data inside the Hierarchy window (check the ToolboxEditorHierarchy.cs)

## 0.5.6 [16.01.2020]

### Added:
- Possibility to customize folder icons positions (check the ToolboxEditorProject.cs)

### Changed:
- Fix footer position of the ReorderableList in Unity 2019.3+
- Fix handle position of the ReorderableList in Unity 2019.3+
- Fix custom folders scalling 
- Fix object assignment in the InLineEditorAttributeDrawer
- Fix settings reload in the Editor Settings window

## 0.5.5 [09.01.2020]

### Added:
- Tooltip for custom folders
- Possibility to add drawers to hierarchy overlay from external classes (check ToolboxEditorHierarchy)