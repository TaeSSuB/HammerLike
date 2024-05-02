// Copyright Elliot Bentine, 2018-
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProPixelizer
{
    /// <summary>
    /// Renders objects to be pixelated into the low-res render target.
    /// 
    /// This Pass prepares render targets that are used by other passes. This pass retains
    /// responsibility for disposing of these targets.
    /// </summary>
    public class ProPixelizerLowResolutionDrawPass : ProPixelizerPass
    {
        public ProPixelizerLowResolutionDrawPass(ProPixelizerLowResolutionConfigurationPass configPass, ProPixelizerLowResolutionTargetPass targetPass)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            ConfigPass = configPass;
            TargetPass = targetPass;
        }
        ProfilingSampler m_ProfilingSampler = new ProfilingSampler(nameof(ProPixelizerLowResolutionDrawPass));
        private ProPixelizerLowResolutionConfigurationPass ConfigPass;
        private ProPixelizerLowResolutionTargetPass TargetPass;

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer buffer = CommandBufferPool.Get();
            using (new ProfilingScope(buffer, m_ProfilingSampler))
            {
                buffer.SetRenderTarget(TargetPass.Color, TargetPass.Depth);
                buffer.ClearRenderTarget(true, true, renderingData.cameraData.camera.backgroundColor); // dont use cameraData.backgroundColor -> doesnt exist for 2021 lts

                if (Settings.UsePixelExpansion && renderingData.cameraData.cameraType != CameraType.Preview)
                    SetPixelGlobalScale(buffer, 1f);
                else
                    SetPixelGlobalScale(buffer, 0.01f);

                // Set camera matrices for rendering.
                SetLowResViewProjectionMatrices(buffer, ref renderingData, ConfigPass.OrthoLowResWidth, ConfigPass.OrthoLowResHeight, ConfigPass.LowResCameraDeltaWS);
                if (Settings.Filter == PixelizationFilter.OnlyProPixelizer)
                    buffer.SetGlobalFloat(ProPixelizerFloats.PROPIXELIZER_PASS, 1.0f);
                context.ExecuteCommandBuffer(buffer);
                buffer.Clear();

                
                var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
                if (Settings.Filter == PixelizationFilter.Layers)
                {
                    var layerMask = renderingData.cameraData.cameraType != CameraType.Preview ? Settings.PixelizedLayers.value : -1;
                    filteringSettings.layerMask = layerMask;
                }

#if UNITY_2022_2_OR_NEWER
                var drawingSettings = RenderingUtils.CreateDrawingSettings(new ShaderTagId("UNIVERSALFORWARD"), ref renderingData, SortingCriteria.CommonOpaque);
#else
                var drawingSettings = new DrawingSettings(new ShaderTagId("UNIVERSALFORWARD"), new SortingSettings(renderingData.cameraData.camera));
#endif
                if (Settings.Filter == PixelizationFilter.OnlyProPixelizer)
                {
                    // Draw only shaders with "RenderType" = "ProPixelizerRenderType"
                    var stateBlocks = new NativeArray<RenderStateBlock>(2, Allocator.Temp);
                    stateBlocks[0] = new RenderStateBlock(RenderStateMask.Nothing);
                    stateBlocks[1] = new RenderStateBlock(RenderStateMask.Everything)
                    {
                        depthState = new DepthState(false, CompareFunction.Never),
                    };

                    // Create the parameters that tell Unity when to override the render state
                    ShaderTagId renderType = new ShaderTagId("ProPixelizer");
                    var renderTypes = new NativeArray<ShaderTagId>(2, Allocator.Temp);
                    renderTypes[0] = renderType;
                    renderTypes[1] = new ShaderTagId(); //default -> 'all others'.
                    
                    #if UNITY_2022_3_OR_NEWER
                        var renderListParams = new RendererListParams(renderingData.cullResults, drawingSettings, filteringSettings);
                        renderListParams.isPassTagName = false;
                        renderListParams.tagName = new ShaderTagId("RenderType");
                        renderListParams.stateBlocks = stateBlocks;
                        renderListParams.tagValues = renderTypes;
                        var renderList = context.CreateRendererList(ref renderListParams);
                        buffer.DrawRendererList(renderList);
                        context.ExecuteCommandBuffer(buffer);
                        buffer.Clear();
                    #else
                        context.ExecuteCommandBuffer(buffer);
                        buffer.Clear();
                        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, renderTypes, stateBlocks);
                    #endif
                    stateBlocks.Dispose();
                    renderTypes.Dispose();
                }
                
                if (Settings.Filter == PixelizationFilter.Layers)
                {
                    //draw everything on the layer mask to the low-res texture.
                    #if UNITY_2022_3_OR_NEWER
                        var renderListParams = new RendererListParams(renderingData.cullResults, drawingSettings, filteringSettings);
                        var renderList = context.CreateRendererList(ref renderListParams);
                        buffer.DrawRendererList(renderList);
                    #else
                        context.ExecuteCommandBuffer(buffer);
                        buffer.Clear();
                        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
                    #endif
                }

                // restore initial targets.
                SetColorAndDepthTargets(buffer, ref renderingData);
            }
            context.ExecuteCommandBuffer(buffer);
            CommandBufferPool.Release(buffer);
        }
    }
}