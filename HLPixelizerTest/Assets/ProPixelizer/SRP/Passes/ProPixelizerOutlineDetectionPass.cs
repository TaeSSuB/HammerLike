﻿// Copyright Elliot Bentine, 2018-
using System;
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
    /// Performs a pass to detect outlines.
    /// 
    /// This passes uses the ProPixelizer metadata buffers, which are populated by the ProPixelizerMetadataPass.
    /// </summary>
    public class ProPixelizerOutlineDetectionPass : ProPixelizerPass
    {
        public ProPixelizerOutlineDetectionPass(ShaderResources resources, ProPixelizerLowResolutionConfigurationPass configPass, ProPixelizerMetadataPass metadataPass)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            Materials = new MaterialLibrary(resources);
            ConfigurationPass = configPass;
            MetadataPass = metadataPass;
        }

        private MaterialLibrary Materials;
        #region non-RG
        private ProPixelizerLowResolutionConfigurationPass ConfigurationPass;
        private ProPixelizerMetadataPass MetadataPass;

        /// <summary>
        /// Buffer to store the results from outline analysis.
        /// </summary>
        public RTHandle OutlineBuffer;

        private const string OutlineDetectionShader = "Hidden/ProPixelizer/SRP/OutlineDetection";

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var outlineDescriptor = cameraTextureDescriptor;
            outlineDescriptor.useMipMap = false;
            outlineDescriptor.colorFormat = RenderTextureFormat.ARGB32;
            outlineDescriptor.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;
            if (ConfigurationPass.Data.Width > 0)
            {
                outlineDescriptor.width = ConfigurationPass.Data.Width;
                outlineDescriptor.height = ConfigurationPass.Data.Height;
            }
            outlineDescriptor.depthBufferBits = 0;
#pragma warning disable 0618
            RenderingUtils.ReAllocateIfNeeded(ref OutlineBuffer, outlineDescriptor, name: ProPixelizerTargets.OUTLINE_BUFFER);
            ConfigureClear(ClearFlag.None, new Color(1f,1f,1f,0f));
#pragma warning restore 0618
        }

        public void Dispose() => OutlineBuffer?.Release();

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // set externally managed RTHandle references to null.
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer command = CommandBufferPool.Get(nameof(ProPixelizerOutlineDetectionPass));

            command.SetGlobalTexture("_MainTex", MetadataPass.MetadataObjectBuffer);
            command.SetGlobalTexture("_MainTex_Depth", MetadataPass.MetadataObjectBuffer_Depth);

            ExecutePass(command,
                ref Settings,
                OutlineBuffer,
                MetadataPass.MetadataObjectBuffer,
                MetadataPass.MetadataObjectBuffer_Depth,
                ConfigurationPass.Data,
                Materials.OutlineDetection,
                new LocalKeyword(Materials.OutlineDetection.shader, ProPixelizerOutlineKeywords.DEPTH_TEST_OUTLINES_ON),
                new LocalKeyword(Materials.OutlineDetection.shader, ProPixelizerOutlineKeywords.DEPTH_TEST_NORMAL_EDGES_ON),
                new LocalKeyword(Materials.OutlineDetection.shader, ProPixelizerOutlineKeywords.NORMAL_EDGE_DETECTION_ON),
                false
                );

            command.SetGlobalTexture(ProPixelizerTargets.OUTLINE_BUFFER, OutlineBuffer);
            command.SetRenderTarget(ColorTarget(ref renderingData), DepthTarget(ref renderingData));

            context.ExecuteCommandBuffer(command);
            CommandBufferPool.Release(command);
        }
        #endregion

        #region RG
#if UNITY_2023_3_OR_NEWER

        class PassData
        {
            public ProPixelizerSettings settings;
            public TextureHandle Outlines;
            public TextureHandle Metadata;
            public TextureHandle MetadataDepth;
            public ProPixelizerLowResolutionConfigurationPass.ProPixelizerTargetConfiguration LowResConfiguration;
            public Material OutlineMaterial;
            public LocalKeyword DEPTH_TEST_OUTLINES_ON;
            public LocalKeyword DEPTH_TEST_NORMAL_EDGES_ON;
            public LocalKeyword NORMAL_EDGE_DETECTION_ON;
        }

        private static readonly int Outline_Buffer_Global_ID = Shader.PropertyToID(ProPixelizerTargets.OUTLINE_BUFFER);

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var outline = frameData.GetOrCreate<OutlineTargetData>();
            var metadata = frameData.Get<MetadataTargetData>();
            var cameraData = frameData.Get<UniversalCameraData>();
            var lowResConfig = frameData.Get<ProPixelizerTargetConfigurationContextItem>();

            var outlineDescriptor = cameraData.cameraTargetDescriptor;
            outlineDescriptor.useMipMap = false;
            outlineDescriptor.colorFormat = RenderTextureFormat.ARGB32;
            outlineDescriptor.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;
            outlineDescriptor.width = lowResConfig.Width;
            outlineDescriptor.height = lowResConfig.Height;
            outlineDescriptor.depthBufferBits = 0;

            outline.Color = UniversalRenderer.CreateRenderGraphTexture(renderGraph, outlineDescriptor, name: ProPixelizerTargets.OUTLINE_BUFFER, true, wrapMode: TextureWrapMode.Clamp);

            var usingBackBuffer = cameraData.cameraType == CameraType.Preview;

            using (var builder = renderGraph.AddUnsafePass<PassData>(GetType().Name, out var passData))
            {
                passData.settings = Settings;
                passData.Outlines = outline.Color;
                passData.Metadata = metadata.Color;
                passData.MetadataDepth = metadata.Depth;
                passData.LowResConfiguration = lowResConfig;
                passData.OutlineMaterial = Materials.OutlineDetection;
                passData.DEPTH_TEST_OUTLINES_ON = new LocalKeyword(Materials.OutlineDetection.shader, ProPixelizerOutlineKeywords.DEPTH_TEST_OUTLINES_ON);
                passData.DEPTH_TEST_NORMAL_EDGES_ON = new LocalKeyword(Materials.OutlineDetection.shader, ProPixelizerOutlineKeywords.DEPTH_TEST_NORMAL_EDGES_ON);
                passData.NORMAL_EDGE_DETECTION_ON = new LocalKeyword(Materials.OutlineDetection.shader, ProPixelizerOutlineKeywords.NORMAL_EDGE_DETECTION_ON);

                builder.SetGlobalTextureAfterPass(outline.Color, Outline_Buffer_Global_ID);
                builder.UseTexture(metadata.Color);
                builder.AllowPassCulling(false);
                builder.AllowGlobalStateModification(true);
                builder.UseTexture(outline.Color, AccessFlags.Write);
                builder.UseTexture(metadata.Color, AccessFlags.Read);
                builder.UseTexture(metadata.Depth, AccessFlags.Read);
                builder.SetRenderFunc(
                    (PassData passData, UnsafeGraphContext context) =>
                    {
                        var command = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                        ExecutePass(
                            command,
                            ref passData.settings,
                            passData.Outlines,
                            passData.Metadata,
                            passData.MetadataDepth,
                            passData.LowResConfiguration,
                            passData.OutlineMaterial,
                            passData.DEPTH_TEST_OUTLINES_ON,
                            passData.DEPTH_TEST_NORMAL_EDGES_ON,
                            passData.NORMAL_EDGE_DETECTION_ON,
                            usingBackBuffer
                            );
                    });
            }
        }

