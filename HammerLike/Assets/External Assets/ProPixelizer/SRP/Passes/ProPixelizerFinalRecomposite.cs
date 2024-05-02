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
using UnityEngine.Rendering.Universal;

namespace ProPixelizer
{
    /// <summary>
    /// A pass that blits the desired color and depth targets into all color and depth targets required.
    /// 
    /// These include those used by:
    ///  * The editor scene tab
    ///  * The in-editor preview camera overlay
    ///  * The editor game tab
    ///  * the final game view in build
    ///  * any _Depth or _Opaque targets used for subsequent post-processing and camera attachment.
    /// </summary>
    public class ProPixelizerFinalRecompositionPass : ProPixelizerPass
	{
		public ProPixelizerFinalRecompositionPass(ShaderResources shaders, ProPixelizerLowResRecompositionPass recomp)
		{
			renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
			Materials = new MaterialLibrary(shaders);
			RecompositionPass = recomp;
		}

		public ProPixelizerLowResRecompositionPass RecompositionPass;
		private MaterialLibrary Materials;

		// Inputs - bookkeeping for now
		private RTHandle _ColorInput;
		private RTHandle _DepthInput;
        private RTHandle _CameraColorTexture;
        private RTHandle _CameraDepthAttachment;
        private RTHandle _CameraDepthAttachmentTemp;

#if UNITY_2021_3_OR_NEWER
		readonly static FieldInfo CameraDepthTextureGetter = typeof(UniversalRenderer).GetField("m_DepthTexture", BindingFlags.Instance | BindingFlags.NonPublic);
#endif

#if !URP_13
        private int _CameraDepthTexture;
#endif

		private const string CopyDepthShaderName = "Hidden/ProPixelizer/SRP/BlitCopyDepth";
		private const string CopyMainTexAndDepthShaderName = "Hidden/ProPixelizer/SRP/BlitCopyMainTexAndDepth";

		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			base.OnCameraSetup(cmd, ref renderingData);
#if DISABLE_PROPIX_PREVIEW_WINDOW
			// Inspector window in 2022.2 is bugged with a null render target.
			// https://forum.unity.com/threads/preview-camera-does-not-call-setuprenderpasses.1377363/
			// https://forum.unity.com/threads/nullreferenceexception-in-2022-2-1f1-urp-14-0-4-when-rendering-the-inspector-preview-window.1377240/
			if (renderingData.cameraData.cameraType == CameraType.Preview)
				return;
#endif

#if BLIT_API
			ConfigureTarget(renderingData.cameraData.renderer.cameraColorTargetHandle);
				// possibly also depth - todo.
#endif
		}

		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{

			_ColorInput = RecompositionPass._Color;
			_DepthInput = RecompositionPass._Depth;

			var depthDescriptor = cameraTextureDescriptor;
            depthDescriptor.useMipMap = false;
            depthDescriptor.colorFormat = RenderTextureFormat.Depth;

#if URP_13
            _CameraColorTexture = colorAttachmentHandle;
            _CameraDepthAttachment = depthAttachmentHandle;
            RenderingUtils.ReAllocateIfNeeded(ref _CameraDepthAttachmentTemp, depthDescriptor, name: "ProP_CameraDepthAttachmentTemp");
#else
            _CameraColorTexture = RTHandles.Alloc("_CameraColorTexture", name: "_CameraColorTexture");
            _CameraDepthAttachment = RTHandles.Alloc("_CameraDepthAttachment", name: "_CameraDepthAttachment");
            _CameraDepthAttachmentTemp = RTHandles.Alloc("_CameraDepthAttachmentTemp", name: "_CameraDepthAttachmentTemp");
            _CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");

            cmd.GetTemporaryRT(Shader.PropertyToID(_CameraDepthAttachment.name), depthDescriptor);
            cmd.GetTemporaryRT(Shader.PropertyToID(_CameraDepthAttachmentTemp.name), depthDescriptor);
#endif
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

			CommandBuffer buffer = CommandBufferPool.Get(nameof(ProPixelizerFinalRecompositionPass));

			bool isOverlay = renderingData.cameraData.camera.GetUniversalAdditionalCameraData().renderType == CameraRenderType.Overlay;

			// Start copying ---------------

			// Copy depth into Color target so that transparents work in scene view tab
			buffer.SetGlobalTexture("_MainTex", _ColorInput);
			buffer.SetGlobalTexture("_SourceTex", _ColorInput); // so that it works for fallback pass for platforms that fail to compile BCMT&D
			buffer.SetGlobalTexture("_Depth", _DepthInput);
#if BLIT_API
            Blitter.BlitCameraTexture(buffer, _ColorInput, ColorTarget(ref renderingData));
            Blitter.BlitCameraTexture(buffer, _DepthInput, DepthTarget(ref renderingData), Materials.CopyDepth, 0);
#else
			SetColorAndDepthTargets(buffer, ref renderingData);
			buffer.SetViewMatrix(Matrix4x4.identity);
			buffer.SetProjectionMatrix(Matrix4x4.identity);
			buffer.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, Materials.CopyMainTexAndDepth);
#endif

