// Copyright Elliot Bentine, 2018-
using UnityEngine;
using UnityEngine.Rendering;
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
        [Range(0.01f, 1.0f)]
        public float DefaultLowResScale = 1.0f;

        [Tooltip("Whether to perform ProPixelizer's 'Pixel Expansion' method of pixelation. This is required for per-object pixelisation and for moving pixelated objects at apparent sub-pixel resolutions.")]
        public bool UsePixelExpansion = true;

        [Tooltip("Controls which objects in the scene should be pixelated.")]
        public PixelizationFilter PixelizationFilter = PixelizationFilter.FullScene;

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
        public bool UsePixelArtUpscalingFilter = true;
        #endregion

        ProPixelizerApplyPixelizationMapPass _PixelisationPass;
        ProPixelizerPixelizationMapPass _PixelizationMapPass;
        ProPixelizerMetadataPass _MetadataPass;
        ProPixelizerOutlineDetectionPass _OutlineDetectionPass;
        ProPixelizerLowResolutionDrawPass _LowResolutionDrawPass;
        ProPixelizerLowResRecompositionPass _LowResRecompositionPass;
        ProPixelizerFinalRecompositionPass _FinalRecompositionPass;
        ProPixelizerLowResolutionConfigurationPass _LowResConfigurationPass;
        ProPixelizerLowResolutionTargetPass _LowResTargetPass;
        ProPixelizerOpaqueDrawRedirectionPass _OpaqueDrawRedirectionPass;
#if UNITY_2023_3_OR_NEWER
        ProPixelizerRenderGraphRedirectPass _GraphRedirectsPass;
        ProPixelizerRenderGraphUnRedirectPass _GraphUnredirectsPass;
