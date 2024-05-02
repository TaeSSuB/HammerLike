﻿// Copyright Elliot Bentine, 2018-
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace ProPixelizer
{

    /// <summary>
    /// The ProPixelizerRenderFeature adds all the passes required for ProPixelizer to your Scriptable render pipeline.
    /// 
    /// The generalised pass structure is as follows (--> Indicates render events).
    /// 
    /// Configure:
    ///   * Low-res targets configured and buffers allocated.
    ///   * ProPixelizerMetadata buffer configured - same target size as low-res target (pixelisation target).
    /// 
    /// --> BeforeRenderingOpaques
    ///   * ProPixelizerMetadata buffer rendered.
    ///   * Outline detection pass performed.
    /// * Render Opaques
    /// --> AfterRenderingOpaques
    ///   * Pixelated objects rendered to the low-res target.
    ///   * ProPixelizer dithered pixelisation applied to low-res target.
    ///   * Low-res target merged back into scene buffer, blended according to depth.
    /// * Onward as usual (render transparents, etc).
    /// 
    /// For 'Entire Screen'
    /// 
    /// --> Configure:
    ///   * Low-res targets configured and buffers allocated.
    ///   * ProPixelizerMetadata buffer configured - same target size as low-res target.
    /// 
    /// --> BeforeRenderingOpaques
    ///   * Change target to low-res target (or, perhaps easier, change size of main target).
    ///   * Outline detection pass performed
    /// --> Render Opaques
    /// --> AfterRenderingOpaqures
    ///   * ProPixelizer dithered pixelisation applied to low-res target.
    ///   * Blend low-res target into scene buffer.
    /// 
    /// </summary>
    public class ProPixelizerRenderFeature : ScriptableRendererFeature
    {
        #region User Properties
        [FormerlySerializedAs("DepthTestOutlines")]
        [Tooltip("Perform depth testing for outlines where object IDs differ. This prevents outlines appearing when one object intersects another, but requires an extra depth sample.")]
        public bool UseDepthTestingForIDOutlines = true;

        [Tooltip("The threshold value used when depth comparing outlines.")]
        public float DepthTestThreshold = 0.0001f;

        [Tooltip("Use normals for edge detection. This will analyse pixelated screen normals to determine where edges occur within an objects silhouette.")]
        public bool UseNormalsForEdgeDetection = true;

        [Tooltip("Threshold value when using normals for detecting edges.")]
        public float NormalEdgeDetectionThreshold = 2f;

        [Tooltip("Perform additional depth testing to confirm edges due to normals.")]
        public bool UseDepthTestingForEdgeOutlines = true;

        [Tooltip("Default scale of the low-resolution render target, used in Scene tab. Game cameras may override this, e.g. to set world space pixel size.")]
        public float DefaultLowResScale = 1.0f;

        [Tooltip("Whether to perform ProPixelizer's 'Pixel Expansion' method of pixelation. This is required for per-object pixelisation and for moving pixelated objects at apparent sub-pixel resolutions.")]
        public bool UsePixelExpansion = true;

        [Tooltip("Controls which objects in the scene should be pixelated.")]
        public PixelizationFilter PixelizationFilter = PixelizationFilter.OnlyProPixelizer;

        [Tooltip("Selection of layers to draw pixelated.")]
        public LayerMask PixelizedLayers;

        [Tooltip("Color palette for use in full screen color grading.")]
        public Texture2D FullscreenColorPalette = null;

        [Tooltip("Enable color grading in the editor scene tab?")]
        public bool EditorFullscreenColorGrading = false;

        [Tooltip("Use pixel expansion in the editor scene tab?")]
        public bool EditorPixelExpansion = true;

        [Tooltip("Generates warnings if the pipeline state is incompatible with ProPixelizer.")]
        public bool GenerateWarnings = true;

        [Tooltip("Enable use a pixel art upscaling filter that reduces aliasing.")]
        public bool UsePixelArtUpscalingFilter = false;
        #endregion

        ProPixelizerPixelizationPass _PixelisationPass;
        ProPixelizerMetadataPass _MetadataPass;
        ProPixelizerOutlineDetectionPass _OutlineDetectionPass;
        ProPixelizerLowResolutionDrawPass _LowResolutionDrawPass;
        ProPixelizerLowResRecompositionPass _LowResRecompositionPass;
        ProPixelizerFinalRecompositionPass _FinalRecompositionPass;
        ProPixelizerLowResolutionConfigurationPass _LowResConfigurationPass;
        ProPixelizerLowResolutionTargetPass _LowResTargetPass;
        ProPixelizerOpaqueDrawRedirectionPass _OpaqueDrawRedirectionPass;
        ProPixelizerFullscreenColorGradingPass _FullscreenColorGradingPass;

        public override void Create()
        {
            LoadShaderResources();

            _LowResConfigurationPass = new ProPixelizerLowResolutionConfigurationPass();
            _LowResTargetPass = new ProPixelizerLowResolutionTargetPass(_LowResConfigurationPass);
            _LowResolutionDrawPass = new ProPixelizerLowResolutionDrawPass(_LowResConfigurationPass, _LowResTargetPass);
            _MetadataPass = new ProPixelizerMetadataPass(_LowResConfigurationPass);
            _OutlineDetectionPass = new ProPixelizerOutlineDetectionPass(OutlineShaders, _LowResConfigurationPass, _MetadataPass);
            _OpaqueDrawRedirectionPass = new ProPixelizerOpaqueDrawRedirectionPass(_LowResConfigurationPass, _LowResTargetPass);
            _PixelisationPass = new ProPixelizerPixelizationPass(
                PixelizationShaders,
                _MetadataPass,
                _LowResolutionDrawPass,
                _LowResConfigurationPass,
                _OutlineDetectionPass,
                _LowResTargetPass);
            _LowResRecompositionPass = new ProPixelizerLowResRecompositionPass(DepthRecompositionShaders, _LowResTargetPass);
            _FinalRecompositionPass = new ProPixelizerFinalRecompositionPass(FinalRecompositionPassShaders, _LowResRecompositionPass);
            _FullscreenColorGradingPass = new ProPixelizerFullscreenColorGradingPass(FullscreenColorGradingPassShaders, _MetadataPass);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // NB: We don't want Depth input - we actually want to modify CameraDepthAttachment, which we can do at any time,
            // and then have the CopyDepth into CameraDepthTarget after we are done.
            //_LowResRecompositionPass.ConfigureInput(ScriptableRenderPassInput.Color);
            //_FinalRecompositionPass.ConfigureInput(ScriptableRenderPassInput.Color);

            var settings = new ProPixelizerSettings
            {
                UseDepthTestingForEdgeOutlines = UseDepthTestingForEdgeOutlines,
                UseDepthTestingForIDOutlines = UseDepthTestingForIDOutlines,
                DepthTestThreshold = DepthTestThreshold,

                UseNormalsForEdgeDetection = UseNormalsForEdgeDetection,
                NormalEdgeDetectionThreshold = NormalEdgeDetectionThreshold,

                Filter = PixelizationFilter,
                PixelizedLayers = PixelizedLayers,
                UsePixelExpansion = UsePixelExpansion,

                DefaultLowResScale = DefaultLowResScale,
                FullscreenLUT = FullscreenColorPalette,
                UsePixelArtUpscalingFilter = UsePixelArtUpscalingFilter,

                Enabled = true,
            };

            // Camera ProPixelizer overrides.
            var proPixelizerCamera = renderingData.cameraData.camera.GetComponent<ProPixelizerCamera>();
            if (proPixelizerCamera != null)
                proPixelizerCamera.ModifySettings(ref settings);

            ModifySettings(ref settings, renderer, ref renderingData);

            

            // Configure passes
            _LowResConfigurationPass.ConfigureForSettings(settings);
            _LowResTargetPass.ConfigureForSettings(settings);
            _MetadataPass.ConfigureForSettings(settings);
            _OutlineDetectionPass.ConfigureForSettings(settings);
            _OpaqueDrawRedirectionPass.ConfigureForSettings(settings);
            _LowResolutionDrawPass.ConfigureForSettings(settings);
            _PixelisationPass.ConfigureForSettings(settings);
            _LowResRecompositionPass.ConfigureForSettings(settings);
            _FullscreenColorGradingPass.ConfigureForSettings(settings);
            _FinalRecompositionPass.ConfigureForSettings(settings);

            var recompositionEvent = settings.Filter == PixelizationFilter.FullScene
                ? RenderPassEvent.AfterRenderingTransparents 
                : RenderPassEvent.AfterRenderingOpaques;
            _LowResRecompositionPass.renderPassEvent = recompositionEvent;
            _FinalRecompositionPass.renderPassEvent = recompositionEvent;

            renderer.EnqueuePass(_LowResConfigurationPass);
            renderer.EnqueuePass(_LowResTargetPass);
            if (!settings.Enabled) return; // we always run the first two passes, so that global float keywords can be set.
            renderer.EnqueuePass(_MetadataPass);
            renderer.EnqueuePass(_OutlineDetectionPass);
            renderer.EnqueuePass(_OpaqueDrawRedirectionPass);
            if (settings.Filter != PixelizationFilter.FullScene)
                renderer.EnqueuePass(_LowResolutionDrawPass);
            if (settings.UsePixelExpansion)
                renderer.EnqueuePass(_PixelisationPass);
            renderer.EnqueuePass(_LowResRecompositionPass);
            renderer.EnqueuePass(_FinalRecompositionPass);
            renderer.EnqueuePass(_FullscreenColorGradingPass);
            _FullscreenColorGradingPass.ConfigureInput(ScriptableRenderPassInput.Color);

            if (GenerateWarnings)
                ProPixelizerVerification.GenerateWarnings();
            
        }

        public void ModifySettings(ref ProPixelizerSettings settings, ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.cameraType == CameraType.SceneView && !EditorFullscreenColorGrading)
                settings.FullscreenLUT = null;

            if (renderingData.cameraData.camera.cameraType == CameraType.SceneView && !EditorPixelExpansion)
                settings.UsePixelExpansion = false;

            if (renderingData.cameraData.camera.cameraType == CameraType.Preview)
                settings.UsePixelExpansion = false;

            if (!settings.Enabled)
                settings.UsePixelExpansion = false;
        }

#if BLIT_API
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            // Note: Preview camera does not call SetupRenderPasses (bug) - https://forum.unity.com/threads/preview-camera-does-not-call-setuprenderpasses.1377363/
            _PixelisationPass.ConfigureInput(ScriptableRenderPassInput.Color);
#if URP_SETUPRENDERPASSES
            _OpaqueDrawRedirectionPass.FindDrawObjectPasses(renderer as UniversalRenderer, in renderingData);
#endif
        }
