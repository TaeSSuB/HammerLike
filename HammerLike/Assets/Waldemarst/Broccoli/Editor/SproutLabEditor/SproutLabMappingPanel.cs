using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

using Broccoli.Pipe;
using Broccoli.Base;
using Broccoli.Utils;

namespace Broccoli.BroccoEditor
{
    public class SproutLabMappingPanel {
        #region Vars
        bool isInit = false;
        static string containerName = "mappingPanel";
		SproutLabEditor sproutLabEditor = null;
		public bool requiresRepaint = false;
		bool listenGUIEvents = true;
        #endregion

        #region GUI Vars
        /// <summary>
        /// Container for the UI.
        /// </summary>
        VisualElement container;
        /// <summary>
        /// Rect used to draw the components in this panel.
        /// </summary>
        Rect rect;
		/// <summary>
		/// Side panel xml.
		/// </summary>
		private VisualTreeAsset panelXml;
		/// <summary>
		/// Side panel style.
		/// </summary>
		private StyleSheet panelStyle;
		/// <summary>
		/// Path to the side panel xml.
		/// </summary>
		private string panelXmlPath {
			get { return ExtensionManager.extensionPath + "Editor/Resources/GUI/SproutLabMappingPanelView.uxml"; }
		}
		/// <summary>
		/// Path to the side style.
		/// </summary>
		private string panelStylePath {
			get { return ExtensionManager.extensionPath + "Editor/Resources/GUI/SproutLabPanelStyle.uss"; }
		}
		private static List<string> optionItems = new List<string> {"Stem", "Sprout A", "Sprout B"};
		private static List<string> optionCrownItems = new List<string> {"Stem", "Sprout A", "Sprout B", "Crown"};
		private Action<VisualElement, int> bindOptionItem = (e, i) => (e as Label).text = optionItems[i];
		private Action<VisualElement, int> bindOptionCrownItem = (e, i) => (e as Label).text = optionCrownItems[i];
		private static string optionsListName = "options-list";
		private static string stemContainerName = "container-stem";
		private static string sproutAContainerName = "container-sprout-a";
		private static string sproutBContainerName = "container-sprout-b";
		private static string crownContainerName = "container-sprout-crown";
		private static string dissolveContainerName = "sprout-dissolve-container";
		private static string iconSaturationName = "icon-saturation";
		private static string iconShadeName = "icon-shade";
		private static string iconTintName = "icon-tint";
		private static string iconSurfaceName = "icon-surface";
		private static string iconDissolveName = "icon-dissolve";

		private static string sproutASaturationResetName = "sprout-a-saturation-reset";
		private static string sproutAShadeResetName = "sprout-a-shade-reset";
		private static string sproutATintResetName = "sprout-a-tint-reset";
		private static string sproutADissolveResetName = "sprout-a-dissolve-reset";
		private static string sproutATintColorName = "sprout-a-tint-color";
		private static string sproutATintName = "sprout-a-tint";
		private static string sproutATintModeName = "sprout-a-tint-mode";
		private static string sproutATintModeInvertName = "sprout-a-tint-mode-invert";
		private static string sproutATintVarianceName = "sprout-a-tint-variance";
		private static string sproutAShadeName = "sprout-a-shade";
		private static string sproutAShadeModeName = "sprout-a-shade-mode";
		private static string sproutAShadeModeInvertName = "sprout-a-shade-mode-invert";
		private static string sproutAShadeVarianceName = "sprout-a-shade-variance";
		private static string sproutADissolveName = "sprout-a-dissolve";
		private static string sproutADissolveModeName = "sprout-a-dissolve-mode";
		private static string sproutADissolveModeInvertName = "sprout-a-dissolve-mode-invert";
		private static string sproutADissolveVarianceName = "sprout-a-dissolve-variance";
		private static string sproutASaturationName = "sprout-a-saturation";
		private static string sproutASaturationModeName = "sprout-a-saturation-mode";
		private static string sproutASaturationModeInvertName = "sprout-a-saturation-mode-invert";
		private static string sproutASaturationVarianceName = "sprout-a-saturation-variance";
		private static string sproutAMetallicName = "sprout-a-metallic";
		private static string sproutAGlossinessName = "sprout-a-glossiness";
		private static string sproutASubsurfaceName = "sprout-a-subsurface";

		private static string sproutBSaturationResetName = "sprout-b-saturation-reset";
		private static string sproutBShadeResetName = "sprout-b-shade-reset";
		private static string sproutBTintResetName = "sprout-b-tint-reset";
		private static string sproutBDissolveResetName = "sprout-b-dissolve-reset";
		private static string sproutBTintColorName = "sprout-b-tint-color";
		private static string sproutBTintName = "sprout-b-tint";
		private static string sproutBTintModeName = "sprout-b-tint-mode";
		private static string sproutBTintModeInvertName = "sprout-b-tint-mode-invert";
		private static string sproutBTintVarianceName = "sprout-b-tint-variance";
		private static string sproutBShadeName = "sprout-b-shade";
		private static string sproutBShadeModeName = "sprout-b-shade-mode";
		private static string sproutBShadeModeInvertName = "sprout-b-shade-mode-invert";
		private static string sproutBShadeVarianceName = "sprout-b-shade-variance";
		private static string sproutBDissolveName = "sprout-b-dissolve";
		private static string sproutBDissolveModeName = "sprout-b-dissolve-mode";
		private static string sproutBDissolveModeInvertName = "sprout-b-dissolve-mode-invert";
		private static string sproutBDissolveVarianceName = "sprout-b-dissolve-variance";
		private static string sproutBSaturationName = "sprout-b-saturation";
		private static string sproutBSaturationModeName = "sprout-b-saturation-mode";
		private static string sproutBSaturationModeInvertName = "sprout-b-saturation-mode-invert";
		private static string sproutBSaturationVarianceName = "sprout-b-saturation-variance";
		private static string sproutBMetallicName = "sprout-b-metallic";
		private static string sproutBGlossinessName = "sprout-b-glossiness";
		private static string sproutBSubsurfaceName = "sprout-b-subsurface";

		private static string sproutCrownSaturationResetName = "sprout-crown-saturation-reset";
		private static string sproutCrownShadeResetName = "sprout-crown-shade-reset";
		private static string sproutCrownTintResetName = "sprout-crown-tint-reset";
		private static string sproutCrownDissolveResetName = "sprout-crown-dissolve-reset";
		private static string sproutCrownTintColorName = "sprout-crown-tint-color";
		private static string sproutCrownTintName = "sprout-crown-tint";
		private static string sproutCrownTintModeName = "sprout-crown-tint-mode";
		private static string sproutCrownTintModeInvertName = "sprout-crown-tint-mode-invert";
		private static string sproutCrownTintVarianceName = "sprout-crown-tint-variance";
		private static string sproutCrownShadeName = "sprout-crown-shade";
		private static string sproutCrownShadeModeName = "sprout-crown-shade-mode";
		private static string sproutCrownShadeModeInvertName = "sprout-crown-shade-mode-invert";
		private static string sproutCrownShadeVarianceName = "sprout-crown-shade-variance";
		private static string sproutCrownDissolveName = "sprout-crown-dissolve";
		private static string sproutCrownDissolveModeName = "sprout-crown-dissolve-mode";
		private static string sproutCrownDissolveModeInvertName = "sprout-crown-dissolve-mode-invert";
		private static string sproutCrownDissolveVarianceName = "sprout-crown-dissolve-variance";
		private static string sproutCrownSaturationName = "sprout-crown-saturation";
		private static string sproutCrownSaturationModeName = "sprout-crown-saturation-mode";
		private static string sproutCrownSaturationModeInvertName = "sprout-crown-saturation-mode-invert";
		private static string sproutCrownSaturationVarianceName = "sprout-crown-saturation-variance";
		private static string sproutCrownMetallicName = "sprout-crown-metallic";
		private static string sproutCrownGlossinessName = "sprout-crown-glossiness";
		private static string sproutCrownSubsurfaceName = "sprout-crown-subsurface";

		private ListView optionsList;
		private VisualElement stemContainer;
		private VisualElement sproutAContainerElem;
		private VisualElement sproutBContainerElem;
		private VisualElement sproutCrownContainerElem;
		private VisualElement sproutADissolveContainerElem;
		private VisualElement sproutBDissolveContainerElem;
		private VisualElement sproutCrownDissolveContainerElem;

		private Button sproutASaturationResetElem;
		private Button sproutAShadeResetElem;
		private Button sproutATintResetElem;
		private Button sproutADissolveResetElem;
		private ColorField sproutATintColorElem;
		private MinMaxSlider sproutATintElem;
		private EnumField sproutATintModeElem;
		private Toggle sproutATintModeInvertElem;
		private Slider sproutATintVarianceElem;
		private MinMaxSlider sproutAShadeElem;
		private EnumField sproutAShadeModeElem;
		private Toggle sproutAShadeModeInvertElem;
		private Slider sproutAShadeVarianceElem;
		private MinMaxSlider sproutADissolveElem;
		private EnumField sproutADissolveModeElem;
		private Toggle sproutADissolveModeInvertElem;
		private Slider sproutADissolveVarianceElem;
		private MinMaxSlider sproutASaturationElem;
		private EnumField sproutASaturationModeElem;
		private Toggle sproutASaturationModeInvertElem;
		private Slider sproutASaturationVarianceElem;
		private Slider sproutAMetallicElem;
		private Slider sproutAGlossinessElem;
		private Slider sproutASubsurfaceElem;