#endif
        #endregion

        static void ExecutePass(
            CommandBuffer command, 
            ref ProPixelizerSettings settings,
            RTHandle outlines,
            RTHandle metadata,
            RTHandle metadataDepth,
            ProPixelizerTargetConfiguration lowResConfiguration,
            Material outlineMaterial,
            LocalKeyword DEPTH_TEST_OUTLINES_ON,
            LocalKeyword DEPTH_TEST_NORMAL_EDGES_ON,
            LocalKeyword NORMAL_EDGE_DETECTION_ON,
            bool colorUsingBackbuffer
            )
        {
            if (settings.UseDepthTestingForIDOutlines)
            {
                command.EnableKeyword(outlineMaterial, DEPTH_TEST_OUTLINES_ON);
                outlineMaterial.SetFloat("_OutlineDepthTestThreshold", settings.DepthTestThreshold);
            }
            else
                command.DisableKeyword(outlineMaterial, DEPTH_TEST_OUTLINES_ON);

            if (settings.UseDepthTestingForEdgeOutlines)
                command.EnableKeyword(outlineMaterial, DEPTH_TEST_NORMAL_EDGES_ON);
            else
                command.DisableKeyword(outlineMaterial, DEPTH_TEST_NORMAL_EDGES_ON);


            if (settings.UseNormalsForEdgeDetection)
            {
                outlineMaterial.SetFloat("_NormalEdgeDetectionSensitivity", settings.NormalEdgeDetectionThreshold);
            }

            if (settings.UseNormalsForEdgeDetection)
                command.EnableKeyword(outlineMaterial, NORMAL_EDGE_DETECTION_ON);
            else
                command.DisableKeyword(outlineMaterial, NORMAL_EDGE_DETECTION_ON);

            //buffer.SetGlobalVector("_TexelSize", TexelSize);
            command.SetGlobalVector("_TexelSize", new Vector4(
                1f / lowResConfiguration.Width,
                1f / lowResConfiguration.Height,
                lowResConfiguration.Width,
                lowResConfiguration.Height
            ));

            // See description of Backbuffer flag.
            if (colorUsingBackbuffer)
                command.SetGlobalFloat(ProPixelizerFloats.BACKBUFFER_FLAG, 1.0f);
            else
                command.SetGlobalFloat(ProPixelizerFloats.BACKBUFFER_FLAG, 0.0f);

            command.SetGlobalTexture("_MainTex", metadata); // required because of blitter api is no longer main tex, but shaders must use MainTex for backwards compatability
            command.SetGlobalTexture("_MainTex_Depth", metadataDepth); // required because of blitter api is no longer main tex, but shaders must use MainTex for backwards compatability
            Blitter.BlitCameraTexture(command, metadata, outlines, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, outlineMaterial, 0);
        }


        /// <summary>
        /// Shader resources used by the OutlineDetectionPass.
        /// </summary>
        [Serializable]
        public sealed class ShaderResources
        {
            public Shader OutlineDetection;

            public ShaderResources Load()
            {
                OutlineDetection = Shader.Find(OutlineDetectionShader);
                return this;
            }
        }

        /// <summary>
        /// Materials used by the OutlineDetectionPass.
        /// </summary>
        public sealed class MaterialLibrary
        {
            private ShaderResources Resources;
            public Material OutlineDetection
            {
                get
                {
                    if (_OutlineDetection == null)
                        _OutlineDetection = new Material(Resources.OutlineDetection);
                    return _OutlineDetection;
                }
            }
            private Material _OutlineDetection;

            public MaterialLibrary(ShaderResources resources)
            {
                Resources = resources;
            }
        }
    }
}