#endif

#if URP_13
        protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {                
                _PixelisationPass.Dispose();
                _MetadataPass.Dispose();
                _OutlineDetectionPass.Dispose();
                _LowResTargetPass.Dispose();
                _LowResRecompositionPass.Dispose();
                _FullscreenColorGradingPass.Dispose();
                _FinalRecompositionPass.Dispose();
        }
    }
#endif

        #region Serialized Shader Fields
        // We serialize shader fields to prevent them from being stripped from builds.

        [HideInInspector, SerializeField]
        ProPixelizerPixelizationPass.ShaderResources PixelizationShaders;
        [HideInInspector, SerializeField]
        ProPixelizerOutlineDetectionPass.ShaderResources OutlineShaders;
        [HideInInspector, SerializeField]
        ProPixelizerLowResRecompositionPass.ShaderResources DepthRecompositionShaders;
        [HideInInspector, SerializeField]
        ProPixelizerFinalRecompositionPass.ShaderResources FinalRecompositionPassShaders;
        [HideInInspector, SerializeField]
        ProPixelizerFullscreenColorGradingPass.ShaderResources FullscreenColorGradingPassShaders;

        private void LoadShaderResources()
        {
            PixelizationShaders = new ProPixelizerPixelizationPass.ShaderResources().Load();
            OutlineShaders = new ProPixelizerOutlineDetectionPass.ShaderResources().Load();
            DepthRecompositionShaders = new ProPixelizerLowResRecompositionPass.ShaderResources().Load();
            FinalRecompositionPassShaders = new ProPixelizerFinalRecompositionPass.ShaderResources().Load();
            FullscreenColorGradingPassShaders = new ProPixelizerFullscreenColorGradingPass.ShaderResources().Load();
        }
        #endregion
    }
}