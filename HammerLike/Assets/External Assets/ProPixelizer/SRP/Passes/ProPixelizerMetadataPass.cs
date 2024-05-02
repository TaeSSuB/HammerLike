// Copyright Elliot Bentine, 2018-
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProPixelizer
{
    /// <summary>
    /// Renders object shaders with LightMode=ProPixelizer to ProPixelizer_Metadata.
	/// 
	/// Requires the size of the buffers to be predetermined.
    /// </summary>
    public class ProPixelizerMetadataPass : ProPixelizerPass
	{
		public ProPixelizerMetadataPass(ProPixelizerLowResolutionConfigurationPass config)
		{
			renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
			ConfigurationPass = config;
		}
        ProfilingSampler m_ProfilingSampler = new ProfilingSampler(nameof(ProPixelizerMetadataPass));
        private ProPixelizerLowResolutionConfigurationPass ConfigurationPass;

        /// <summary>
        /// Buffer into which objects are rendered using the Outline pass.
        /// </summary>
        public RTHandle MetadataObjectBuffer;
        public RTHandle MetadataObjectBuffer_Depth;

		public const string PROPIXELIZER_METADATA_BUFFER = "ProPixelizer_Metadata";
		public const string PROPIXELIZER_METADATA_BUFFER_DEPTH = "ProPixelizer_Metadata_Depth";

		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			var outlineDescriptor = cameraTextureDescriptor;
			outlineDescriptor.useMipMap = false;
			outlineDescriptor.colorFormat = RenderTextureFormat.ARGB32;
			outlineDescriptor.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;
			outlineDescriptor.width = ConfigurationPass.Width;
			outlineDescriptor.height = ConfigurationPass.Height;
#if URP_13
            var depthDescriptor = outlineDescriptor;
			depthDescriptor.colorFormat = RenderTextureFormat.Depth;
			outlineDescriptor.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref MetadataObjectBuffer, outlineDescriptor, name: PROPIXELIZER_METADATA_BUFFER, wrapMode: TextureWrapMode.Clamp);
            RenderingUtils.ReAllocateIfNeeded(ref MetadataObjectBuffer_Depth, depthDescriptor, name: PROPIXELIZER_METADATA_BUFFER_DEPTH, wrapMode: TextureWrapMode.Clamp);
#else
            MetadataObjectBuffer = RTHandles.Alloc(PROPIXELIZER_METADATA_BUFFER, name: PROPIXELIZER_METADATA_BUFFER);
			MetadataObjectBuffer_Depth = MetadataObjectBuffer;
			cmd.GetTemporaryRT(Shader.PropertyToID(MetadataObjectBuffer.name), outlineDescriptor, FilterMode.Point);
#endif
			ConfigureTarget(MetadataObjectBuffer, MetadataObjectBuffer_Depth);
			ConfigureClear(ClearFlag.All, new Color(0.5f, 1f, 0.5f, 0f));
		}

		public override void FrameCleanup(CommandBuffer cmd)
		{
#if URP_13
#else
			cmd.ReleaseTemporaryRT(Shader.PropertyToID(MetadataObjectBuffer.name));
			// NB: depth not allocated pre URP13 - no need to release it.
#endif
		}

#if URP_13
        public void Dispose()
        {
            MetadataObjectBuffer?.Release();
            MetadataObjectBuffer_Depth?.Release();
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
#endif

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer buffer = CommandBufferPool.Get();
			using (new ProfilingScope(buffer, m_ProfilingSampler))
			{
				// Set up matrices for rendering outlines.
				//ProPixelizerUtils.SetCameraMatrices(buffer, ref renderingData.cameraData);
				SetLowResViewProjectionMatrices(buffer, ref renderingData, ConfigurationPass.OrthoLowResWidth, ConfigurationPass.OrthoLowResHeight, ConfigurationPass.LowResCameraDeltaWS);
                if (Settings.UsePixelExpansion && renderingData.cameraData.cameraType != CameraType.Preview)
					SetPixelGlobalScale(buffer, 1f);
				else
                    SetPixelGlobalScale(buffer, 0.01f);
				if (Settings.Filter == PixelizationFilter.OnlyProPixelizer)
					buffer.SetGlobalFloat(ProPixelizerFloats.PROPIXELIZER_PASS, 1.0f);
				buffer.SetRenderTarget(MetadataObjectBuffer, MetadataObjectBuffer_Depth);
				context.ExecuteCommandBuffer(buffer);
                buffer.Clear();

                var sort = new SortingSettings(renderingData.cameraData.camera);
				var drawingSettings = new DrawingSettings(ProPixelizerMetadataLightmodeShaderTag, sort);
				var filteringSettings = new FilteringSettings(RenderQueueRange.all);

				if (Settings.Filter == PixelizationFilter.Layers)
				{
					// In layer mode, metadata is only rendered for layered objects.
					// In preview mode, treat all as pixelized.
					var layerMask = renderingData.cameraData.cameraType != CameraType.Preview ? Settings.PixelizedLayers.value : int.MaxValue;
                    filteringSettings.layerMask = layerMask;
				}

				#if UNITY_2022_3_OR_NEWER
					var renderListParams = new RendererListParams(renderingData.cullResults, drawingSettings, filteringSettings);
					var renderList = context.CreateRendererList(ref renderListParams);
					buffer.DrawRendererList(renderList);
                #else
                    context.ExecuteCommandBuffer(buffer);
                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
                    buffer.Clear();
                #endif

                // Disable ProPixelizer pass variable
                if (Settings.Filter == PixelizationFilter.OnlyProPixelizer)
                    buffer.SetGlobalFloat(ProPixelizerFloats.PROPIXELIZER_PASS, 0.0f);

				// Restore camera matrices
				ProPixelizerUtils.SetCameraMatrices(buffer, ref renderingData.cameraData);
			}
            context.ExecuteCommandBuffer(buffer);
			CommandBufferPool.Release(buffer);
		}
	}
}