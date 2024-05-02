// Copyright Elliot Bentine, 2018-
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProPixelizer
{
    /// <summary>
    /// Creates the low resolution target used by ProPixelizer.
    /// </summary>
    public class ProPixelizerLowResolutionTargetPass : ProPixelizerPass
    {
        public ProPixelizerLowResolutionTargetPass(ProPixelizerLowResolutionConfigurationPass configPass) : base()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            ConfigPass = configPass;
        }

        /// <summary>
        /// Low resolution render target for scene color.
        /// </summary>
        public RTHandle Color;
        /// <summary>
        /// Low resolution render target for scene depth.
        /// </summary>
        public RTHandle Depth;

        private ProPixelizerLowResolutionConfigurationPass ConfigPass;
        private const string PROPIXELIZER_LOWRES = "ProPixelizer_LowRes";
        private const string PROPIXELIZER_LOWRES_DEPTH = "ProPixelizer_LowRes_Depth";

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var colorDescriptor = cameraTextureDescriptor;
            colorDescriptor.useMipMap = false;
            colorDescriptor.width = ConfigPass.Width;
            colorDescriptor.height = ConfigPass.Height;

            var depthDescriptor = colorDescriptor;
            depthDescriptor.colorFormat = RenderTextureFormat.Depth;
            depthDescriptor.depthBufferBits = 32;
#if URP_13
            colorDescriptor.depthBufferBits = 0;
            if (Settings.Filter == PixelizationFilter.FullScene && Settings.UsePixelArtUpscalingFilter)
                RenderingUtils.ReAllocateIfNeeded(ref Color, colorDescriptor, name: PROPIXELIZER_LOWRES, filterMode: FilterMode.Bilinear);
            else // disable pixelart AA filter for hybrid mode.
                RenderingUtils.ReAllocateIfNeeded(ref Color, colorDescriptor, name: PROPIXELIZER_LOWRES, filterMode: FilterMode.Point);
            RenderingUtils.ReAllocateIfNeeded(ref Depth, depthDescriptor, name: PROPIXELIZER_LOWRES_DEPTH);
#else
            Color = RTHandles.Alloc(PROPIXELIZER_LOWRES, name: PROPIXELIZER_LOWRES);
            Depth = RTHandles.Alloc(PROPIXELIZER_LOWRES_DEPTH, name: PROPIXELIZER_LOWRES_DEPTH);
            cmd.GetTemporaryRT(Shader.PropertyToID(Color.name), colorDescriptor, FilterMode.Bilinear);
            cmd.GetTemporaryRT(Shader.PropertyToID(Depth.name), depthDescriptor, FilterMode.Point);
#endif
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
#if URP_13
#else
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(Color.name));
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(Depth.name));
#endif
        }

#if URP_13
        public void Dispose()
        {
            Color?.Release();
            Depth?.Release();
        }
#endif

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // set the pixel scale according to pixel mode.
            CommandBuffer buffer = CommandBufferPool.Get(nameof(ProPixelizerLowResRecompositionPass));
            
            // Turn off pixel size for now - this covers propixelizer shaders on an un pixelated layer, for instance.
            SetPixelGlobalScale(buffer, 0.01f);
            if (Settings.UsePixelExpansion && renderingData.cameraData.cameraType != CameraType.Preview)
            {
                buffer.EnableShaderKeyword(ProPixelizerKeywords.PIXEL_EXPANSION);
            }
            else
            {
                buffer.DisableShaderKeyword(ProPixelizerKeywords.PIXEL_EXPANSION);
            }
            if (Settings.Filter == PixelizationFilter.FullScene)
            {
                if (Settings.UsePixelExpansion && renderingData.cameraData.cameraType != CameraType.Preview)
                {
                    SetPixelGlobalScale(buffer, 1f);
                }
                buffer.EnableShaderKeyword(ProPixelizerKeywords.FULL_SCENE);
            }
            else
            {
                buffer.DisableShaderKeyword(ProPixelizerKeywords.FULL_SCENE);
            }
            if (Settings.Filter == PixelizationFilter.Layers || !Settings.Enabled)
            {
                buffer.SetGlobalFloat(ProPixelizerFloats.PROPIXELIZER_PASS, 1.0f);
            }
            context.ExecuteCommandBuffer(buffer);
            CommandBufferPool.Release(buffer);
        }
    }
}