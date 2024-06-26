// Copyright Elliot Bentine, 2018-
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_2023_3_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

#pragma warning disable 0672

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

        #region non-RG
        ProPixelizerLowResolutionTargetPass TargetPass;
        ProPixelizerLowResolutionConfigurationPass ConfigurationPass;

        bool _currentlyDrawingPreviewCamera;
        private bool _storedOriginalColorTarget; // non-nullable types
        RTHandle _originalOpaqueColorTarget;
        RTHandle _originalOpaqueDepthTarget;

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // workaround for stupid bug: https://forum.unity.com/threads/preview-camera-does-not-call-setuprenderpasses.1377363/
            if (renderingData.cameraData.cameraType == CameraType.Preview)
                FindDrawObjectPasses(renderingData.cameraData.renderer as UniversalRenderer, renderingData.cameraData);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (Settings.Filter == PixelizationFilter.FullScene && !_currentlyDrawingPreviewCamera)
            {
                if (OpaqueDrawPass != null && TransparentDrawPass != null)
                {
                    _originalOpaqueColorTarget = OpaqueDrawPass.colorAttachmentHandle;
                    _originalOpaqueDepthTarget = OpaqueDrawPass.depthAttachmentHandle;
                    var currentTransparentColor = TransparentDrawPass.colorAttachmentHandle;

#pragma warning disable 0618
                    if (_originalOpaqueColorTarget != TargetPass.Color.nameID)
                    {
                        OpaqueDrawPass.ConfigureTarget(TargetPass.Color, TargetPass.Depth);
                        if (SkyboxPass != null)
                            SkyboxPass.ConfigureTarget(TargetPass.Color, TargetPass.Depth);
                    }
                    if (currentTransparentColor != TargetPass.Color.nameID)
                        TransparentDrawPass.ConfigureTarget(TargetPass.Color, TargetPass.Depth);
#pragma warning restore 0618
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
                    var currentOpaqueColor = OpaqueDrawPass.colorAttachmentHandle;
                    var currentTransparentColor = TransparentDrawPass.colorAttachmentHandle;
#pragma warning disable 0618
                    if (currentOpaqueColor != _originalOpaqueColorTarget && _storedOriginalColorTarget)
                    {
                        OpaqueDrawPass.ConfigureTarget(_originalOpaqueColorTarget, _originalOpaqueDepthTarget);
                        if (SkyboxPass != null)
                            SkyboxPass.ConfigureTarget(_originalOpaqueColorTarget, _originalOpaqueDepthTarget);
                    }
                    if (currentTransparentColor != _originalOpaqueColorTarget && _storedOriginalColorTarget)
                        TransparentDrawPass.ConfigureTarget(_originalOpaqueColorTarget, _originalOpaqueDepthTarget);
#pragma warning restore 0618
                }
            }
        }

        public bool ShouldRestoreDrawPassTargets => Settings.Filter != PixelizationFilter.FullScene;

        public void FindDrawObjectPasses(UniversalRenderer renderer, in RenderingData renderingData)
        {
            FindDrawObjectPasses(renderer, renderingData.cameraData);
        }

        public void FindDrawObjectPasses(UniversalRenderer renderer, in CameraData cameraData)
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
#pragma warning disable 0618
                if (OpaqueDrawPass != null)
                    OpaqueDrawPass.ConfigureTarget(cameraData.renderer.cameraColorTargetHandle, cameraData.renderer.cameraDepthTargetHandle);
                if (TransparentDrawPass != null)
                    TransparentDrawPass.ConfigureTarget(cameraData.renderer.cameraColorTargetHandle, cameraData.renderer.cameraDepthTargetHandle);
                if (SkyboxPass != null)
                    SkyboxPass.ConfigureTarget(cameraData.renderer.cameraColorTargetHandle, cameraData.renderer.cameraDepthTargetHandle);
#pragma warning restore 0618
            }
            _currentlyDrawingPreviewCamera = cameraData.cameraType == CameraType.Preview;
        }

        readonly static FieldInfo OpaqueForwardRendererFieldGetter = typeof(UniversalRenderer).GetField("m_RenderOpaqueForwardPass", BindingFlags.NonPublic | BindingFlags.Instance);
        readonly static FieldInfo TransparentForwardRendererFieldGetter = typeof(UniversalRenderer).GetField("m_RenderTransparentForwardPass", BindingFlags.NonPublic | BindingFlags.Instance);
        readonly static FieldInfo SkyboxFieldGetter = typeof(UniversalRenderer).GetField("m_DrawSkyboxPass", BindingFlags.NonPublic | BindingFlags.Instance);

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
                ProPixelizerUtils.SetLowResViewProjectionMatrices(
                    buffer,
                    ref renderingData.cameraData,
                    ConfigurationPass.Data.OrthoLowResWidth,
                    ConfigurationPass.Data.OrthoLowResHeight,
                    ConfigurationPass.Data.LowResCameraDeltaWS,
                    ConfigurationPass.Data.LowResPerspectiveVerticalFOV,
                    ConfigurationPass.Data.LowResAspect
                    );
            }
            else
                ProPixelizerUtils.SetPixelGlobalScale(buffer, 0.01f); // disable pixel size in other cases - draw objects wont be matching metadata buffer.

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
        #endregion

#if UNITY_2023_3_OR_NEWER
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // nothing - no effect in RG
        }
#endif
    }
}