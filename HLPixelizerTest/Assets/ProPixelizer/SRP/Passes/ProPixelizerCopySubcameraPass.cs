// A pass that copies the desired color and depth targets to all of the required textures
// * Scene tab
// * Preview camera
// * Game tab
// * Editor Inspector
// * _Depth targets used for post processing and camera attachment.
//
// Could possibly be a sort of helper function if not a full pass?
// Might aswell make a full pass, easier to test in isolation. And most of these processes blit to a target
// and then back to screen anyway, so this really just replaces the final blit.
//
// Copyright Elliot Bentine, 2018-
using System;
using System.Reflection;
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
    /// This pass copies the depth target from a Subcamera into a longer-lived RTHandle for use during
	/// recomposition for the main camera.
    /// </summary>
    public class ProPixelizerCopySubcameraDepthPass : ProPixelizerPass
	{
		public ProPixelizerCopySubcameraDepthPass(ShaderResources shaders)
		{
			renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
			Materials = new MaterialLibrary(shaders);
		}

        private MaterialLibrary Materials;

		#region non-RG

		private RTHandle _SubcameraDepthOutput;

		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
#if DISABLE_PROPIX_PREVIEW_WINDOW
			// Inspector window in 2022.2 is bugged with a null render target.
			// https://forum.unity.com/threads/preview-camera-does-not-call-setuprenderpasses.1377363/
			// https://forum.unity.com/threads/nullreferenceexception-in-2022-2-1f1-urp-14-0-4-when-rendering-the-inspector-preview-window.1377240/
			if (renderingData.cameraData.cameraType == CameraType.Preview)
				return;
#endif

			var subcamera = renderingData.cameraData.camera.GetComponent<ProPixelizerSubCamera>();
			_SubcameraDepthOutput = subcamera.Depth;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
#if DISABLE_PROPIX_PREVIEW_WINDOW
				// Inspector window in 2022.2 is bugged with a null render target.
				// https://forum.unity.com/threads/preview-camera-does-not-call-setuprenderpasses.1377363/
				// https://forum.unity.com/threads/nullreferenceexception-in-2022-2-1f1-urp-14-0-4-when-rendering-the-inspector-preview-window.1377240/
				if (renderingData.cameraData.cameraType == CameraType.Preview)
					return;
#endif
            if (_SubcameraDepthOutput == null)
                return;

            CommandBuffer buffer = CommandBufferPool.Get(nameof(ProPixelizerCopySubcameraDepthPass));
			bool isOverlay = renderingData.cameraData.camera.GetUniversalAdditionalCameraData().renderType == CameraRenderType.Overlay;

			// Start copying ---------------

			// Copy depth into Color target so that transparents work in scene view tab
			buffer.SetGlobalTexture("_Depth", DepthTarget(ref renderingData));
            Blitter.BlitCameraTexture(buffer, DepthTarget(ref renderingData), _SubcameraDepthOutput, Materials.CopyDepth, 0);

			// ...and restore transformations:
			// ProPixelizerUtils.SetCameraMatrices(buffer, ref renderingData.cameraData);
			context.ExecuteCommandBuffer(buffer);
			CommandBufferPool.Release(buffer);
		}

        public void Dispose()
        {
        }
        #endregion

        #region RG
#if UNITY_2023_3_OR_NEWER

		class PassData
		{
			public TextureHandle CameraDepth;
            public TextureHandle SubcameraDepthOutput;
			public Material copyDepth;
        }

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
			var subcamera = renderingData.cameraData.camera.GetComponent<ProPixelizerSubCamera>();
			if (subcamera == null)
				return;

			var resources = frameData.Get<UniversalResourceData>();
			var camera = frameData.Get<UniversalCameraData>();
			var recomposition = frameData.Get<ProPixelizerRecompositionData>();

			using (var builder = renderGraph.AddUnsafePass<PassData>(GetType().Name, out var passData))
			{
				passData.SubcameraDepthOutput = subcamera.Depth;
				passData.CameraDepth = CameraDepthTexture(resources, camera);
				passData.copyDepth = Materials.CopyDepth;

				builder.AllowGlobalStateModification(true);
				builder.AllowPassCulling(false);
				builder.ImportTexture(SubcameraDepthOutput)
				builder.UseTexture(passData.SubcameraDepthOutput, AccessFlags.Write);
                builder.UseTexture(passData.CameraDepth, AccessFlags.Read);
				builder.SetRenderFunc(
					(PassData data, UnsafeGraphContext context) =>
					{
						ExecutePass(
							CommandBufferHelpers.GetNativeCommandBuffer(context.cmd),
							data
							);
					});
            }
        }

		static void ExecutePass(CommandBuffer command, PassData data)
		{
			buffer.SetGlobalTexture("_Depth", data.CameraDepth);
            Blitter.BlitCameraTexture(buffer, data.CameraDepth, data.SubcameraDepthOutput, Materials.CopyDepth, 0);
        }

#endif
        #endregion

        /// <summary>
        /// Shader resources used by the PixelizationPass.
        /// </summary>
        [Serializable]
		public sealed class ShaderResources
		{
			public Shader CopyDepth;
            private const string CopyDepthShaderName = "Hidden/ProPixelizer/SRP/BlitCopyDepth";

            public ShaderResources Load()
			{
				CopyDepth = Shader.Find(CopyDepthShaderName);
				return this;
			}
		}

		/// <summary>
		/// Materials used by the PixelizationPass
		/// </summary>
		public sealed class MaterialLibrary
		{
			private ShaderResources Resources;
			private Material _CopyDepth;
			public Material CopyDepth
			{
				get
				{
					if (_CopyDepth == null)
						_CopyDepth = new Material(Resources.CopyDepth);
					return _CopyDepth;
				}
			}
			public MaterialLibrary(ShaderResources resources)
			{
				Resources = resources;
			}
		}
	}
}