#endif
        ProPixelizerFullscreenColorGradingPass _FullscreenColorGradingPass;
        ProPixelizerCopySubcameraDepthPass _CopySubcameraDepthPass;
        ProPixelizerHideShadersPass _HideShadersPass;

        public override void Create()
        {
            LoadShaderResources();

            _LowResConfigurationPass = new ProPixelizerLowResolutionConfigurationPass();
            _LowResTargetPass = new ProPixelizerLowResolutionTargetPass(_LowResConfigurationPass);
            _LowResolutionDrawPass = new ProPixelizerLowResolutionDrawPass(_LowResConfigurationPass, _LowResTargetPass);
            _MetadataPass = new ProPixelizerMetadataPass(_LowResConfigurationPass);
            _OutlineDetectionPass = new ProPixelizerOutlineDetectionPass(OutlineShaders, _LowResConfigurationPass, _MetadataPass);
            _OpaqueDrawRedirectionPass = new ProPixelizerOpaqueDrawRedirectionPass(_LowResConfigurationPass, _LowResTargetPass);
            _PixelizationMapPass = new ProPixelizerPixelizationMapPass(PixelizationMapShaders,
                _MetadataPass,
                _LowResConfigurationPass,
                _LowResTargetPass
                );
            _PixelisationPass = new ProPixelizerApplyPixelizationMapPass(
                PixelizationShaders,
                _MetadataPass,
                _LowResConfigurationPass,
                _LowResTargetPass,
                _PixelizationMapPass
                );
            
            _LowResRecompositionPass = new ProPixelizerLowResRecompositionPass(DepthRecompositionShaders, _LowResTargetPass);
            _FinalRecompositionPass = new ProPixelizerFinalRecompositionPass(FinalRecompositionPassShaders, _LowResRecompositionPass);
            _FullscreenColorGradingPass = new ProPixelizerFullscreenColorGradingPass(FullscreenColorGradingPassShaders, _MetadataPass);
#if UNITY_2023_3_OR_NEWER
            _GraphRedirectsPass = new ProPixelizerRenderGraphRedirectPass();
            _GraphUnredirectsPass = new ProPixelizerRenderGraphUnRedirectPass(RenderGraphUnredirectPassShaders);
#endif
            _CopySubcameraDepthPass = new ProPixelizerCopySubcameraDepthPass(CopySubcameraDepthPassShaders);
            _HideShadersPass = new ProPixelizerHideShadersPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // NB: We don't want Depth input - we actually want to modify CameraDepthAttachment, which we can do at any time,
            // and then have the CopyDepth into CameraDepthTarget after we are done.
            //_LowResRecompositionPass.ConfigureInput(ScriptableRenderPassInput.Color);
            //_FinalRecompositionPass.ConfigureInput(ScriptableRenderPassInput.Color);

#if UNITY_2023_3_OR_NEWER
            _GraphRedirectsPass._Renderer = renderer as UniversalRenderer;
#endif

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
                RenderingPathway = ProPixelizerRenderingPathway.MainWithVirtualCamera,
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
            _PixelizationMapPass.ConfigureForSettings(settings);
            _PixelisationPass.ConfigureForSettings(settings);
            _LowResRecompositionPass.ConfigureForSettings(settings);
            _FullscreenColorGradingPass.ConfigureForSettings(settings);
            _FinalRecompositionPass.ConfigureForSettings(settings);
            _CopySubcameraDepthPass.ConfigureForSettings(settings);
            _HideShadersPass.ConfigureForSettings(settings);

#if UNITY_2023_OR_NEWER
            _GraphRedirectsPass.ConfigureForSettings(settings);
            _GraphUnredirectsPass.ConfigureForSettings(settings);
#endif            

            var recompositionEvent = settings.Filter == PixelizationFilter.FullScene
                ? RenderPassEvent.AfterRenderingTransparents 
                : RenderPassEvent.AfterRenderingOpaques;
            if (settings.RenderingPathway == ProPixelizerRenderingPathway.MainCamera)
                recompositionEvent = RenderPassEvent.BeforeRenderingOpaques;
            _LowResRecompositionPass.renderPassEvent = recompositionEvent;
            _FinalRecompositionPass.renderPassEvent = recompositionEvent;

            // if we are a propixelizer camera with a defined subcamera, go straight to recomposition.
            var onlyRecomposite = proPixelizerCamera != null && proPixelizerCamera.enabled && proPixelizerCamera.SubCamera != null && proPixelizerCamera.SubCamera.enabled;

            renderer.EnqueuePass(_LowResConfigurationPass);
            renderer.EnqueuePass(_LowResTargetPass);
            if (!settings.Enabled) return; // we always run the first two passes, so that global float keywords can be set.

            if (
                settings.RenderingPathway == ProPixelizerRenderingPathway.SubCamera ||
                settings.RenderingPathway == ProPixelizerRenderingPathway.MainWithVirtualCamera
                )
            {
                renderer.EnqueuePass(_MetadataPass);
                renderer.EnqueuePass(_OutlineDetectionPass);

                // ...opaque draw happens between these...

                if (settings.Filter != PixelizationFilter.FullScene)
                    renderer.EnqueuePass(_LowResolutionDrawPass);
                if (settings.UsePixelExpansion)
                {
                    renderer.EnqueuePass(_PixelizationMapPass);
                    renderer.EnqueuePass(_PixelisationPass);
                }

                renderer.EnqueuePass(_FullscreenColorGradingPass);
                _FullscreenColorGradingPass.ConfigureInput(ScriptableRenderPassInput.Color);
            }

            // Redirection only for virtual camera setup
            if (settings.RenderingPathway == ProPixelizerRenderingPathway.MainWithVirtualCamera)
            {
#if UNITY_2023_3_OR_NEWER
                if (GraphicsSettings.GetRenderPipelineSettings<RenderGraphSettings>().enableRenderCompatibilityMode)
#endif
                renderer.EnqueuePass(_OpaqueDrawRedirectionPass); // non-RG only
#if UNITY_2023_3_OR_NEWER
                else
                {
                    renderer.EnqueuePass(_GraphRedirectsPass); // RG only
                    renderer.EnqueuePass(_GraphUnredirectsPass); // RG only
                }
#endif
            }

            // renderer.EnqueuePass(_HideShadersPass);

            // Recomposition stage
            if (
                settings.RenderingPathway == ProPixelizerRenderingPathway.MainWithVirtualCamera ||
                settings.RenderingPathway == ProPixelizerRenderingPathway.MainCamera
                )
            {
                renderer.EnqueuePass(_LowResRecompositionPass);
                renderer.EnqueuePass(_FinalRecompositionPass);
            }

            if (settings.RenderingPathway == ProPixelizerRenderingPathway.SubCamera)
                renderer.EnqueuePass(_CopySubcameraDepthPass);

            if (GenerateWarnings)
                ProPixelizerVerification.GenerateWarnings();

            //Debug.Log(string.Format("Camera: {0}, {1}", renderingData.cameraData.cameraType, settings.RenderingPathway));
        }

        public void ModifySettings(ref ProPixelizerSettings settings, ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.cameraType == CameraType.SceneView && !EditorFullscreenColorGrading)
                settings.FullscreenLUT = null;

            if (renderingData.cameraData.camera.cameraType == CameraType.SceneView && !EditorPixelExpansion)
                settings.UsePixelExpansion = false;

            var propCamera = renderingData.cameraData.camera.GetComponent<ProPixelizerCamera>();
            var subCamera = renderingData.cameraData.camera.GetComponent<ProPixelizerSubCamera>();
            if (propCamera != null && propCamera.enabled)
            {
                if (propCamera.SubCamera != null && propCamera.SubCamera.enabled)
                    settings.RenderingPathway = ProPixelizerRenderingPathway.MainCamera;
            }
            else if (subCamera != null && subCamera.enabled)
                settings.RenderingPathway = ProPixelizerRenderingPathway.SubCamera;
            else
                settings.RenderingPathway = ProPixelizerRenderingPathway.MainWithVirtualCamera;

            if (
                renderingData.cameraData.camera.cameraType == CameraType.Preview
#if UNITY_2023_3_OR_NEWER
                && GraphicsSettings.GetRenderPipelineSettings<RenderGraphSettings>().enableRenderCompatibilityMode
#endif
                )
            {
                settings.UsePixelExpansion = false;
                settings.RenderingPathway = ProPixelizerRenderingPathway.MainWithVirtualCamera;
            }

            if (renderingData.cameraData.camera.GetUniversalAdditionalCameraData().renderType == CameraRenderType.Overlay) {
                if (settings.Filter == PixelizationFilter.FullScene)
                {
                    settings.Filter = PixelizationFilter.OnlyProPixelizer;
                }
            }

