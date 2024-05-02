// Copyright Elliot Bentine, 2018-
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace ProPixelizer
{
    /// <summary>
    /// Reconfigures draw target and projection matrices to force opaques to be rendered to the low-res target.
    /// </summary>
    public class ProPixelizerOpaqueDrawRedirectionPass : ProPixelizerPass
    {
        public ProPixelizerOpaqueDrawRedirectionPass(ProPixelizerLowResolutionConfigurationPass configPass, ProPixelizerLowResolutionTargetPass targetPass)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            ConfigurationPass = configPass;
            TargetPass = targetPass;
        }

        ProPixelizerLowResolutionTargetPass TargetPass;
        ProPixelizerLowResolutionConfigurationPass ConfigurationPass;

        bool _currentlyDrawingPreviewCamera;
        private bool _storedOriginalColorTarget; // non-nullable types
#if URP_RTHANDLE
        RTHandle _originalOpaqueColorTarget;
        RTHandle _originalOpaqueDepthTarget;
#else
        RenderTargetIdentifier _originalOpaqueColorTarget;
        RenderTargetIdentifier _originalOpaqueDepthTarget;
#endif

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
#if !URP_SETUPRENDERPASSES // Never simultaneously true with URP_UNIVERSALRENDERER
            FindDrawObjectPasses(renderingData.cameraData.renderer as ForwardRenderer, in renderingData);
#else
            // workaround for stupid bug: https://forum.unity.com/threads/preview-camera-does-not-call-setuprenderpasses.1377363/
            if (renderingData.cameraData.cameraType == CameraType.Preview)
                FindDrawObjectPasses(renderingData.cameraData.renderer as UniversalRenderer, renderingData.cameraData);
#endif
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (Settings.Filter == PixelizationFilter.FullScene && !_currentlyDrawingPreviewCamera)
            {
                if (OpaqueDrawPass != null && TransparentDrawPass != null)
                {
#if URP_RTHANDLE
                    _originalOpaqueColorTarget = OpaqueDrawPass.colorAttachmentHandle;
                    _originalOpaqueDepthTarget = OpaqueDrawPass.depthAttachmentHandle;
                    var currentTransparentColor = TransparentDrawPass.colorAttachmentHandle;
#else
                    _originalOpaqueColorTarget = OpaqueDrawPass.colorAttachment;
                    _originalOpaqueDepthTarget = OpaqueDrawPass.depthAttachment;
                    _storedOriginalColorTarget = true;
                    var currentTransparentColor = TransparentDrawPass.colorAttachment;
#endif

                    if (_originalOpaqueColorTarget != TargetPass.Color.nameID)
                    {
                        OpaqueDrawPass.ConfigureTarget(TargetPass.Color, TargetPass.Depth);
                        if (SkyboxPass != null)
                            SkyboxPass.ConfigureTarget(TargetPass.Color, TargetPass.Depth);
                    }
                    if (currentTransparentColor != TargetPass.Color.nameID)
                        TransparentDrawPass.ConfigureTarget(TargetPass.Color, TargetPass.Depth);
                }
            }
            else
                _storedOriginalColorTarget = false;
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (Settings.Filter == PixelizationFilter.FullScene && !_currentlyDrawingPreviewCamera)
            {
                if (OpaqueDrawPass != null && TransparentDrawPass != null)
                {
#if URP_RTHANDLE
                    var currentOpaqueColor = OpaqueDrawPass.colorAttachmentHandle;
                    var currentTransparentColor = TransparentDrawPass.colorAttachmentHandle;
#else
                    var currentOpaqueColor = OpaqueDrawPass.colorAttachment;
                    var currentTransparentColor = TransparentDrawPass.colorAttachment;
#endif
                    if (currentOpaqueColor != _originalOpaqueColorTarget && _storedOriginalColorTarget)
                    {
                        OpaqueDrawPass.ConfigureTarget(_originalOpaqueColorTarget, _originalOpaqueDepthTarget);
                        if (SkyboxPass != null)
                            SkyboxPass.ConfigureTarget(_originalOpaqueColorTarget, _originalOpaqueDepthTarget);
                    }
                    if (currentTransparentColor != _originalOpaqueColorTarget && _storedOriginalColorTarget)
                        TransparentDrawPass.ConfigureTarget(_originalOpaqueColorTarget, _originalOpaqueDepthTarget);
                }
            }
        }

        public bool ShouldRestoreDrawPassTargets => Settings.Filter != PixelizationFilter.FullScene;