			// Copy pixelated depth texture
#if BLIT_API
            Blitter.BlitCameraTexture(buffer, _DepthInput, _CameraDepthAttachmentTemp, Materials.CopyDepth, 0);
#else
			buffer.SetGlobalTexture("_MainTex", _DepthInput);
			buffer.SetRenderTarget(_CameraDepthAttachmentTemp);
			buffer.SetViewMatrix(Matrix4x4.identity);
			buffer.SetProjectionMatrix(Matrix4x4.identity);
			buffer.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, Materials.CopyDepth);
#endif

			// ...then restore transformations:
			ProPixelizerUtils.SetCameraMatrices(buffer, ref renderingData.cameraData);

#if URP_13
                //// Blit pixelised depth into the used depth texture
                /// (2022.1b and beyond)
                if (renderingData.cameraData.cameraType != CameraType.Preview)
                    Blit(buffer, _CameraDepthAttachmentTemp, DepthTarget(ref renderingData), Materials.CopyDepth);
#else
			// 2020 LTS: required to fix depth buffer in scene view.
			if (!isOverlay)
				Blit(buffer, _CameraDepthAttachmentTemp, _CameraDepthTexture, Materials.CopyDepth);
			Blit(buffer, _CameraDepthAttachmentTemp, _CameraDepthAttachment, Materials.CopyDepth);

			if (!isOverlay)
				buffer.SetGlobalTexture("_CameraDepthTexture", _CameraDepthTexture);
#endif

            // Modifying the scene tab's depth:
            // When URP renders the scene tab under UNITY_EDITOR, a final depth pass is enqued.
            // This pass applies CopyDepth of the m_DepthTexture into k_CameraTarget.
            // https://github.com/Unity-Technologies/Graphics/blob/1e4487f95a63937cd3a67d733bd26af31870c77d/Packages/com.unity.render-pipelines.universal/Runtime/UniversalRenderer.cs#L1289
            // So we do need to get m_DepthTexture...
			#if UNITY_2021_3_OR_NEWER
            if (renderingData.cameraData.cameraType == CameraType.SceneView || renderingData.cameraData.cameraType == CameraType.Preview)
			{
				var universalRenderer = renderingData.cameraData.renderer as UniversalRenderer;
				if (universalRenderer != null)
				{
#if UNITY_2021
                    var cameraDepthTexture = (RenderTargetHandle)CameraDepthTextureGetter.GetValue(universalRenderer);
#else
					var cameraDepthTexture = (RTHandle)CameraDepthTextureGetter.GetValue(universalRenderer);
#endif

                    if (!isOverlay && cameraDepthTexture != null)
					{
#if UNITY_2021
                        Blit(buffer, _CameraDepthAttachmentTemp.nameID, cameraDepthTexture.Identifier(), Materials.CopyDepth);
#else
						Blit(buffer, _CameraDepthAttachmentTemp, cameraDepthTexture, Materials.CopyDepth);
#endif

                    }
                }
            }
#endif

					// ...and restore transformations:
					ProPixelizerUtils.SetCameraMatrices(buffer, ref renderingData.cameraData);
			context.ExecuteCommandBuffer(buffer);
			CommandBufferPool.Release(buffer);
		}


#if URP_13
        public void Dispose()
        {
            _CameraDepthAttachmentTemp?.Release();
            //_CameraDepthAttachment?.Release(); // not needed?
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // set externally managed RTHandle references to null.
        }
#endif

		public override void FrameCleanup(CommandBuffer cmd)
		{
#if URP_13

#else
			cmd.ReleaseTemporaryRT(Shader.PropertyToID(_CameraDepthAttachment.name));
			cmd.ReleaseTemporaryRT(Shader.PropertyToID(_CameraDepthAttachmentTemp.name));
#endif
		}

            /// <summary>
            /// Shader resources used by the PixelizationPass.
            /// </summary>
            [Serializable]
		public sealed class ShaderResources
		{
			public Shader CopyDepth;
			public Shader CopyMainTexAndDepth;

			public ShaderResources Load()
			{
				CopyDepth = Shader.Find(CopyDepthShaderName);
				CopyMainTexAndDepth = Shader.Find(CopyMainTexAndDepthShaderName);
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
			private Material _CopyMainTexAndDepth;
			public Material CopyMainTexAndDepth
			{
				get
				{
					if (_CopyMainTexAndDepth == null)
						_CopyMainTexAndDepth = new Material(Resources.CopyMainTexAndDepth);
					return _CopyMainTexAndDepth;
				}
			}

			public MaterialLibrary(ShaderResources resources)
			{
				Resources = resources;
			}
		}
	}
}