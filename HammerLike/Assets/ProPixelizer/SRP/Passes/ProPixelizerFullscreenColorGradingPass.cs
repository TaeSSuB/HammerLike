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
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProPixelizer
{
    /// <summary>
    /// Applies a color grading palette to the full screen.
    /// </summary>
    public class ProPixelizerFullscreenColorGradingPass : ProPixelizerPass
	{
		public ProPixelizerFullscreenColorGradingPass(ShaderResources shaders, ProPixelizerMetadataPass metadata)
		{
			renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
			Materials = new MaterialLibrary(shaders);
            _Metadata = metadata;
        }

		private MaterialLibrary Materials;
        private RTHandle _Color;
        private ProPixelizerMetadataPass _Metadata;

        /// <summary>
        /// Returns true if full screen color grading is active.
        /// </summary>
        public bool Active => Settings.FullscreenLUT != null;

		public const string COLOR_TARGET_NAME = "ProPixelizer_FullScreenColorGrading";

		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (Active)
                Materials.FullScreenColorGradeMaterial.SetTexture("_ColorGradingLUT", Settings.FullscreenLUT);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cameraTextureDescriptor.depthBufferBits = 0;
#if URP_13
            RenderingUtils.ReAllocateIfNeeded(ref _Color, cameraTextureDescriptor, name: COLOR_TARGET_NAME);
#else
            _Color = RTHandles.Alloc(COLOR_TARGET_NAME, name: COLOR_TARGET_NAME);
            cmd.GetTemporaryRT(Shader.PropertyToID(_Color.name), cameraTextureDescriptor, FilterMode.Point);
#endif
            ConfigureTarget(_Color);
            ConfigureClear(ClearFlag.None, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!Active) return;
            if (renderingData.cameraData.cameraType == CameraType.Preview)
                return;

            var colorTarget = ColorTarget(ref renderingData);
            if (colorTarget == null) return;

            CommandBuffer buffer = CommandBufferPool.Get(nameof(ProPixelizerFullscreenColorGradingPass));
            buffer.SetGlobalTexture("_ProPixelizer_Metadata", _Metadata.MetadataObjectBuffer);
#if BLIT_API
            Blitter.BlitCameraTexture(buffer, colorTarget, _Color, Materials.FullScreenColorGradeMaterial, 0);
#else
            buffer.SetGlobalTexture("_MainTex", colorTarget);
            buffer.SetViewMatrix(Matrix4x4.identity);
            buffer.SetProjectionMatrix(Matrix4x4.identity);
            buffer.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, Materials.FullScreenColorGradeMaterial);
#endif


#if BLIT_API
            Blitter.BlitCameraTexture(buffer, _Color, colorTarget);
#else
            buffer.SetGlobalTexture("_MainTex", _Color);
            buffer.SetViewMatrix(Matrix4x4.identity);
            buffer.SetProjectionMatrix(Matrix4x4.identity);
            buffer.SetRenderTarget(ColorTarget(ref renderingData));
            buffer.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, Materials.CopyMainTex);
#endif
            // Now blit the color back into scene color.
            context.ExecuteCommandBuffer(buffer);
            CommandBufferPool.Release(buffer);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
#if URP_13
#else
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(_Color.name));
#endif
        }

#if URP_13
        public void Dispose()
        {
            if (!Active) return;
            _Color?.Release();
        }
#endif


        /// <summary>
        /// Shader resources used by the PixelizationPass.
        /// </summary>
        [Serializable]
		public sealed class ShaderResources
		{
			public Shader FullScreenColorGrade;
            private const string FullScreenColorGradeName = "Hidden/ProPixelizer/SRP/FullscreenColorGrade";
            public Shader CopyMainTex;
            private const string CopyMainTexName = "Hidden/ProPixelizer/SRP/BlitCopyMainTex";

            public ShaderResources Load()
			{
                FullScreenColorGrade = Shader.Find(FullScreenColorGradeName);
                CopyMainTex = Shader.Find(CopyMainTexName);
                return this;
			}
		}

		/// <summary>
		/// Materials used by the PixelizationPass
		/// </summary>
		public sealed class MaterialLibrary
		{
			private ShaderResources Resources;

			private Material _FullScreenColorGradeMaterial;
			public Material FullScreenColorGradeMaterial
            {
				get
				{
					if (_FullScreenColorGradeMaterial == null)
                        _FullScreenColorGradeMaterial = new Material(Resources.FullScreenColorGrade);
					return _FullScreenColorGradeMaterial;
				}
			}

            private Material _CopyMainTex;
            public Material CopyMainTex
            {
                get
                {
                    if (_CopyMainTex == null)
                        _CopyMainTex = new Material(Resources.CopyMainTex);
                    return _CopyMainTex;
                }
            }

            public MaterialLibrary(ShaderResources resources)
			{
				Resources = resources;
			}
		}
	}
}