#if URP_UNIVERSALRENDERER
        public void FindDrawObjectPasses(UniversalRenderer renderer, in RenderingData renderingData)
#else
        public void FindDrawObjectPasses(ForwardRenderer renderer, in RenderingData renderingData)
#endif
        {
            FindDrawObjectPasses(renderer, renderingData.cameraData);
        }

#if URP_UNIVERSALRENDERER
        public void FindDrawObjectPasses(UniversalRenderer renderer, in CameraData cameraData)
#else
        public void FindDrawObjectPasses(ForwardRenderer renderer, in CameraData cameraData)
#endif
        {
            // In 2022.3+, we can no longer just redirect the pass by leaving the target set as the low res target just before opaques are drawn.
            // Instead, we find the pass and change the target color and depth buffers.
            // Note that we can't get activeRenderPassQueue from the renderer because it's protected. I don't want to create a ProPixelizerRenderer
            // to work around this limitation.
            // This must also run for preview cameras, even though we don't want to modify them, so that the references are to the current targets.
            OpaqueDrawPass = OpaqueForwardRendererFieldGetter.GetValue(renderer) as DrawObjectsPass;
            TransparentDrawPass = TransparentForwardRendererFieldGetter.GetValue(renderer) as DrawObjectsPass;
            SkyboxPass = SkyboxFieldGetter.GetValue(renderer) as DrawSkyboxPass;

            // this is required for when someone changes ProPixelizer from Fullscreen mode to Hybrid mode, and we have to undo the redirect.
            // This is likely in editor when people are originally playing with the settings, but it's never
            // going to occur again once they've decided.
            if (ShouldRestoreDrawPassTargets || cameraData.cameraType == CameraType.Preview)
            {
#if URP_RTHANDLE
                if (OpaqueDrawPass != null)
                    OpaqueDrawPass.ConfigureTarget(cameraData.renderer.cameraColorTargetHandle, cameraData.renderer.cameraDepthTargetHandle);
                if (TransparentDrawPass != null)
                    TransparentDrawPass.ConfigureTarget(cameraData.renderer.cameraColorTargetHandle, cameraData.renderer.cameraDepthTargetHandle);
                if (SkyboxPass != null)
                    SkyboxPass.ConfigureTarget(cameraData.renderer.cameraColorTargetHandle, cameraData.renderer.cameraDepthTargetHandle);
#else
                if (OpaqueDrawPass != null)
                    OpaqueDrawPass.ConfigureTarget(cameraData.renderer.cameraColorTarget, cameraData.renderer.cameraDepthTarget);
                if (TransparentDrawPass != null)
                    TransparentDrawPass.ConfigureTarget(cameraData.renderer.cameraColorTarget, cameraData.renderer.cameraDepthTarget);
                if (SkyboxPass != null)
                    SkyboxPass.ConfigureTarget(cameraData.renderer.cameraColorTarget, cameraData.renderer.cameraDepthTarget);
#endif
            }
            _currentlyDrawingPreviewCamera = cameraData.cameraType == CameraType.Preview;
        }

#if URP_UNIVERSALRENDERER
        readonly static FieldInfo OpaqueForwardRendererFieldGetter = typeof(UniversalRenderer).GetField("m_RenderOpaqueForwardPass", BindingFlags.NonPublic | BindingFlags.Instance);
        readonly static FieldInfo TransparentForwardRendererFieldGetter = typeof(UniversalRenderer).GetField("m_RenderTransparentForwardPass", BindingFlags.NonPublic | BindingFlags.Instance);
        readonly static FieldInfo SkyboxFieldGetter = typeof(UniversalRenderer).GetField("m_DrawSkyboxPass", BindingFlags.NonPublic | BindingFlags.Instance);
