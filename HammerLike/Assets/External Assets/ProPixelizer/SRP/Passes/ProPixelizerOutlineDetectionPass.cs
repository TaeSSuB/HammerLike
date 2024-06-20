// Copyright Elliot Bentine, 2018-
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
        private ProPixelizerLowResolutionConfigurationPass ConfigurationPass;
        private ProPixelizerMetadataPass MetadataPass;
        public const string OUTLINE_BUFFER = "_ProPixelizerOutlines";

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
            outlineDescriptor.width = ConfigurationPass.Width;
            outlineDescriptor.height = ConfigurationPass.Height;
#if URP_13
            outlineDescriptor.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref OutlineBuffer, outlineDescriptor, name: OUTLINE_BUFFER);
#else
            OutlineBuffer = RTHandles.Alloc(OUTLINE_BUFFER, name: OUTLINE_BUFFER);
            cmd.GetTemporaryRT(Shader.PropertyToID(OutlineBuffer.name), outlineDescriptor, FilterMode.Point);
#endif
            //ConfigureTarget(OutlineBuffer);
            ConfigureClear(ClearFlag.None, new Color(1f,1f,1f,0f));
        }
        public override void FrameCleanup(CommandBuffer cmd)
        {
#if URP_13
#else
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(OutlineBuffer.name));
#endif
        }

#if URP_13
        public void Dispose() => OutlineBuffer?.Release();

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // set externally managed RTHandle references to null.
        }
#endif

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (Settings.UseDepthTestingForIDOutlines)
            {
                Materials.OutlineDetection.EnableKeyword("DEPTH_TEST_OUTLINES_ON");
                Materials.OutlineDetection.SetFloat("_OutlineDepthTestThreshold", Settings.DepthTestThreshold);
            }
            else
                Materials.OutlineDetection.DisableKeyword("DEPTH_TEST_OUTLINES_ON");

            if (Settings.UseDepthTestingForEdgeOutlines)
                Materials.OutlineDetection.EnableKeyword("DEPTH_TEST_NORMAL_EDGES_ON");
            else
                Materials.OutlineDetection.DisableKeyword("DEPTH_TEST_NORMAL_EDGES_ON");


            if (Settings.UseNormalsForEdgeDetection)
            {
                Materials.OutlineDetection.SetFloat("_NormalEdgeDetectionSensitivity", Settings.NormalEdgeDetectionThreshold);
            }

            CommandBuffer buffer = CommandBufferPool.Get(nameof(ProPixelizerOutlineDetectionPass));

            if (Settings.UseNormalsForEdgeDetection)
                buffer.EnableShaderKeyword("NORMAL_EDGE_DETECTION_ON");
            else
                buffer.DisableShaderKeyword("NORMAL_EDGE_DETECTION_ON");

            buffer.SetGlobalTexture("_MainTex", MetadataPass.MetadataObjectBuffer);
#if URP_13
            buffer.SetGlobalTexture("_MainTex_Depth", MetadataPass.MetadataObjectBuffer_Depth);//, RenderTextureSubElement.Depth);
#else
            buffer.SetGlobalTexture("_MainTex_Depth", MetadataPass.MetadataObjectBuffer_Depth, RenderTextureSubElement.Depth);
#endif
            //buffer.SetGlobalVector("_TexelSize", TexelSize);
            buffer.SetGlobalVector("_TexelSize", new Vector4(
                1f / ConfigurationPass.Width,
                1f / ConfigurationPass.Height,
                ConfigurationPass.Width,
                ConfigurationPass.Height
            ));

#if BLIT_API
            Blitter.BlitCameraTexture(buffer, MetadataPass.MetadataObjectBuffer, OutlineBuffer, Materials.OutlineDetection, 0);
#else
            Blit(buffer, MetadataPass.MetadataObjectBuffer, OutlineBuffer, Materials.OutlineDetection);
#endif

            buffer.SetGlobalTexture(OUTLINE_BUFFER, OutlineBuffer);
            buffer.SetRenderTarget(ColorTarget(ref renderingData), DepthTarget(ref renderingData));
            context.ExecuteCommandBuffer(buffer);
            CommandBufferPool.Release(buffer);
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