#if UNITY_2023_3_OR_NEWER
            // trying to get the fullscreen pass working with the backbuffer was a nightmare.
            // we don't need fullscreen mode in preview windows anyway, and layer setup isn't used either.
            if (renderingData.cameraData.camera.cameraType == CameraType.Preview
                && !GraphicsSettings.GetRenderPipelineSettings<RenderGraphSettings>().enableRenderCompatibilityMode)
                settings.Filter = PixelizationFilter.OnlyProPixelizer;
#endif

            if (!settings.Enabled)
                settings.UsePixelExpansion = false;
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            // Note: Preview camera does not call SetupRenderPasses (bug) - https://forum.unity.com/threads/preview-camera-does-not-call-setuprenderpasses.1377363/
            _PixelisationPass.ConfigureInput(ScriptableRenderPassInput.Color);
            _OpaqueDrawRedirectionPass.FindDrawObjectPasses(renderer as UniversalRenderer, in renderingData);
        }
        protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {                
                _LowResTargetPass.Dispose();
                _MetadataPass.Dispose();
                _OutlineDetectionPass.Dispose();
                _LowResolutionDrawPass.Dispose();
                _PixelizationMapPass.Dispose();
                _PixelisationPass.Dispose();
                _LowResRecompositionPass.Dispose();
                _FullscreenColorGradingPass.Dispose();
                _FinalRecompositionPass.Dispose();
            }
    }

        #region Serialized Shader Fields
        // We serialize shader fields to prevent them from being stripped from builds.
        [HideInInspector, SerializeField]
        ProPixelizerPixelizationMapPass.ShaderResources PixelizationMapShaders;
        [HideInInspector, SerializeField]
        ProPixelizerApplyPixelizationMapPass.ShaderResources PixelizationShaders;
        [HideInInspector, SerializeField]
        ProPixelizerOutlineDetectionPass.ShaderResources OutlineShaders;
        [HideInInspector, SerializeField]
        ProPixelizerLowResRecompositionPass.ShaderResources DepthRecompositionShaders;
        [HideInInspector, SerializeField]
        ProPixelizerFinalRecompositionPass.ShaderResources FinalRecompositionPassShaders;
        [HideInInspector, SerializeField]
        ProPixelizerFullscreenColorGradingPass.ShaderResources FullscreenColorGradingPassShaders;
#if UNITY_2023_3_OR_NEWER
        [HideInInspector, SerializeField]
        ProPixelizerRenderGraphUnRedirectPass.ShaderResources RenderGraphUnredirectPassShaders;
#endif
        [HideInInspector, SerializeField]
        ProPixelizerCopySubcameraDepthPass.ShaderResources CopySubcameraDepthPassShaders;

        private void LoadShaderResources()
        {
            PixelizationMapShaders = new ProPixelizerPixelizationMapPass.ShaderResources().Load();
            PixelizationShaders = new ProPixelizerApplyPixelizationMapPass.ShaderResources().Load();
            OutlineShaders = new ProPixelizerOutlineDetectionPass.ShaderResources().Load();
            DepthRecompositionShaders = new ProPixelizerLowResRecompositionPass.ShaderResources().Load();
            FinalRecompositionPassShaders = new ProPixelizerFinalRecompositionPass.ShaderResources().Load();
            FullscreenColorGradingPassShaders = new ProPixelizerFullscreenColorGradingPass.ShaderResources().Load();
#if UNITY_2023_3_OR_NEWER
            RenderGraphUnredirectPassShaders = new ProPixelizerRenderGraphUnRedirectPass.ShaderResources().Load();
#endif
            CopySubcameraDepthPassShaders = new ProPixelizerCopySubcameraDepthPass.ShaderResources().Load();
        }
#endregion
    }
};