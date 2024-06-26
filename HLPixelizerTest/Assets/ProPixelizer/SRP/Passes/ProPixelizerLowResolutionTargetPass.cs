// Copyright Elliot Bentine, 2018-
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_2023_3_OR_NEWER
using ProPixelizer.RenderGraphResources;
using UnityEngine.Rendering.RenderGraphModule;
#endif
using UnityEngine.Rendering.Universal;

#pragma warning disable 0672

namespace ProPixelizer
{
    /// <summary>
    /// Creates the low resolution target used by ProPixelizer.
    /// </summary>
    public class ProPixelizerLowResolutionTargetPass : ProPixelizerPass
    {
        public ProPixelizerLowResolutionTargetPass(ProPixelizerLowResolutionConfigurationPass configPass) : base()
        {
            renderPassEvent = RenderPassEvent.BeforeRendering;
            ConfigPass = configPass;
        }

        #region Non-RG
        /// <summary>
        /// Low resolution render target for scene color.
        /// </summary>
        public RTHandle Color;
        /// <summary>
        /// Low resolution render target for scene depth.
        /// </summary>
        public RTHandle Depth;

        private ProPixelizerLowResolutionConfigurationPass ConfigPass;

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var colorDescriptor = cameraTextureDescriptor;
            colorDescriptor.useMipMap = false;
            if (ConfigPass.Data.Width > 0)
            {
                colorDescriptor.width = ConfigPass.Data.Width;
                colorDescriptor.height = ConfigPass.Data.Height;
            }

            var depthDescriptor = colorDescriptor;
            depthDescriptor.colorFormat = RenderTextureFormat.Depth;
            depthDescriptor.depthBufferBits = 32;
            colorDescriptor.depthBufferBits = 0;

            // only allocate targets if they are needed.
            if (Settings.TargetForPixelation == TargetForPixelation.ProPixelizerTarget)
            {
#pragma warning disable 0618
                if (Settings.Filter == PixelizationFilter.FullScene && Settings.UsePixelArtUpscalingFilter)
                    RenderingUtils.ReAllocateIfNeeded(ref Color, colorDescriptor, name: ProPixelizerTargets.PROPIXELIZER_LOWRES, filterMode: FilterMode.Bilinear);
                else // disable pixelart AA filter for hybrid mode.
                    RenderingUtils.ReAllocateIfNeeded(ref Color, colorDescriptor, name: ProPixelizerTargets.PROPIXELIZER_LOWRES, filterMode: FilterMode.Point);
                RenderingUtils.ReAllocateIfNeeded(ref Depth, depthDescriptor, name: ProPixelizerTargets.PROPIXELIZER_LOWRES_DEPTH);
#pragma warning restore 0618
            }
        }

        public void Dispose()
        {
            Color?.Release();
            Depth?.Release();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // set the pixel scale according to pixel mode.
            CommandBuffer command = CommandBufferPool.Get(nameof(ProPixelizerLowResRecompositionPass));

#if UNITY_2023_3_OR_NEWER
            var passCommand = CommandBufferHelpers.GetRasterCommandBuffer(command);
#else
            var passCommand = command;
#endif
            ExecutePass(passCommand, Settings);

            context.ExecuteCommandBuffer(command);
            CommandBufferPool.Release(command);
        }

        #endregion

        #region RG
#if UNITY_2023_3_OR_NEWER
        public class PassData
        {
            public ProPixelizerSettings settings;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<UniversalCameraData>();
            var lowResConfig = frameData.Get<ProPixelizerTargetConfigurationContextItem>().Config;
            var lowResTarget = frameData.GetOrCreate<LowResTargetData>();
            var resources = frameData.Get<UniversalResourceData>();

            var colorDescriptor = lowResTarget.ColorDescriptor(cameraData.cameraTargetDescriptor, lowResConfig);
            var depthDescriptor = lowResTarget.DepthDescriptor(cameraData.cameraTargetDescriptor, lowResConfig);

            lowResTarget.Color = UniversalRenderer.CreateRenderGraphTexture(renderGraph, colorDescriptor, ProPixelizerTargets.PROPIXELIZER_LOWRES, true, FilterMode.Point);
            lowResTarget.Depth = UniversalRenderer.CreateRenderGraphTexture(renderGraph, depthDescriptor, ProPixelizerTargets.PROPIXELIZER_LOWRES_DEPTH, true, FilterMode.Point);

            using (var builder = renderGraph.AddUnsafePass<PassData>(nameof(ProPixelizerLowResolutionTargetPass), out var passData)
                )
            {
                passData.settings = Settings;
                builder.AllowPassCulling(false);
                builder.AllowGlobalStateModification(true);
                builder.SetRenderFunc((PassData passData, UnsafeGraphContext context) =>
                {
                    ExecutePass(context.cmd, passData.settings);
                });
            }
        }
#endif
        #endregion

#if UNITY_2023_3_OR_NEWER
        public static void ExecutePass(IRasterCommandBuffer command, ProPixelizerSettings settings)
#else
        public static void ExecutePass(CommandBuffer command, ProPixelizerSettings settings)
#endif
        {
            // Turn off pixel size for now - this covers propixelizer shaders on an un pixelated layer, for instance.
            ProPixelizerUtils.SetPixelGlobalScale(command, 0.01f);

            if (settings.UsePixelExpansion)
                command.EnableShaderKeyword(ProPixelizerKeywords.PIXEL_EXPANSION);
            else
                command.DisableShaderKeyword(ProPixelizerKeywords.PIXEL_EXPANSION);
            if (settings.Filter == PixelizationFilter.FullScene)
            {
                if (settings.UsePixelExpansion)
                    ProPixelizerUtils.SetPixelGlobalScale(command, 1f);
                command.EnableShaderKeyword(ProPixelizerKeywords.FULL_SCENE);
            }
            else
                command.DisableShaderKeyword(ProPixelizerKeywords.FULL_SCENE);
            if (settings.Filter == PixelizationFilter.Layers || !settings.Enabled)
                command.SetGlobalFloat(ProPixelizerFloats.PROPIXELIZER_PASS, 1.0f);
        }
    }
}