		private Button sproutBSaturationResetElem;
		private Button sproutBShadeResetElem;
		private Button sproutBTintResetElem;
		private Button sproutBDissolveResetElem;
		private ColorField sproutBTintColorElem;
		private MinMaxSlider sproutBTintElem;
		private EnumField sproutBTintModeElem;
		private Toggle sproutBTintModeInvertElem;
		private Slider sproutBTintVarianceElem;
		private MinMaxSlider sproutBShadeElem;
		private EnumField sproutBShadeModeElem;
		private Toggle sproutBShadeModeInvertElem;
		private Slider sproutBShadeVarianceElem;
		private MinMaxSlider sproutBDissolveElem;
		private EnumField sproutBDissolveModeElem;
		private Toggle sproutBDissolveModeInvertElem;
		private Slider sproutBDissolveVarianceElem;
		private MinMaxSlider sproutBSaturationElem;
		private EnumField sproutBSaturationModeElem;
		private Toggle sproutBSaturationModeInvertElem;
		private Slider sproutBSaturationVarianceElem;
		private Slider sproutBMetallicElem;
		private Slider sproutBGlossinessElem;
		private Slider sproutBSubsurfaceElem;

		private Button sproutCrownSaturationResetElem;
		private Button sproutCrownShadeResetElem;
		private Button sproutCrownTintResetElem;
		private Button sproutCrownDissolveResetElem;
		private ColorField sproutCrownTintColorElem;
		private MinMaxSlider sproutCrownTintElem;
		private EnumField sproutCrownTintModeElem;
		private Toggle sproutCrownTintModeInvertElem;
		private Slider sproutCrownTintVarianceElem;
		private MinMaxSlider sproutCrownShadeElem;
		private EnumField sproutCrownShadeModeElem;
		private Toggle sproutCrownShadeModeInvertElem;
		private Slider sproutCrownShadeVarianceElem;
		private MinMaxSlider sproutCrownDissolveElem;
		private EnumField sproutCrownDissolveModeElem;
		private Toggle sproutCrownDissolveModeInvertElem;
		private Slider sproutCrownDissolveVarianceElem;
		private MinMaxSlider sproutCrownSaturationElem;
		private EnumField sproutCrownSaturationModeElem;
		private Toggle sproutCrownSaturationModeInvertElem;
		private Slider sproutCrownSaturationVarianceElem;
		private Slider sproutCrownMetallicElem;
		private Slider sproutCrownGlossinessElem;
		private Slider sproutCrownSubsurfaceElem;
        #endregion

        #region Constructor
        public SproutLabMappingPanel (SproutLabEditor sproutLabEditor) {
            Initialize (sproutLabEditor);
        }
        #endregion

        #region Init
		public void SproutSelected () {
			if (sproutLabEditor.snapSettings.hasCrown) {
				optionsList.bindItem = bindOptionCrownItem;
				optionsList.itemsSource = optionCrownItems;
				optionsList.selectedIndex = 0;
			} else {
				optionsList.bindItem = bindOptionItem;
				optionsList.itemsSource = optionItems;
			}
			#if UNITY_2021_2_OR_NEWER
			optionsList.Rebuild ();
			#else
			optionsList.Refresh ();
			#endif
		}
		private void OnSelectionChanged(IEnumerable<object> selectedItems) {
			stemContainer.style.display = DisplayStyle.None;
			sproutAContainerElem.style.display = DisplayStyle.None;
			sproutBContainerElem.style.display = DisplayStyle.None;
			sproutCrownContainerElem.style.display = DisplayStyle.None;
			if (optionsList.selectedIndex == 1 ) {
				sproutAContainerElem.style.display = DisplayStyle.Flex;
			} else if (optionsList.selectedIndex == 2) {
				sproutBContainerElem.style.display = DisplayStyle.Flex;
			} else if (optionsList.selectedIndex == 3) {
				sproutCrownContainerElem.style.display = DisplayStyle.Flex;
			} else {
				stemContainer.style.display = DisplayStyle.Flex;
			}
		}
        public void Initialize (SproutLabEditor sproutLabEditor) {
			this.sproutLabEditor = sproutLabEditor;
            if (!isInit) {
                // Start the container UIElement.
                container = new VisualElement ();
                container.name = containerName;

				// Load the VisualTreeAsset from a file 
				panelXml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(panelXmlPath);

				// Create a new instance of the root VisualElement
				container.Add (panelXml.CloneTree()); 

				// Init List and Containers.
				optionsList = container.Q<ListView> (optionsListName);
				stemContainer = container.Q<VisualElement> (stemContainerName);
				sproutAContainerElem = container.Q<VisualElement> (sproutAContainerName);
				sproutBContainerElem = container.Q<VisualElement> (sproutBContainerName);
				sproutCrownContainerElem = container.Q<VisualElement> (crownContainerName);
				SetContainerIcons (sproutAContainerElem);
				SetContainerIcons (sproutBContainerElem);
				SetContainerIcons (sproutCrownContainerElem);

				// Dissolve containers (hide if experimental is disabled).
				if (!Broccoli.Base.GlobalSettings.experimentalSproutLabDissolveSprouts) {
					sproutADissolveContainerElem = sproutAContainerElem.Q<VisualElement> (dissolveContainerName);
					sproutBDissolveContainerElem = sproutBContainerElem.Q<VisualElement> (dissolveContainerName);
					sproutCrownDissolveContainerElem = sproutCrownContainerElem.Q<VisualElement> (dissolveContainerName);
					sproutADissolveContainerElem.style.display = DisplayStyle.None;
					sproutBDissolveContainerElem.style.display = DisplayStyle.None;
					sproutCrownDissolveContainerElem.style.display = DisplayStyle.None;
				}

				// The "makeItem" function will be called as needed
				// when the ListView needs more items to render
				Func<VisualElement> makeItem = () => new Label();

				optionsList.makeItem = makeItem;
				optionsList.bindItem = bindOptionItem;
				optionsList.itemsSource = optionItems;
				#if UNITY_2021_2_OR_NEWER
				optionsList.Rebuild ();
				#else
				optionsList.Refresh ();
				#endif
				
				#if UNITY_2022_2_OR_NEWER
				optionsList.selectionChanged -= OnSelectionChanged;
				optionsList.selectionChanged += OnSelectionChanged;
				#else
				optionsList.onSelectionChange -= OnSelectionChanged;
				optionsList.onSelectionChange += OnSelectionChanged;
				#endif
				optionsList.selectedIndex = 0;


				InitializeSproutStyleA ();
				InitializeSproutStyleB ();
				InitializeSproutStyleCrown ();

				isInit = true;

                RefreshValues ();
            }
        }
		
		void InitializeSproutStyleA () {
			// SPROUT A.
			// Saturation Range.
			sproutASaturationElem = container.Q<MinMaxSlider> (sproutASaturationName);
			sproutASaturationElem?.RegisterValueChangedCallback(evt => {
				Vector2 newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorSaturation = newVal.x;
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorSaturation = newVal.y;
					OnEdit (false, false, true, false, true);
				}
			});
			SproutLabEditor.SetupMinMaxSlider (sproutASaturationElem);

