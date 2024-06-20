// Copyright Elliot Bentine, 2018-
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_2023_3_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif
using UnityEngine.Rendering.Universal;

#pragma warning disable 0672

namespace ProPixelizer
{
    /// <summary>
    /// Disables ProPixelizer shaders from being drawn..
    /// </summary>
    public class ProPixelizerHideShadersPass : ProPixelizerPass
    {
        public ProPixelizerHideShadersPass() : base()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        }

        #region non-RG
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer buffer = CommandBufferPool.Get(nameof(ProPixelizerHideShadersPass));
            buffer.SetGlobalFloat(ProPixelizerFloats.PROPIXELIZER_PASS, 0.0f);
            context.ExecuteCommandBuffer(buffer);
            CommandBufferPool.Release(buffer);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }
        #endregion

        #region RG
#if UNITY_2023_3_OR_NEWER
        class PassData
        {
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(GetType().Name, out var passData))
            {
                builder.AllowPassCulling(false);
                builder.AllowGlobalStateModification(true);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    context.cmd.SetGlobalFloat(ProPixelizerFloats.PROPIXELIZER_PASS, 0.0f);
                });
            }
        }
#endif
        #endregion
    }
}