#else
        readonly static FieldInfo OpaqueForwardRendererFieldGetter = typeof(ForwardRenderer).GetField("m_RenderOpaqueForwardPass", BindingFlags.NonPublic | BindingFlags.Instance);
        readonly static FieldInfo TransparentForwardRendererFieldGetter = typeof(ForwardRenderer).GetField("m_RenderTransparentForwardPass", BindingFlags.NonPublic | BindingFlags.Instance);
        readonly static FieldInfo SkyboxFieldGetter = typeof(ForwardRenderer).GetField("m_DrawSkyboxPass", BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        private DrawObjectsPass OpaqueDrawPass;
        private DrawObjectsPass TransparentDrawPass;
        private DrawSkyboxPass SkyboxPass;
        public static FieldInfo DrawPassFilterSettingsField = typeof(DrawObjectsPass).GetField("m_FilteringSettings", BindingFlags.NonPublic | BindingFlags.Instance);

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer buffer = CommandBufferPool.Get(nameof(ProPixelizerOpaqueDrawRedirectionPass));
            if (renderingData.cameraData.cameraType == CameraType.Preview) // preview camera badly bugged, doesnt call all URP frame methods
            {
                buffer.SetGlobalFloat(ProPixelizerFloats.PROPIXELIZER_PASS, 1.0f);
                buffer.SetRenderTarget(TargetPass.Color, TargetPass.Depth);
                buffer.ClearRenderTarget(true, true, renderingData.cameraData.camera.backgroundColor);
                context.ExecuteCommandBuffer(buffer);
                CommandBufferPool.Release(buffer);
                return;
            }
            
            if (Settings.Filter == PixelizationFilter.FullScene)
            {
                buffer.SetGlobalFloat(ProPixelizerFloats.PROPIXELIZER_PASS, 1.0f);
                buffer.SetRenderTarget(TargetPass.Color, TargetPass.Depth);
                if (renderingData.cameraData.camera.GetUniversalAdditionalCameraData().renderType == CameraRenderType.Base)
                    buffer.ClearRenderTarget(true, true, renderingData.cameraData.camera.backgroundColor);
                else
                    buffer.ClearRenderTarget(true, false, renderingData.cameraData.camera.backgroundColor);
                SetLowResViewProjectionMatrices(
                    buffer,
                    ref renderingData,
                    ConfigurationPass.OrthoLowResWidth,
                    ConfigurationPass.OrthoLowResHeight,
                    ConfigurationPass.LowResCameraDeltaWS
                    );
            }
            else
                SetPixelGlobalScale(buffer, 0.01f); // disable pixel size in other cases - draw objects wont be matching metadata buffer.

            context.ExecuteCommandBuffer(buffer);
            CommandBufferPool.Release(buffer);

            var pixelizedLayerMask = renderingData.cameraData.cameraType != CameraType.Preview ? Settings.PixelizedLayers.value : int.MaxValue;
            var cullingMask = Settings.Filter != PixelizationFilter.Layers ? renderingData.cameraData.camera.cullingMask : renderingData.cameraData.camera.cullingMask & ~pixelizedLayerMask;

            if (OpaqueDrawPass != null)
            {
                // change opaque and transparent passes so that they do not draw the pixelated layers.
                var opaqueFilteringSettings = (FilteringSettings)DrawPassFilterSettingsField.GetValue(OpaqueDrawPass);
                opaqueFilteringSettings.layerMask = cullingMask;
                DrawPassFilterSettingsField.SetValue(OpaqueDrawPass, opaqueFilteringSettings);
            }

            if (TransparentDrawPass != null)
            {
                var transparentFilteringSettings = (FilteringSettings)DrawPassFilterSettingsField.GetValue(TransparentDrawPass);
                //different culling mask for transparents?
                transparentFilteringSettings.layerMask = cullingMask;
                DrawPassFilterSettingsField.SetValue(TransparentDrawPass, transparentFilteringSettings);
            }
        }
    }
}