			// Saturation Mode.
			sproutASaturationModeElem = container.Q<EnumField> (sproutASaturationModeName);
			sproutASaturationModeElem.Init (BranchDescriptorCollection.SproutStyle.SproutSaturationMode.Uniform);
			sproutASaturationModeElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutSaturationMode = (BranchDescriptorCollection.SproutStyle.SproutSaturationMode)evt.newValue;
					if (sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutSaturationMode == BranchDescriptorCollection.SproutStyle.SproutSaturationMode.Uniform) {
						sproutASaturationModeInvertElem.style.display = DisplayStyle.None;
						sproutASaturationVarianceElem.style.display = DisplayStyle.None;
					} else {
						sproutASaturationModeInvertElem.style.display = DisplayStyle.Flex;
						sproutASaturationVarianceElem.style.display = DisplayStyle.Flex;
					}
					OnEdit (false, false, true, false, true);
				}
			});

			// Saturation Invert.
			sproutASaturationModeInvertElem = container.Q<Toggle> (sproutASaturationModeInvertName);
			sproutASaturationModeInvertElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutSaturationMode = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});

			// Saturation Variance.
			sproutASaturationVarianceElem = container.Q<Slider> (sproutASaturationVarianceName);
			sproutASaturationVarianceElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutSaturationVariance = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutASaturationVarianceElem);

			// Shade Range
			sproutAShadeElem = container.Q<MinMaxSlider> (sproutAShadeName);
			sproutAShadeElem?.RegisterValueChangedCallback(evt => {
				Vector2 newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorShade = newVal.x;
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorShade = newVal.y;
					OnEdit (true);
					UpdateShade (0);
				}
			});
			SproutLabEditor.SetupMinMaxSlider (sproutAShadeElem);

			// Shade Mode.
			sproutAShadeModeElem = container.Q<EnumField> (sproutAShadeModeName);
			sproutAShadeModeElem.Init (BranchDescriptorCollection.SproutStyle.SproutShadeMode.Uniform);
			sproutAShadeModeElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutShadeMode = (BranchDescriptorCollection.SproutStyle.SproutShadeMode)evt.newValue;
					if (sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutShadeMode == BranchDescriptorCollection.SproutStyle.SproutShadeMode.Uniform) {
						sproutAShadeModeInvertElem.style.display = DisplayStyle.None;
						sproutAShadeVarianceElem.style.display = DisplayStyle.None;
					} else {
						sproutAShadeModeInvertElem.style.display = DisplayStyle.Flex;
						sproutAShadeVarianceElem.style.display = DisplayStyle.Flex;
					}
					OnEdit (true);
					UpdateShade (0);
				}
			});

			// Shade Invert.
			sproutAShadeModeInvertElem = container.Q<Toggle> (sproutAShadeModeInvertName);
			sproutAShadeModeInvertElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutShadeMode = evt.newValue;
					OnEdit (true);
					UpdateShade (0);
				}
			});

			// Shade Variance.
			sproutAShadeVarianceElem = container.Q<Slider> (sproutAShadeVarianceName);
			sproutAShadeVarianceElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutShadeVariance = evt.newValue;
					OnEdit (true);
					UpdateShade (0);
				}
			});
			SproutLabEditor.SetupSlider (sproutAShadeVarianceElem);

			// Dissolve Range
			sproutADissolveElem = container.Q<MinMaxSlider> (sproutADissolveName);
			sproutADissolveElem?.RegisterValueChangedCallback(evt => {
				Vector2 newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorDissolve = newVal.x;
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorDissolve = newVal.y;
					OnEdit (true);
					UpdateDissolve (0);
				}
			});
			SproutLabEditor.SetupMinMaxSlider (sproutADissolveElem);

			// Dissolve Mode.
			sproutADissolveModeElem = container.Q<EnumField> (sproutADissolveModeName);
			sproutADissolveModeElem.Init (BranchDescriptorCollection.SproutStyle.SproutDissolveMode.Uniform);
			sproutADissolveModeElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutDissolveMode = (BranchDescriptorCollection.SproutStyle.SproutDissolveMode)evt.newValue;
					if (sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutDissolveMode == BranchDescriptorCollection.SproutStyle.SproutDissolveMode.Uniform) {
						sproutADissolveModeInvertElem.style.display = DisplayStyle.None;
						sproutADissolveVarianceElem.style.display = DisplayStyle.None;
					} else {
						sproutADissolveModeInvertElem.style.display = DisplayStyle.Flex;
						sproutADissolveVarianceElem.style.display = DisplayStyle.Flex;
					}
					OnEdit (true);
					UpdateDissolve (0);
				}
			});

			// Dissolve Invert.
			sproutADissolveModeInvertElem = container.Q<Toggle> (sproutADissolveModeInvertName);
			sproutADissolveModeInvertElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutDissolveMode = evt.newValue;
					OnEdit (true);
					UpdateDissolve (0);
				}
			});

			// Dissolve Variance.
			sproutADissolveVarianceElem = container.Q<Slider> (sproutADissolveVarianceName);
			sproutADissolveVarianceElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutDissolveVariance = evt.newValue;
					OnEdit (true);
					UpdateDissolve (0);
				}
			});
			SproutLabEditor.SetupSlider (sproutADissolveVarianceElem);

			// Tint Color.
			sproutATintColorElem = container.Q<ColorField> (sproutATintColorName);
			sproutATintColorElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.colorTint = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});

			// Tint Range.
			sproutATintElem = container.Q<MinMaxSlider> (sproutATintName);
			sproutATintElem?.RegisterValueChangedCallback(evt => {
				Vector2 newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorTint = newVal.x;
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorTint = newVal.y;
					OnEdit (false, false, true, false, true);
				}
			});
			SproutLabEditor.SetupMinMaxSlider (sproutATintElem);

			// Tint Mode.
			sproutATintModeElem = container.Q<EnumField> (sproutATintModeName);
			sproutATintModeElem.Init (BranchDescriptorCollection.SproutStyle.SproutTintMode.Uniform);
			sproutATintModeElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutTintMode = (BranchDescriptorCollection.SproutStyle.SproutTintMode)evt.newValue;
					if (sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutTintMode == BranchDescriptorCollection.SproutStyle.SproutTintMode.Uniform) {
						sproutATintModeInvertElem.style.display = DisplayStyle.None;
						sproutATintVarianceElem.style.display = DisplayStyle.None;
					} else {
						sproutATintModeInvertElem.style.display = DisplayStyle.Flex;
						sproutATintVarianceElem.style.display = DisplayStyle.Flex;
					}
					OnEdit (false, false, true, false, true);
				}
			});

			// Tint Invert.
			sproutATintModeInvertElem = container.Q<Toggle> (sproutATintModeInvertName);
			sproutATintModeInvertElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutTintMode = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});

			// Tint Variance.
			sproutATintVarianceElem = container.Q<Slider> (sproutATintVarianceName);
			sproutATintVarianceElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutTintVariance = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutATintVarianceElem);

			// Metallic Slider.
			sproutAMetallicElem = container.Q<Slider> (sproutAMetallicName);
			sproutAMetallicElem?.RegisterValueChangedCallback(evt => {
				float newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.metallic = newVal;
					OnEdit (true, true, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutAMetallicElem);

			// Glossiness Slider.
			sproutAGlossinessElem = container.Q<Slider> (sproutAGlossinessName);
			sproutAGlossinessElem?.RegisterValueChangedCallback(evt => {
				float newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.glossiness = newVal;
					OnEdit (true, true, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutAGlossinessElem);

			// Subsurface Slider.
			sproutASubsurfaceElem = container.Q<Slider> (sproutASubsurfaceName);
			sproutASubsurfaceElem?.RegisterValueChangedCallback(evt => {
				float newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleA.subsurface = newVal;
					OnEdit (true, true, false, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutASubsurfaceElem);

			// Saturation Reset Button.
			sproutASaturationResetElem = container.Q<Button> (sproutASaturationResetName);
			sproutASaturationResetElem?.UnregisterCallback<ClickEvent> (ResetSproutASaturation);
			sproutASaturationResetElem?.RegisterCallback<ClickEvent> (ResetSproutASaturation);
			// Shade Reset Button.
			sproutAShadeResetElem = container.Q<Button> (sproutAShadeResetName);
			sproutAShadeResetElem?.UnregisterCallback<ClickEvent> (ResetSproutAShade);
			sproutAShadeResetElem?.RegisterCallback<ClickEvent> (ResetSproutAShade);
			// Tint Reset Button.
			sproutATintResetElem = container.Q<Button> (sproutATintResetName);
			sproutATintResetElem?.UnregisterCallback<ClickEvent> (ResetSproutATint);
			sproutATintResetElem?.RegisterCallback<ClickEvent> (ResetSproutATint);
			// Dissolve Reset Button.
			sproutADissolveResetElem = container.Q<Button> (sproutADissolveResetName);
			sproutADissolveResetElem?.UnregisterCallback<ClickEvent> (ResetSproutADissolve);
			sproutADissolveResetElem?.RegisterCallback<ClickEvent> (ResetSproutADissolve);
		}
		void ResetSproutASaturation (ClickEvent evt) {
			if (listenGUIEvents) {
				OnBeforeEdit ();
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorSaturation = 1;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorSaturation = 1;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutSaturationMode = 
					BranchDescriptorCollection.SproutStyle.SproutSaturationMode.Uniform;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutSaturationMode = false;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutSaturationVariance = 0;
				RefreshStyleAValues ();
				OnEdit (true, true, false, false, true);
			}
		}
		void ResetSproutAShade (ClickEvent evt) {
			if (listenGUIEvents) {
				OnBeforeEdit ();
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorShade = 1;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorShade = 1;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutShadeMode = 
					BranchDescriptorCollection.SproutStyle.SproutShadeMode.Uniform;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutShadeMode = false;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutShadeVariance = 0;
				RefreshStyleAValues ();
				OnEdit (true, true, false, false, true);
			}
		}
		void ResetSproutATint (ClickEvent evt) {
			if (listenGUIEvents) {
				OnBeforeEdit ();
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorTint = 0;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorTint = 0;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutTintMode = 
					BranchDescriptorCollection.SproutStyle.SproutTintMode.Uniform;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutTintMode = false;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutTintVariance = 0;
				RefreshStyleAValues ();
				OnEdit (true, true, false, false, true);
			}
		}
		void ResetSproutADissolve (ClickEvent evt) {
			if (listenGUIEvents) {
				OnBeforeEdit ();
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorDissolve = 0;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorDissolve = 0;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutDissolveMode = 
					BranchDescriptorCollection.SproutStyle.SproutDissolveMode.Uniform;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutDissolveMode = false;
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutDissolveVariance = 0;
				RefreshStyleAValues ();
				OnEdit (true, true, false, false, true);
			}
		}
		void InitializeSproutStyleB () {
			// SPROUT B.
			// Saturation Range.
			sproutBSaturationElem = container.Q<MinMaxSlider> (sproutBSaturationName);
			sproutBSaturationElem?.RegisterValueChangedCallback(evt => {
				Vector2 newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorSaturation = newVal.x;
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorSaturation = newVal.y;
					OnEdit (false, false, true, false, true);
				}
			});
			SproutLabEditor.SetupMinMaxSlider (sproutBSaturationElem);

			// Saturation Mode.
			sproutBSaturationModeElem = container.Q<EnumField> (sproutBSaturationModeName);
			sproutBSaturationModeElem.Init (BranchDescriptorCollection.SproutStyle.SproutSaturationMode.Uniform);
			sproutBSaturationModeElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutSaturationMode = (BranchDescriptorCollection.SproutStyle.SproutSaturationMode)evt.newValue;
					if (sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutSaturationMode == BranchDescriptorCollection.SproutStyle.SproutSaturationMode.Uniform) {
						sproutBSaturationModeInvertElem.style.display = DisplayStyle.None;
						sproutBSaturationVarianceElem.style.display = DisplayStyle.None;
					} else {
						sproutBSaturationModeInvertElem.style.display = DisplayStyle.Flex;
						sproutBSaturationVarianceElem.style.display = DisplayStyle.Flex;
					}
					OnEdit (false, false, true, false, true);
				}
			});

			// Saturation Invert.
			sproutBSaturationModeInvertElem = container.Q<Toggle> (sproutBSaturationModeInvertName);
			sproutBSaturationModeInvertElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutSaturationMode = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});

			// Saturation Variance.
			sproutBSaturationVarianceElem = container.Q<Slider> (sproutBSaturationVarianceName);
			sproutBSaturationVarianceElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutSaturationVariance = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutBSaturationVarianceElem);

			// Shade Range
			sproutBShadeElem = container.Q<MinMaxSlider> (sproutBShadeName);
			sproutBShadeElem?.RegisterValueChangedCallback(evt => {
				Vector2 newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorShade = newVal.x;
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorShade = newVal.y;
					OnEdit (true);
					UpdateShade (1);
				}
			});
			SproutLabEditor.SetupMinMaxSlider (sproutBShadeElem);

			// Shade Mode.
			sproutBShadeModeElem = container.Q<EnumField> (sproutBShadeModeName);
			sproutBShadeModeElem.Init (BranchDescriptorCollection.SproutStyle.SproutShadeMode.Uniform);
			sproutBShadeModeElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutShadeMode = (BranchDescriptorCollection.SproutStyle.SproutShadeMode)evt.newValue;
					if (sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutShadeMode == BranchDescriptorCollection.SproutStyle.SproutShadeMode.Uniform) {
						sproutBShadeModeInvertElem.style.display = DisplayStyle.None;
						sproutBShadeVarianceElem.style.display = DisplayStyle.None;
					} else {
						sproutBShadeModeInvertElem.style.display = DisplayStyle.Flex;
						sproutBShadeVarianceElem.style.display = DisplayStyle.Flex;
					}
					OnEdit (true);
					UpdateShade (1);
				}
			});

			// Shade Invert.
			sproutBShadeModeInvertElem = container.Q<Toggle> (sproutBShadeModeInvertName);
			sproutBShadeModeInvertElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutShadeMode = evt.newValue;
					OnEdit (true);
					UpdateShade (1);
				}
			});

			// Shade Variance.
			sproutBShadeVarianceElem = container.Q<Slider> (sproutBShadeVarianceName);
			sproutBShadeVarianceElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutShadeVariance = evt.newValue;
					OnEdit (true);
					UpdateShade (1);
				}
			});
			SproutLabEditor.SetupSlider (sproutBShadeVarianceElem);

			// Dissolve Range
			sproutBDissolveElem = container.Q<MinMaxSlider> (sproutBDissolveName);
			sproutBDissolveElem?.RegisterValueChangedCallback(evt => {
				Vector2 newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorDissolve = newVal.x;
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorDissolve = newVal.y;
					OnEdit (true);
					UpdateDissolve (0);
				}
			});
			SproutLabEditor.SetupMinMaxSlider (sproutBDissolveElem);

			// Dissolve Mode.
			sproutBDissolveModeElem = container.Q<EnumField> (sproutBDissolveModeName);
			sproutBDissolveModeElem.Init (BranchDescriptorCollection.SproutStyle.SproutDissolveMode.Uniform);
			sproutBDissolveModeElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutDissolveMode = (BranchDescriptorCollection.SproutStyle.SproutDissolveMode)evt.newValue;
					if (sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutDissolveMode == BranchDescriptorCollection.SproutStyle.SproutDissolveMode.Uniform) {
						sproutBDissolveModeInvertElem.style.display = DisplayStyle.None;
						sproutBDissolveVarianceElem.style.display = DisplayStyle.None;
					} else {
						sproutBDissolveModeInvertElem.style.display = DisplayStyle.Flex;
						sproutBDissolveVarianceElem.style.display = DisplayStyle.Flex;
					}
					OnEdit (true);
					UpdateDissolve (0);
				}
			});

			// Dissolve Invert.
			sproutBDissolveModeInvertElem = container.Q<Toggle> (sproutBDissolveModeInvertName);
			sproutBDissolveModeInvertElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutDissolveMode = evt.newValue;
					OnEdit (true);
					UpdateDissolve (0);
				}
			});

			// Dissolve Variance.
			sproutBDissolveVarianceElem = container.Q<Slider> (sproutBDissolveVarianceName);
			sproutBDissolveVarianceElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutDissolveVariance = evt.newValue;
					OnEdit (true);
					UpdateDissolve (0);
				}
			});
			SproutLabEditor.SetupSlider (sproutBDissolveVarianceElem);

			// Tint Color.
			sproutBTintColorElem = container.Q<ColorField> (sproutBTintColorName);
			sproutBTintColorElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.colorTint = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});

			// Tint Range.
			sproutBTintElem = container.Q<MinMaxSlider> (sproutBTintName);
			sproutBTintElem?.RegisterValueChangedCallback(evt => {
				Vector2 newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorTint = newVal.x;
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorTint = newVal.y;
					OnEdit (false, false, true, false, true);
				}
			});
			SproutLabEditor.SetupMinMaxSlider (sproutBTintElem);

			// Tint Mode.
			sproutBTintModeElem = container.Q<EnumField> (sproutBTintModeName);
			sproutBTintModeElem.Init (BranchDescriptorCollection.SproutStyle.SproutTintMode.Uniform);
			sproutBTintModeElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutTintMode = (BranchDescriptorCollection.SproutStyle.SproutTintMode)evt.newValue;
					if (sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutTintMode == BranchDescriptorCollection.SproutStyle.SproutTintMode.Uniform) {
						sproutBTintModeInvertElem.style.display = DisplayStyle.None;
						sproutBTintVarianceElem.style.display = DisplayStyle.None;
					} else {
						sproutBTintModeInvertElem.style.display = DisplayStyle.Flex;
						sproutBTintVarianceElem.style.display = DisplayStyle.Flex;
					}
					OnEdit (false, false, true, false, true);
				}
			});

			// Tint Invert.
			sproutBTintModeInvertElem = container.Q<Toggle> (sproutBTintModeInvertName);
			sproutBTintModeInvertElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutTintMode = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});

			// Tint Variance.
			sproutBTintVarianceElem = container.Q<Slider> (sproutBTintVarianceName);
			sproutBTintVarianceElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutTintVariance = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutBTintVarianceElem);

			// Metallic Slider.
			sproutBMetallicElem = container.Q<Slider> (sproutBMetallicName);
			sproutBMetallicElem?.RegisterValueChangedCallback(evt => {
				float newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.metallic = newVal;
					OnEdit (true, true, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutBMetallicElem);

			// Glossiness Slider.
			sproutBGlossinessElem = container.Q<Slider> (sproutBGlossinessName);
			sproutBGlossinessElem?.RegisterValueChangedCallback(evt => {
				float newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.glossiness = newVal;
					OnEdit (true, true, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutBGlossinessElem);

			// Subsurface Slider.
			sproutBSubsurfaceElem = container.Q<Slider> (sproutBSubsurfaceName);
			sproutBSubsurfaceElem?.RegisterValueChangedCallback(evt => {
				float newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleB.subsurface = newVal;
					OnEdit (true, true, false, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutBSubsurfaceElem);

			// Saturation Reset Button.
			sproutBSaturationResetElem = container.Q<Button> (sproutBSaturationResetName);
			sproutBSaturationResetElem?.UnregisterCallback<ClickEvent> (ResetSproutBSaturation);
			sproutBSaturationResetElem?.RegisterCallback<ClickEvent> (ResetSproutBSaturation);
			// Shade Reset Button.
			sproutBShadeResetElem = container.Q<Button> (sproutBShadeResetName);
			sproutBShadeResetElem?.UnregisterCallback<ClickEvent> (ResetSproutBShade);
			sproutBShadeResetElem?.RegisterCallback<ClickEvent> (ResetSproutBShade);
			// Tint Reset Button.
			sproutBTintResetElem = container.Q<Button> (sproutBTintResetName);
			sproutBTintResetElem?.UnregisterCallback<ClickEvent> (ResetSproutBTint);
			sproutBTintResetElem?.RegisterCallback<ClickEvent> (ResetSproutBTint);
			// Dissolve Reset Button.
			sproutBDissolveResetElem = container.Q<Button> (sproutBDissolveResetName);
			sproutBDissolveResetElem?.UnregisterCallback<ClickEvent> (ResetSproutBDissolve);
			sproutBDissolveResetElem?.RegisterCallback<ClickEvent> (ResetSproutBDissolve);
		}
		void ResetSproutBSaturation (ClickEvent evt) {
			if (listenGUIEvents) {
				OnBeforeEdit ();
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorSaturation = 1;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorSaturation = 1;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutSaturationMode = 
					BranchDescriptorCollection.SproutStyle.SproutSaturationMode.Uniform;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutSaturationMode = false;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutSaturationVariance = 0;
				RefreshStyleBValues ();
				OnEdit (true, true, false, false, true);
			}
		}
		void ResetSproutBShade (ClickEvent evt) {
			if (listenGUIEvents) {
				OnBeforeEdit ();
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorShade = 1;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorShade = 1;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutShadeMode = 
					BranchDescriptorCollection.SproutStyle.SproutShadeMode.Uniform;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutShadeMode = false;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutShadeVariance = 0;
				RefreshStyleBValues ();
				OnEdit (true, true, false, false, true);
			}
		}
		void ResetSproutBTint (ClickEvent evt) {
			if (listenGUIEvents) {
				OnBeforeEdit ();
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorTint = 0;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorTint = 0;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutTintMode = 
					BranchDescriptorCollection.SproutStyle.SproutTintMode.Uniform;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutTintMode = false;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutTintVariance = 0;
				RefreshStyleBValues ();
				OnEdit (true, true, false, false, true);
			}
		}
		void ResetSproutBDissolve (ClickEvent evt) {
			if (listenGUIEvents) {
				OnBeforeEdit ();
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorDissolve = 0;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorDissolve = 0;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutDissolveMode = 
					BranchDescriptorCollection.SproutStyle.SproutDissolveMode.Uniform;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutDissolveMode = false;
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutDissolveVariance = 0;
				RefreshStyleBValues ();
				OnEdit (true, true, false, false, true);
			}
		}
		void InitializeSproutStyleCrown () {
			// SPROUT B.
			// Saturation Range.
			sproutCrownSaturationElem = container.Q<MinMaxSlider> (sproutCrownSaturationName);
			sproutCrownSaturationElem?.RegisterValueChangedCallback(evt => {
				Vector2 newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorSaturation = newVal.x;
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorSaturation = newVal.y;
					OnEdit (false, false, true, false, true);
				}
			});
			SproutLabEditor.SetupMinMaxSlider (sproutCrownSaturationElem);

			// Saturation Mode.
			sproutCrownSaturationModeElem = container.Q<EnumField> (sproutCrownSaturationModeName);
			sproutCrownSaturationModeElem.Init (BranchDescriptorCollection.SproutStyle.SproutSaturationMode.Uniform);
			sproutCrownSaturationModeElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutSaturationMode = (BranchDescriptorCollection.SproutStyle.SproutSaturationMode)evt.newValue;
					if (sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutSaturationMode == BranchDescriptorCollection.SproutStyle.SproutSaturationMode.Uniform) {
						sproutCrownSaturationModeInvertElem.style.display = DisplayStyle.None;
						sproutCrownSaturationVarianceElem.style.display = DisplayStyle.None;
					} else {
						sproutCrownSaturationModeInvertElem.style.display = DisplayStyle.Flex;
						sproutCrownSaturationVarianceElem.style.display = DisplayStyle.Flex;
					}
					OnEdit (false, false, true, false, true);
				}
			});

			// Saturation Invert.
			sproutCrownSaturationModeInvertElem = container.Q<Toggle> (sproutCrownSaturationModeInvertName);
			sproutCrownSaturationModeInvertElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutSaturationMode = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});

			// Saturation Variance.
			sproutCrownSaturationVarianceElem = container.Q<Slider> (sproutCrownSaturationVarianceName);
			sproutCrownSaturationVarianceElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutSaturationVariance = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutCrownSaturationVarianceElem);

			// Shade Range
			sproutCrownShadeElem = container.Q<MinMaxSlider> (sproutCrownShadeName);
			sproutCrownShadeElem?.RegisterValueChangedCallback(evt => {
				Vector2 newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorShade = newVal.x;
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorShade = newVal.y;
					OnEdit (true);
					UpdateShade (2);
				}
			});
			SproutLabEditor.SetupMinMaxSlider (sproutCrownShadeElem);

			// Shade Mode.
			sproutCrownShadeModeElem = container.Q<EnumField> (sproutCrownShadeModeName);
			sproutCrownShadeModeElem.Init (BranchDescriptorCollection.SproutStyle.SproutShadeMode.Uniform);
			sproutCrownShadeModeElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutShadeMode = (BranchDescriptorCollection.SproutStyle.SproutShadeMode)evt.newValue;
					if (sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutShadeMode == BranchDescriptorCollection.SproutStyle.SproutShadeMode.Uniform) {
						sproutCrownShadeModeInvertElem.style.display = DisplayStyle.None;
						sproutCrownShadeVarianceElem.style.display = DisplayStyle.None;
					} else {
						sproutCrownShadeModeInvertElem.style.display = DisplayStyle.Flex;
						sproutCrownShadeVarianceElem.style.display = DisplayStyle.Flex;
					}
					OnEdit (true);
					UpdateShade (2);
				}
			});

			// Shade Invert.
			sproutCrownShadeModeInvertElem = container.Q<Toggle> (sproutCrownShadeModeInvertName);
			sproutCrownShadeModeInvertElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutShadeMode = evt.newValue;
					OnEdit (true);
					UpdateShade (2);
				}
			});

			// Shade Variance.
			sproutCrownShadeVarianceElem = container.Q<Slider> (sproutCrownShadeVarianceName);
			sproutCrownShadeVarianceElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutShadeVariance = evt.newValue;
					OnEdit (true);
					UpdateShade (2);
				}
			});
			SproutLabEditor.SetupSlider (sproutCrownShadeVarianceElem);

			// Dissolve Range
			sproutCrownDissolveElem = container.Q<MinMaxSlider> (sproutCrownDissolveName);
			sproutCrownDissolveElem?.RegisterValueChangedCallback(evt => {
				Vector2 newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorDissolve = newVal.x;
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorDissolve = newVal.y;
					OnEdit (true);
					UpdateDissolve (0);
				}
			});
			SproutLabEditor.SetupMinMaxSlider (sproutCrownDissolveElem);

			// Dissolve Mode.
			sproutCrownDissolveModeElem = container.Q<EnumField> (sproutCrownDissolveModeName);
			sproutCrownDissolveModeElem.Init (BranchDescriptorCollection.SproutStyle.SproutDissolveMode.Uniform);
			sproutCrownDissolveModeElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutDissolveMode = (BranchDescriptorCollection.SproutStyle.SproutDissolveMode)evt.newValue;
					if (sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutDissolveMode == BranchDescriptorCollection.SproutStyle.SproutDissolveMode.Uniform) {
						sproutCrownDissolveModeInvertElem.style.display = DisplayStyle.None;
						sproutCrownDissolveVarianceElem.style.display = DisplayStyle.None;
					} else {
						sproutCrownDissolveModeInvertElem.style.display = DisplayStyle.Flex;
						sproutCrownDissolveVarianceElem.style.display = DisplayStyle.Flex;
					}
					OnEdit (true);
					UpdateDissolve (0);
				}
			});

			// Dissolve Invert.
			sproutCrownDissolveModeInvertElem = container.Q<Toggle> (sproutCrownDissolveModeInvertName);
			sproutCrownDissolveModeInvertElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutDissolveMode = evt.newValue;
					OnEdit (true);
					UpdateDissolve (0);
				}
			});

			// Dissolve Variance.
			sproutCrownDissolveVarianceElem = container.Q<Slider> (sproutCrownDissolveVarianceName);
			sproutCrownDissolveVarianceElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutDissolveVariance = evt.newValue;
					OnEdit (true);
					UpdateDissolve (0);
				}
			});
			SproutLabEditor.SetupSlider (sproutCrownDissolveVarianceElem);

			// Tint Color.
			sproutCrownTintColorElem = container.Q<ColorField> (sproutCrownTintColorName);
			sproutCrownTintColorElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.colorTint = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});

			// Tint Range.
			sproutCrownTintElem = container.Q<MinMaxSlider> (sproutCrownTintName);
			sproutCrownTintElem?.RegisterValueChangedCallback(evt => {
				Vector2 newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorTint = newVal.x;
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorTint = newVal.y;
					OnEdit (false, false, true, false, true);
				}
			});
			SproutLabEditor.SetupMinMaxSlider (sproutCrownTintElem);

			// Tint Mode.
			sproutCrownTintModeElem = container.Q<EnumField> (sproutCrownTintModeName);
			sproutCrownTintModeElem.Init (BranchDescriptorCollection.SproutStyle.SproutTintMode.Uniform);
			sproutCrownTintModeElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutTintMode = (BranchDescriptorCollection.SproutStyle.SproutTintMode)evt.newValue;
					if (sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutTintMode == BranchDescriptorCollection.SproutStyle.SproutTintMode.Uniform) {
						sproutCrownTintModeInvertElem.style.display = DisplayStyle.None;
						sproutCrownTintVarianceElem.style.display = DisplayStyle.None;
					} else {
						sproutCrownTintModeInvertElem.style.display = DisplayStyle.Flex;
						sproutCrownTintVarianceElem.style.display = DisplayStyle.Flex;
					}
					OnEdit (false, false, true, false, true);
				}
			});

			// Tint Invert.
			sproutCrownTintModeInvertElem = container.Q<Toggle> (sproutCrownTintModeInvertName);
			sproutCrownTintModeInvertElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutTintMode = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});

			// Tint Variance.
			sproutCrownTintVarianceElem = container.Q<Slider> (sproutCrownTintVarianceName);
			sproutCrownTintVarianceElem?.RegisterValueChangedCallback(evt => {
				if (listenGUIEvents && evt.newValue != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutTintVariance = evt.newValue;
					OnEdit (false, false, true, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutCrownTintVarianceElem);

			// Metallic Slider.
			sproutCrownMetallicElem = container.Q<Slider> (sproutCrownMetallicName);
			sproutCrownMetallicElem?.RegisterValueChangedCallback(evt => {
				float newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.metallic = newVal;
					OnEdit (true, true, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutCrownMetallicElem);

			// Glossiness Slider.
			sproutCrownGlossinessElem = container.Q<Slider> (sproutCrownGlossinessName);
			sproutCrownGlossinessElem?.RegisterValueChangedCallback(evt => {
				float newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.glossiness = newVal;
					OnEdit (true, true, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutCrownGlossinessElem);

			// Subsurface Slider.
			sproutCrownSubsurfaceElem = container.Q<Slider> (sproutCrownSubsurfaceName);
			sproutCrownSubsurfaceElem?.RegisterValueChangedCallback(evt => {
				float newVal = evt.newValue;
				if (listenGUIEvents && newVal != evt.previousValue) {
					OnBeforeEdit ();
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.subsurface = newVal;
					OnEdit (true, true, false, false, true);
				}
			});
			SproutLabEditor.SetupSlider (sproutCrownSubsurfaceElem);

			// Saturation Reset Button.
			sproutCrownSaturationResetElem = container.Q<Button> (sproutCrownSaturationResetName);
			sproutCrownSaturationResetElem?.UnregisterCallback<ClickEvent> (ResetSproutCrownSaturation);
			sproutCrownSaturationResetElem?.RegisterCallback<ClickEvent> (ResetSproutCrownSaturation);
			// Shade Reset Button.
			sproutCrownShadeResetElem = container.Q<Button> (sproutCrownShadeResetName);
			sproutCrownShadeResetElem?.UnregisterCallback<ClickEvent> (ResetSproutCrownShade);
			sproutCrownShadeResetElem?.RegisterCallback<ClickEvent> (ResetSproutCrownShade);
			// Tint Reset Button.
			sproutCrownTintResetElem = container.Q<Button> (sproutCrownTintResetName);
			sproutCrownTintResetElem?.UnregisterCallback<ClickEvent> (ResetSproutCrownTint);
			sproutCrownTintResetElem?.RegisterCallback<ClickEvent> (ResetSproutCrownTint);
			// Dissolve Reset Button.
			sproutCrownDissolveResetElem = container.Q<Button> (sproutCrownDissolveResetName);
			sproutCrownDissolveResetElem?.UnregisterCallback<ClickEvent> (ResetSproutCrownDissolve);
			sproutCrownDissolveResetElem?.RegisterCallback<ClickEvent> (ResetSproutCrownDissolve);
		}
		void ResetSproutCrownSaturation (ClickEvent evt) {
			if (listenGUIEvents) {
				OnBeforeEdit ();
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorSaturation = 1;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorSaturation = 1;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutSaturationMode = 
					BranchDescriptorCollection.SproutStyle.SproutSaturationMode.Uniform;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutSaturationMode = false;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutSaturationVariance = 0;
				RefreshStyleCrownValues ();
				OnEdit (true, true, false, false, true);
			}
		}
		void ResetSproutCrownShade (ClickEvent evt) {
			if (listenGUIEvents) {
				OnBeforeEdit ();
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorShade = 1;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorShade = 1;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutShadeMode = 
					BranchDescriptorCollection.SproutStyle.SproutShadeMode.Uniform;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutShadeMode = false;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutShadeVariance = 0;
				RefreshStyleCrownValues ();
				OnEdit (true, true, false, false, true);
			}
		}
		void ResetSproutCrownTint (ClickEvent evt) {
			if (listenGUIEvents) {
				OnBeforeEdit ();
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorTint = 0;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorTint = 0;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutTintMode = 
					BranchDescriptorCollection.SproutStyle.SproutTintMode.Uniform;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutTintMode = false;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutTintVariance = 0;
				RefreshStyleCrownValues ();
				OnEdit (true, true, false, false, true);
			}
		}
		void ResetSproutCrownDissolve (ClickEvent evt) {
			if (listenGUIEvents) {
				OnBeforeEdit ();
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorDissolve = 0;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorDissolve = 0;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutDissolveMode = 
					BranchDescriptorCollection.SproutStyle.SproutDissolveMode.Uniform;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutDissolveMode = false;
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutDissolveVariance = 0;
				RefreshStyleCrownValues ();
				OnEdit (true, true, false, false, true);
			}
		}
		private void OnBeforeEdit () {
			sproutLabEditor.onBeforeEditBranchDescriptor?.Invoke (
			sproutLabEditor.selectedSnapshot, sproutLabEditor.branchDescriptorCollection);
		}
		private void OnEdit (
			bool updatePipeline = false, 
			bool updateCompositeMaterials = false, 
			bool updateAlbedoMaterials = false,
			bool updateExtrasMaterials = false,
			bool updateSubsurfaceMaterials = false)
		{
			// If a LOD is selected, return to geometry view.
			if (sproutLabEditor.selectedLODView != 0) {
				sproutLabEditor.ShowPreviewMesh ();
			}
			
			sproutLabEditor.onEditBranchDescriptor?.Invoke (
				sproutLabEditor.selectedSnapshot, sproutLabEditor.branchDescriptorCollection);
			if (updatePipeline) {
				sproutLabEditor.ReflectChangesToPipeline ();
			}
			if (updateCompositeMaterials) {
				UpdateCompositeMaterials ();
			}
			if (updateAlbedoMaterials) {
				UpdateAlbedoMaterials ();
			}
			if (updateExtrasMaterials) {
				UpdateExtrasMaterials ();
			}
			if (updateSubsurfaceMaterials) {
				UpdateSubsurfaceMaterials ();
			}
			sproutLabEditor.sproutSubfactory.sproutCompositeManager.Clear ();
		}
		/// <summary>
		/// Updates the shade of sprout meshes using a sprout style.
		/// </summary>
		/// <param name="styleIndex">Sprout style index (0 = A, 1 = B, 2 = Crown).</param>
		void UpdateShade (int styleIndex) {
			int subMeshIndex;
			int subMeshCount;
			float minShade;
			float maxShade;
			BranchDescriptorCollection.SproutStyle.SproutShadeMode sproutShadeMode;
			bool invertSproutShadeMode;
			float sproutShadeVariance;
			BranchDescriptor snapshot = sproutLabEditor.selectedSnapshot;
			if (styleIndex == 0) {
				subMeshIndex = snapshot.sproutASubmeshIndex;
				subMeshCount = snapshot.sproutASubmeshCount;
				minShade = sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorShade;
				maxShade = sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorShade;
				sproutShadeMode = sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutShadeMode;
				invertSproutShadeMode = sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutShadeMode;
				sproutShadeVariance = sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutShadeVariance;
			} else if (styleIndex == 1) {
				subMeshIndex = snapshot.sproutBSubmeshIndex;
				subMeshCount = snapshot.sproutBSubmeshCount;
				minShade = sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorShade;
				maxShade = sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorShade;
				sproutShadeMode = sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutShadeMode;
				invertSproutShadeMode = sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutShadeMode;
				sproutShadeVariance = sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutShadeVariance;
			} else {
				subMeshIndex = snapshot.sproutCrownSubmeshIndex;
				subMeshCount = snapshot.sproutCrownSubmeshCount;
				minShade = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorShade;
				maxShade = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorShade;
				sproutShadeMode = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutShadeMode;
				invertSproutShadeMode = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutShadeMode;
				sproutShadeVariance = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutShadeVariance;
			}
			for (int i = subMeshIndex; i < subMeshIndex + subMeshCount; i++) {
				Broccoli.Component.SproutMapperComponent.UpdateShadeVariance (
				sproutLabEditor.sproutSubfactory.snapshotTreeMesh,
				minShade, maxShade, sproutShadeMode, invertSproutShadeMode, sproutShadeVariance, i);
			}
		}
		/// <summary>
		/// Updates the dissolve of sprout meshes using a sprout style.
		/// </summary>
		/// <param name="styleIndex">Sprout style index (0 = A, 1 = B, 2 = Crown).</param>
		void UpdateDissolve (int styleIndex) {
			int subMeshIndex;
			int subMeshCount;
			float minDissolve;
			float maxDissolve;
			BranchDescriptorCollection.SproutStyle.SproutDissolveMode sproutDissolveMode;
			bool invertSproutDissolveMode;
			float sproutDissolveVariance;
			BranchDescriptor snapshot = sproutLabEditor.selectedSnapshot;
			if (styleIndex == 0) {
				subMeshIndex = snapshot.sproutASubmeshIndex;
				subMeshCount = snapshot.sproutASubmeshCount;
				minDissolve = sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorDissolve;
				maxDissolve = sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorDissolve;
				sproutDissolveMode = sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutDissolveMode;
				invertSproutDissolveMode = sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutDissolveMode;
				sproutDissolveVariance = sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutDissolveVariance;
			} else if (styleIndex == 1) {
				subMeshIndex = snapshot.sproutBSubmeshIndex;
				subMeshCount = snapshot.sproutBSubmeshCount;
				minDissolve = sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorDissolve;
				maxDissolve = sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorDissolve;
				sproutDissolveMode = sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutDissolveMode;
				invertSproutDissolveMode = sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutDissolveMode;
				sproutDissolveVariance = sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutDissolveVariance;
			} else {
				subMeshIndex = snapshot.sproutCrownSubmeshIndex;
				subMeshCount = snapshot.sproutCrownSubmeshCount;
				minDissolve = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorDissolve;
				maxDissolve = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorDissolve;
				sproutDissolveMode = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutDissolveMode;
				invertSproutDissolveMode = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutDissolveMode;
				sproutDissolveVariance = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutDissolveVariance;
			}
			for (int i = subMeshIndex; i < subMeshIndex + subMeshCount; i++) {
				Broccoli.Component.SproutMapperComponent.UpdateDissolveVariance (
				sproutLabEditor.sproutSubfactory.snapshotTreeMesh,
				minDissolve, maxDissolve, sproutDissolveMode, invertSproutDissolveMode, sproutDissolveVariance, i);
			}
		}
		void UpdateCompositeMaterials () {
			if (sproutLabEditor.currentMapView == SproutLabEditor.VIEW_COMPOSITE) {
				BranchDescriptor snapshot = sproutLabEditor.selectedSnapshot;
				sproutLabEditor.sproutSubfactory.UpdateCompositeMaterials (sproutLabEditor.currentPreviewMaterials,
					sproutLabEditor.branchDescriptorCollection.sproutStyleA,
					sproutLabEditor.branchDescriptorCollection.sproutStyleB,
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown,
					snapshot.sproutASubmeshIndex,
					snapshot.sproutBSubmeshIndex,
					snapshot.sproutCrownSubmeshIndex);
			}
		}
		void UpdateAlbedoMaterials () {
			Material[] mats = null;
			if (sproutLabEditor.currentMapView == SproutLabEditor.VIEW_ALBEDO) {
				mats = sproutLabEditor.currentPreviewMaterials;
			} else if (sproutLabEditor.currentMapView == SproutLabEditor.VIEW_COMPOSITE) {
				mats = sproutLabEditor.meshPreview.secondPassMaterials;
			}
			if (sproutLabEditor.currentMapView == SproutLabEditor.VIEW_ALBEDO || sproutLabEditor.currentMapView == SproutLabEditor.VIEW_COMPOSITE) {
				BranchDescriptor snapshot = sproutLabEditor.selectedSnapshot;
				sproutLabEditor.sproutSubfactory.UpdateAlbedoMaterials (
					mats,
					sproutLabEditor.branchDescriptorCollection.sproutStyleA,
					sproutLabEditor.branchDescriptorCollection.sproutStyleB,
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown,
					sproutLabEditor.branchDescriptorCollection.branchColorShade,
					sproutLabEditor.branchDescriptorCollection.branchColorSaturation,
					snapshot.sproutASubmeshIndex,
					snapshot.sproutBSubmeshIndex,
					snapshot.sproutCrownSubmeshIndex);
			}
		}
		void UpdateExtrasMaterials () {
			if (sproutLabEditor.currentMapView == SproutLabEditor.VIEW_EXTRAS) {
				BranchDescriptor snapshot = sproutLabEditor.selectedSnapshot;
				sproutLabEditor.sproutSubfactory.UpdateExtrasMaterials (
					sproutLabEditor.currentPreviewMaterials, 
					sproutLabEditor.branchDescriptorCollection.sproutStyleA,
					sproutLabEditor.branchDescriptorCollection.sproutStyleB,
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown,
					snapshot.sproutASubmeshIndex,
					snapshot.sproutBSubmeshIndex,
					snapshot.sproutCrownSubmeshIndex);
			}
		}
		void UpdateSubsurfaceMaterials () {
			if (sproutLabEditor.currentMapView == SproutLabEditor.VIEW_SUBSURFACE) {
				BranchDescriptor snapshot = sproutLabEditor.selectedSnapshot;
				sproutLabEditor.sproutSubfactory.UpdateSubsurfaceMaterials (
					sproutLabEditor.currentPreviewMaterials, 
					sproutLabEditor.branchDescriptorCollection.sproutStyleA,
					sproutLabEditor.branchDescriptorCollection.sproutStyleB,
					sproutLabEditor.branchDescriptorCollection.sproutStyleCrown,
					sproutLabEditor.branchDescriptorCollection.branchColorSaturation,
					snapshot.sproutASubmeshIndex,
					snapshot.sproutBSubmeshIndex,
					snapshot.sproutCrownSubmeshIndex);
			}
		}
		public void RefreshValues () {
			if (sproutLabEditor.branchDescriptorCollection != null) {
				listenGUIEvents = false;

				RefreshStyleAValues ();
				RefreshStyleBValues ();
				RefreshStyleCrownValues ();

				listenGUIEvents = true;
			}
		}
		private void RefreshStyleAValues () {
			// SATURATION SPROUT A
			sproutASaturationElem.value = new Vector2 (
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorSaturation,
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorSaturation);
			SproutLabEditor.RefreshMinMaxSlider (sproutASaturationElem, sproutASaturationElem.value);
			sproutASaturationModeElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutSaturationMode;
			if (sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutSaturationMode == BranchDescriptorCollection.SproutStyle.SproutSaturationMode.Uniform) {
				sproutASaturationModeInvertElem.style.display = DisplayStyle.None;
				sproutASaturationVarianceElem.style.display = DisplayStyle.None;
			} else {
				sproutASaturationModeInvertElem.style.display = DisplayStyle.Flex;
				sproutASaturationVarianceElem.style.display = DisplayStyle.Flex;
			}
			sproutASaturationModeInvertElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutSaturationMode;
			sproutASaturationVarianceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutSaturationVariance;
			// TINT SPROUT A
			sproutATintColorElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.colorTint;
			sproutATintElem.value = new Vector2 (
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorTint,
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorTint);
			SproutLabEditor.RefreshMinMaxSlider (sproutATintElem, sproutATintElem.value);
			sproutATintModeElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutTintMode;
			if (sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutTintMode == BranchDescriptorCollection.SproutStyle.SproutTintMode.Uniform) {
				sproutATintModeInvertElem.style.display = DisplayStyle.None;
				sproutATintVarianceElem.style.display = DisplayStyle.None;
			} else {
				sproutATintModeInvertElem.style.display = DisplayStyle.Flex;
				sproutATintVarianceElem.style.display = DisplayStyle.Flex;
			}
			sproutATintModeInvertElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutTintMode;
			sproutATintVarianceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutTintVariance;
			// SHADE A
			sproutAShadeElem.value = new Vector2 (
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorShade,
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorShade);
			SproutLabEditor.RefreshMinMaxSlider (sproutAShadeElem, sproutAShadeElem.value);
			sproutAShadeModeElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutShadeMode;
			if (sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutShadeMode == BranchDescriptorCollection.SproutStyle.SproutShadeMode.Uniform) {
				sproutAShadeModeInvertElem.style.display = DisplayStyle.None;
				sproutAShadeVarianceElem.style.display = DisplayStyle.None;
			} else {
				sproutAShadeModeInvertElem.style.display = DisplayStyle.Flex;
				sproutAShadeVarianceElem.style.display = DisplayStyle.Flex;
			}
			sproutAShadeModeInvertElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutShadeMode;
			sproutAShadeVarianceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutShadeVariance;
			// DISSOLVE
			sproutADissolveElem.value = new Vector2 (
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.minColorDissolve,
				sproutLabEditor.branchDescriptorCollection.sproutStyleA.maxColorDissolve);
			SproutLabEditor.RefreshMinMaxSlider (sproutADissolveElem, sproutADissolveElem.value);
			sproutADissolveModeElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutDissolveMode;
			if (sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutDissolveMode == BranchDescriptorCollection.SproutStyle.SproutDissolveMode.Uniform) {
				sproutADissolveModeInvertElem.style.display = DisplayStyle.None;
				sproutADissolveVarianceElem.style.display = DisplayStyle.None;
			} else {
				sproutADissolveModeInvertElem.style.display = DisplayStyle.Flex;
				sproutADissolveVarianceElem.style.display = DisplayStyle.Flex;
			}
			sproutADissolveModeInvertElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.invertSproutDissolveMode;
			sproutADissolveVarianceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.sproutDissolveVariance;
			// METALLIC, GLOSSINESS, SUBSURFACE SPROUT A
			sproutAMetallicElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.metallic;
			SproutLabEditor.RefreshSlider (sproutAMetallicElem, sproutLabEditor.branchDescriptorCollection.sproutStyleA.metallic);
			sproutAGlossinessElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.glossiness;
			SproutLabEditor.RefreshSlider (sproutAGlossinessElem, sproutLabEditor.branchDescriptorCollection.sproutStyleA.glossiness);
			sproutASubsurfaceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleA.subsurface;
			SproutLabEditor.RefreshSlider (sproutASubsurfaceElem, sproutLabEditor.branchDescriptorCollection.sproutStyleA.subsurface);
		}
		private void RefreshStyleBValues () {
			// SATURATION B
			sproutBSaturationElem.value = new Vector2 (
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorSaturation,
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorSaturation);
			SproutLabEditor.RefreshMinMaxSlider (sproutBSaturationElem, sproutBSaturationElem.value);
			sproutBSaturationModeElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutSaturationMode;
			if (sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutSaturationMode == BranchDescriptorCollection.SproutStyle.SproutSaturationMode.Uniform) {
				sproutBSaturationModeInvertElem.style.display = DisplayStyle.None;
				sproutBSaturationVarianceElem.style.display = DisplayStyle.None;
			} else {
				sproutBSaturationModeInvertElem.style.display = DisplayStyle.Flex;
				sproutBSaturationVarianceElem.style.display = DisplayStyle.Flex;
			}
			sproutBSaturationModeInvertElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutSaturationMode;
			sproutBSaturationVarianceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutSaturationVariance;
			// TINT B
			sproutBTintColorElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.colorTint;
			sproutBTintElem.value = new Vector2 (
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorTint,
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorTint);
			SproutLabEditor.RefreshMinMaxSlider (sproutBTintElem, sproutBTintElem.value);
			sproutBTintModeElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutTintMode;
			if (sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutTintMode == BranchDescriptorCollection.SproutStyle.SproutTintMode.Uniform) {
				sproutBTintModeInvertElem.style.display = DisplayStyle.None;
				sproutBTintVarianceElem.style.display = DisplayStyle.None;
			} else {
				sproutBTintModeInvertElem.style.display = DisplayStyle.Flex;
				sproutBTintVarianceElem.style.display = DisplayStyle.Flex;
			}
			sproutBTintModeInvertElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutTintMode;
			sproutBTintVarianceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutTintVariance;
			// SHADE B
			sproutBShadeElem.value = new Vector2 (
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorShade,
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorShade);
			SproutLabEditor.RefreshMinMaxSlider (sproutBShadeElem, sproutBShadeElem.value);
			sproutBShadeModeElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutShadeMode;
			if (sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutShadeMode == BranchDescriptorCollection.SproutStyle.SproutShadeMode.Uniform) {
				sproutBShadeModeInvertElem.style.display = DisplayStyle.None;
				sproutBShadeVarianceElem.style.display = DisplayStyle.None;
			} else {
				sproutBShadeModeInvertElem.style.display = DisplayStyle.Flex;
				sproutBShadeVarianceElem.style.display = DisplayStyle.Flex;
			}
			sproutBShadeModeInvertElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutShadeMode;
			sproutBShadeVarianceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutShadeVariance;
			// DISSOLVE B
			sproutBDissolveElem.value = new Vector2 (
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.minColorDissolve,
				sproutLabEditor.branchDescriptorCollection.sproutStyleB.maxColorDissolve);
			SproutLabEditor.RefreshMinMaxSlider (sproutBDissolveElem, sproutBDissolveElem.value);
			sproutBDissolveModeElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutDissolveMode;
			if (sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutDissolveMode == BranchDescriptorCollection.SproutStyle.SproutDissolveMode.Uniform) {
				sproutBDissolveModeInvertElem.style.display = DisplayStyle.None;
				sproutBDissolveVarianceElem.style.display = DisplayStyle.None;
			} else {
				sproutBDissolveModeInvertElem.style.display = DisplayStyle.Flex;
				sproutBDissolveVarianceElem.style.display = DisplayStyle.Flex;
			}
			sproutBDissolveModeInvertElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.invertSproutDissolveMode;
			sproutBDissolveVarianceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.sproutDissolveVariance;
			// METALLIC, GLOSSINESS, SUBSURFACE SPROUT A
			sproutBMetallicElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.metallic;
			SproutLabEditor.RefreshSlider (sproutBMetallicElem, sproutLabEditor.branchDescriptorCollection.sproutStyleB.metallic);
			sproutBGlossinessElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.glossiness;
			SproutLabEditor.RefreshSlider (sproutBGlossinessElem, sproutLabEditor.branchDescriptorCollection.sproutStyleB.glossiness);
			sproutBSubsurfaceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleB.subsurface;
			SproutLabEditor.RefreshSlider (sproutBSubsurfaceElem, sproutLabEditor.branchDescriptorCollection.sproutStyleB.subsurface);
		}
		private void RefreshStyleCrownValues () {
			// SATURATION SPROUT CROWN
			sproutCrownSaturationElem.value = new Vector2 (
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorSaturation,
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorSaturation);
			SproutLabEditor.RefreshMinMaxSlider (sproutCrownSaturationElem, sproutCrownSaturationElem.value);
			sproutCrownSaturationModeElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutSaturationMode;
			if (sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutSaturationMode == BranchDescriptorCollection.SproutStyle.SproutSaturationMode.Uniform) {
				sproutCrownSaturationModeInvertElem.style.display = DisplayStyle.None;
				sproutCrownSaturationVarianceElem.style.display = DisplayStyle.None;
			} else {
				sproutCrownSaturationModeInvertElem.style.display = DisplayStyle.Flex;
				sproutCrownSaturationVarianceElem.style.display = DisplayStyle.Flex;
			}
			sproutCrownSaturationModeInvertElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutSaturationMode;
			sproutCrownSaturationVarianceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutSaturationVariance;
			// TINT SPROUT A
			sproutCrownTintColorElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.colorTint;
			sproutCrownTintElem.value = new Vector2 (
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorTint,
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorTint);
			SproutLabEditor.RefreshMinMaxSlider (sproutCrownTintElem, sproutCrownTintElem.value);
			sproutCrownTintModeElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutTintMode;
			if (sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutTintMode == BranchDescriptorCollection.SproutStyle.SproutTintMode.Uniform) {
				sproutCrownTintModeInvertElem.style.display = DisplayStyle.None;
				sproutCrownTintVarianceElem.style.display = DisplayStyle.None;
			} else {
				sproutCrownTintModeInvertElem.style.display = DisplayStyle.Flex;
				sproutCrownTintVarianceElem.style.display = DisplayStyle.Flex;
			}
			sproutCrownTintModeInvertElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutTintMode;
			sproutCrownTintVarianceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutTintVariance;
			// SHADE A
			sproutCrownShadeElem.value = new Vector2 (
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorShade,
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorShade);
			SproutLabEditor.RefreshMinMaxSlider (sproutCrownShadeElem, sproutCrownShadeElem.value);
			sproutCrownShadeModeElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutShadeMode;
			if (sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutShadeMode == BranchDescriptorCollection.SproutStyle.SproutShadeMode.Uniform) {
				sproutCrownShadeModeInvertElem.style.display = DisplayStyle.None;
				sproutCrownShadeVarianceElem.style.display = DisplayStyle.None;
			} else {
				sproutCrownShadeModeInvertElem.style.display = DisplayStyle.Flex;
				sproutCrownShadeVarianceElem.style.display = DisplayStyle.Flex; 
			}
			sproutCrownShadeModeInvertElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutShadeMode;
			sproutCrownShadeVarianceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutShadeVariance;
			// DISSOLVE
			sproutCrownDissolveElem.value = new Vector2 (
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.minColorDissolve,
				sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.maxColorDissolve);
			SproutLabEditor.RefreshMinMaxSlider (sproutCrownDissolveElem, sproutCrownDissolveElem.value);
			sproutCrownDissolveModeElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutDissolveMode;
			if (sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutDissolveMode == BranchDescriptorCollection.SproutStyle.SproutDissolveMode.Uniform) {
				sproutCrownDissolveModeInvertElem.style.display = DisplayStyle.None;
				sproutCrownDissolveVarianceElem.style.display = DisplayStyle.None;
			} else {
				sproutCrownDissolveModeInvertElem.style.display = DisplayStyle.Flex;
				sproutCrownDissolveVarianceElem.style.display = DisplayStyle.Flex;
			}
			sproutCrownDissolveModeInvertElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.invertSproutDissolveMode;
			sproutCrownDissolveVarianceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.sproutDissolveVariance;
			// METALLIC, GLOSSINESS, SUBSURFACE SPROUT A
			sproutCrownMetallicElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.metallic;
			SproutLabEditor.RefreshSlider (sproutCrownMetallicElem, sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.metallic);
			sproutCrownGlossinessElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.glossiness;
			SproutLabEditor.RefreshSlider (sproutCrownGlossinessElem, sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.glossiness);
			sproutCrownSubsurfaceElem.value = sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.subsurface;
			SproutLabEditor.RefreshSlider (sproutCrownSubsurfaceElem, sproutLabEditor.branchDescriptorCollection.sproutStyleCrown.subsurface);
		}
		public void Attach () {
			if (!this.sproutLabEditor.rootVisualElement.Contains (container)) {
				this.sproutLabEditor.rootVisualElement.Add (container);
			}
		}
		public void Detach () {
			if (this.sproutLabEditor.rootVisualElement.Contains (container)) {
				this.sproutLabEditor.rootVisualElement.Remove (container);
			}
		}
		/// <summary>
        /// Called when the GUI textures are loaded.
        /// </summary>
        public void OnGUITexturesLoaded () {
			if (sproutAContainerElem != null)
				SetContainerIcons (sproutAContainerElem);
			if (sproutBContainerElem != null)
				SetContainerIcons (sproutBContainerElem);
			if (sproutCrownContainerElem != null)
				SetContainerIcons (sproutCrownContainerElem);
		}
		void SetContainerIcons (VisualElement container) {
			VisualElement iconSaturationElem = container.Q<VisualElement> (iconSaturationName);
			if (iconSaturationElem != null ) 
				iconSaturationElem.style.backgroundImage = new StyleBackground (GUITextureManager.IconSaturation);
			VisualElement iconShadeElem = container.Q<VisualElement> (iconShadeName);
			if (iconShadeElem != null ) 
				iconShadeElem.style.backgroundImage = new StyleBackground (GUITextureManager.IconShade);
			VisualElement iconTintElem = container.Q<VisualElement> (iconTintName);
			if (iconTintElem != null ) 
				iconTintElem.style.backgroundImage = new StyleBackground (GUITextureManager.IconTint);
			VisualElement iconSurfaceElem = container.Q<VisualElement> (iconSurfaceName);
			if (iconSurfaceElem != null ) 
				iconSurfaceElem.style.backgroundImage = new StyleBackground (GUITextureManager.IconSurface);
			if (Broccoli.Base.GlobalSettings.experimentalSproutLabDissolveSprouts) {
				VisualElement iconDissolveElem = container.Q<VisualElement> (iconDissolveName);
				if (iconDissolveElem != null ) 
					iconDissolveElem.style.backgroundImage = new StyleBackground (GUITextureManager.IconDissolve);
			}
		}
		#endregion

		#region Side Panel 
		public void Repaint () {
			RefreshValues ();
			requiresRepaint = false;
		}
		public void OnUndoRedo () {
			RefreshValues ();
			//LoadSidePanelFields (selectedVariationGroup);
		}
        #endregion

        #region Draw
        public void SetVisible (bool visible) {
            if (visible) {
                container.style.display = DisplayStyle.Flex;
            } else {
                container.style.display = DisplayStyle.None;
            }
        }
        /// <summary>
        /// Sets the draw area for the components.
        /// </summary>
        /// <param name="refRect">Rect to draw the componentes.</param>
        public void SetRect (Rect refRect) {
            if (Event.current.type != EventType.Repaint) return;
            rect = refRect;
            container.style.marginTop = refRect.y;
            container.style.height = refRect.height;
        }
        #endregion
    }
}
