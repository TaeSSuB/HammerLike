// Copyright Elliot Bentine, 2018-
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProPixelizer
{
    /// <summary>
    /// Recomposes the scene by merging the low-res render target with the scene color and depth.
    /// Merging is performed through a depth comparison.
    /// </summary>
    public class ProPixelizerLowResRecompositionPass : ProPixelizerPass
    {
        public ProPixelizerLowResRecompositionPass(ShaderResources resources, ProPixelizerLowResolutionTargetPass targetPass)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            Materials = new MaterialLibrary(resources);
            TargetPass = targetPass;
        }
        private MaterialLibrary Materials;
        private ProPixelizerLowResolutionTargetPass TargetPass;

        public RTHandle _Color;
        public RTHandle _Depth;

        private const string DepthOutputKeywordString = "RECOMPOSITION_DEPTH_OUTPUT_ON";
#if BLIT_API
        private GlobalKeyword DepthOutputKeyword = GlobalKeyword.Create(DepthOutputKeywordString);
#endif

        public const string COLOR_TARGET_TEMP = "ProPixelizer_ColorTemp";
        public const string DEPTH_TARGET_TEMP = "ProPixelizer_DepthTemp";

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // TODO: may need to retrieve a different cameraTextureDescriptor, if 'full screen' post process has
            // already modified the camera texture to a tiny size for pixelisation.
            var colorDescriptor = cameraTextureDescriptor;
            var depthDescriptor = cameraTextureDescriptor;
            depthDescriptor.colorFormat = RenderTextureFormat.Depth;
#if URP_13
            colorDescriptor.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref _Color, colorDescriptor, name: COLOR_TARGET_TEMP);
            RenderingUtils.ReAllocateIfNeeded(ref _Depth, depthDescriptor, name: DEPTH_TARGET_TEMP);
#else
            _Color = RTHandles.Alloc(COLOR_TARGET_TEMP, name: COLOR_TARGET_TEMP);
            _Depth = RTHandles.Alloc(DEPTH_TARGET_TEMP, name: DEPTH_TARGET_TEMP);
            cmd.GetTemporaryRT(Shader.PropertyToID(_Color.name), colorDescriptor, FilterMode.Point);
            cmd.GetTemporaryRT(Shader.PropertyToID(_Depth.name), depthDescriptor, FilterMode.Point);
#endif
            ConfigureTarget(_Color, _Depth);
            ConfigureClear(ClearFlag.None, Color.black);
        }
        public override void FrameCleanup(CommandBuffer cmd)
        {
#if URP_13
#else
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(_Color.name));
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(_Depth.name));
#endif
        }

#if URP_13
        public void Dispose()
        {
            _Color?.Release();
            _Depth?.Release();
        }
#endif

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer buffer = CommandBufferPool.Get(nameof(ProPixelizerLowResRecompositionPass));

            // Blit current color and depth target into the low res targets.
            buffer.SetGlobalTexture("_MainTex", ColorTarget(ref renderingData));
#if UNITY_2022_2_OR_NEWER
            if (renderingData.cameraData.renderer.cameraDepthTargetHandle.nameID == BuiltinRenderTextureType.CameraTarget)
#else
            if (renderingData.cameraData.renderer.cameraDepthTarget == BuiltinRenderTextureType.CameraTarget)
#endif
                buffer.SetGlobalTexture("_Depth", ColorTarget(ref renderingData), RenderTextureSubElement.Depth);
            else
                buffer.SetGlobalTexture("_Depth", DepthTarget(ref renderingData), RenderTextureSubElement.Depth);
            buffer.SetGlobalTexture("_Secondary", TargetPass.Color);
            buffer.SetGlobalTexture("_SecondaryDepth", TargetPass.Depth);
#if BLIT_API
            Blitter.BlitCameraTexture(buffer, TargetPass.Color, _Color, Materials.DepthBasedComposition, 0);
            buffer.EnableKeyword(DepthOutputKeyword);
            Blitter.BlitCameraTexture(buffer, TargetPass.Depth, _Depth, Materials.DepthBasedComposition, 0);
            buffer.DisableKeyword(DepthOutputKeyword);
#else
            buffer.EnableShaderKeyword(DepthOutputKeywordString);
            buffer.SetViewMatrix(Matrix4x4.identity);
            buffer.SetProjectionMatrix(Matrix4x4.identity);
            buffer.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, Materials.DepthBasedComposition);
            buffer.DisableShaderKeyword(DepthOutputKeywordString);
#endif
            context.ExecuteCommandBuffer(buffer);
            CommandBufferPool.Release(buffer);
        }


        /// <summary>
        /// Shader resources used by this pass.
        /// </summary>
        [Serializable]
        public sealed class ShaderResources
        {
            private const string CopyDepthShaderName = "Hidden/ProPixelizer/SRP/BlitCopyDepth";
            private const string CopyMainTexAndDepthShaderName = "Hidden/ProPixelizer/SRP/BlitCopyMainTexAndDepth";
            private const string ProPixelizerTargetSceneRecompositionShaderName = "Hidden/ProPixelizer/SRP/Internal/ProPixelizerTargetSceneRecomposition";
            public Shader CopyDepth;
            public Shader CopyMainTexAndDepth;
            public Shader ProPixelizerTargetSceneRecomposition;

            public ShaderResources Load()
            {
                CopyDepth = Shader.Find(CopyDepthShaderName);
                CopyMainTexAndDepth = Shader.Find(CopyMainTexAndDepthShaderName);
                ProPixelizerTargetSceneRecomposition = Shader.Find(ProPixelizerTargetSceneRecompositionShaderName);
                return this;
            }
        }

        /// <summary>
        /// Materials used by this pass.
        /// </summary>
        public sealed class MaterialLibrary
        {
            private ShaderResources Resources;
            public Material CopyDepth
            {
                get
                {
                    if (_CopyDepth == null)
                        _CopyDepth = new Material(Resources.CopyDepth);
                    return _CopyDepth;
                }
            }
            private Material _CopyDepth;

            
            public Material CopyMainTexAndDepth
            {
                get
                {
                    if (_CopyMainTexAndDepth == null)
                        _CopyMainTexAndDepth = new Material(Resources.CopyMainTexAndDepth);
                    return _CopyMainTexAndDepth;
                }
            }
            private Material _CopyMainTexAndDepth;

            public Material DepthBasedComposition
            {
                get
                {
                    if (_DepthBasedComposition == null)
                        _DepthBasedComposition = new Material(Resources.ProPixelizerTargetSceneRecomposition);
                    return _DepthBasedComposition;
                }
            }
            private Material _DepthBasedComposition;

            public MaterialLibrary(ShaderResources resources)
            {
                Resources = resources;
            }
        }
    }
}