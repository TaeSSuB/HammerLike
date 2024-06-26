// Copyright Elliot Bentine, 2018-
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
    /// The recomposition pass is used to merge the low-resolution render of the scene with the high-resolution scene.
    /// The low-resolution texture can either be from a subcamera, or from a ProPixelizer low-resolution target.
    /// 
    /// Recomposition can be made by a few strategies. These include:
    /// - a depth-based merge by comparing the depth buffers of the two targets and selecting the colors which are nearer.
    /// - one of the targets is taken completely. This is _not_ a blit, because we need to map the low-res target into the high
    ///   resolution scene to account for sub-pixel camera movement.
    /// </summary>
    public class ProPixelizerLowResRecompositionPass : ProPixelizerPass
    {
        public ProPixelizerLowResRecompositionPass(ShaderResources resources, ProPixelizerLowResolutionTargetPass targetPass)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            Materials = new MaterialLibrary(resources);
            TargetPass = targetPass;
            RecompositionDepthBased = GlobalKeyword.Create(RECOMPOSITION_DEPTH_BASED);
            RecompositionOnlySecondary = GlobalKeyword.Create(RECOMPOSITION_ONLY_SECONDARY);
            DepthOutputOn = GlobalKeyword.Create(DEPTH_OUTPUT_ON);
        }
        private const string RECOMPOSITION_DEPTH_BASED = "RECOMPOSITION_DEPTH_BASED";
        private const string RECOMPOSITION_ONLY_SECONDARY = "RECOMPOSITION_ONLY_SECONDARY";
        private const string DEPTH_OUTPUT_ON = "DEPTH_OUTPUT_ON";
        private readonly GlobalKeyword RecompositionDepthBased;
        private readonly GlobalKeyword RecompositionOnlySecondary;
        private readonly GlobalKeyword DepthOutputOn;
        readonly ProfilingSampler _ProfilingSampler = new ProfilingSampler(nameof(ProPixelizerLowResRecompositionPass));
        private MaterialLibrary Materials;

        #region non-RG
        private ProPixelizerLowResolutionTargetPass TargetPass;
        public RTHandle _Color;
        public RTHandle _Depth;

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var colorDescriptor = cameraTextureDescriptor;
            var depthDescriptor = cameraTextureDescriptor;
            depthDescriptor.colorFormat = RenderTextureFormat.Depth;
            colorDescriptor.depthBufferBits = 0;
#pragma warning disable 0618
            RenderingUtils.ReAllocateIfNeeded(ref _Color, colorDescriptor, name: ProPixelizerTargets.PROPIXELIZER_RECOMPOSITION_COLOR);
            RenderingUtils.ReAllocateIfNeeded(ref _Depth, depthDescriptor, name: ProPixelizerTargets.PROPIXELIZER_RECOMPOSITION_DEPTH);
            ConfigureTarget(_Color, _Depth);
            ConfigureClear(ClearFlag.None, Color.black);
#pragma warning restore 0618
        }

        public void Dispose()
        {
            _Color?.Release();
            _Depth?.Release();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Preview)
                return;

            // Blit current color and depth target into the low res targets.
            var useColorTargetForDepth = (renderingData.cameraData.renderer.cameraDepthTargetHandle.nameID == BuiltinRenderTextureType.CameraTarget);
            var depthTarget = useColorTargetForDepth ? ColorTarget(ref renderingData) : DepthTarget(ref renderingData);
            var colorTarget = ColorTarget(ref renderingData);

            // Decide target to use as source
            RTHandle sourceColor, sourceDepth;
            switch (Settings.RecompositionSource)
            {
                case RecompositionLowResSource.ProPixelizerColorTarget:
                default:
                    sourceColor = TargetPass.Color;
                    sourceDepth = TargetPass.Depth;
                    break;
                case RecompositionLowResSource.SubCamera:
                    var subcamera = renderingData.cameraData.camera.GetComponent<ProPixelizerCamera>().SubCamera;
                    if (subcamera == null)
                        return;
                    sourceColor = subcamera.Color;
                    sourceDepth = subcamera.Depth;
                    break;
            }

            CommandBuffer buffer = CommandBufferPool.Get(GetType().Name);
            ExecutePass(
                buffer,
                Materials.SceneRecomposition,
                sourceColor,
                sourceDepth,
                colorTarget,
                depthTarget,
                _Color,
                _Depth,
                Settings.RecompositionMode,
                RecompositionDepthBased,
                RecompositionOnlySecondary,
                DepthOutputOn
                );

            context.ExecuteCommandBuffer(buffer);
            CommandBufferPool.Release(buffer);
        }
        #endregion

        #region RG
#if UNITY_2023_3_OR_NEWER

        class PassData
        {
            public Material depthComposition;
            public TextureHandle lowResColor;
            public TextureHandle lowResDepth;
            public TextureHandle cameraColor;
            public TextureHandle cameraDepth;
            public TextureHandle colorOutput;
            public TextureHandle depthOutput;
            public RecompositionMode mode;
            public GlobalKeyword recompositionDepthKeyword;
            public GlobalKeyword recompositionOnlySecondaryKeyword;
            public GlobalKeyword depthOutputKeyword;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var lowResTarget = frameData.Get<LowResTargetData>();
            var resources = frameData.Get<UniversalResourceData>();
            var recomposed = frameData.GetOrCreate<ProPixelizerRecompositionData>();
            var cameraData = frameData.Get<UniversalCameraData>();

            var colorDescriptor = cameraData.cameraTargetDescriptor;
            colorDescriptor.depthBufferBits = 0;
            var depthDescriptor = cameraData.cameraTargetDescriptor;
            depthDescriptor.colorFormat = RenderTextureFormat.Depth;

            recomposed.Color = UniversalRenderer.CreateRenderGraphTexture(
                renderGraph,
                colorDescriptor,
                ProPixelizerTargets.PROPIXELIZER_RECOMPOSITION_COLOR,
                false
                );
            recomposed.Depth = UniversalRenderer.CreateRenderGraphTexture(
                renderGraph,
                depthDescriptor,
                ProPixelizerTargets.PROPIXELIZER_RECOMPOSITION_DEPTH,
                false
                );

            using (var builder = renderGraph.AddUnsafePass<PassData>(GetType().Name, out var passData))
            {
                passData.depthComposition = Materials.DepthBasedComposition;
                passData.lowResColor = lowResTarget.Color;
                passData.lowResDepth = lowResTarget.Depth;
                passData.cameraColor = CameraColorTexture(resources, cameraData);
                passData.cameraDepth = CameraDepthTexture(resources, cameraData);
                passData.colorOutput = recomposed.Color;
                passData.depthOutput = recomposed.Depth;
                passData.mode = Settings.RecompositionMode;
                passData.recompositionDepthKeyword = RecompositionDepthBased;
                passData.recompositionOnlySecondaryKeyword = RecompositionOnlySecondary;
                passData.depthOutputKeyword = DepthOutputKeyword;

                builder.AllowGlobalStateModification(true);
                builder.AllowPassCulling(false);
                builder.UseTexture(passData.lowResColor, AccessFlags.Read);
                builder.UseTexture(passData.lowResDepth, AccessFlags.Read);
                builder.UseTexture(passData.cameraColor, AccessFlags.Read);
                builder.UseTexture(passData.cameraDepth, AccessFlags.Read);
                builder.UseTexture(passData.colorOutput, AccessFlags.Write);
                builder.UseTexture(passData.depthOutput, AccessFlags.Write);
                builder.SetRenderFunc((PassData data, UnsafeGraphContext context) =>
                {
                    ExecutePass(
                        CommandBufferHelpers.GetNativeCommandBuffer(context.cmd),
                        data.depthComposition,
                        data.lowResColor,
                        data.lowResDepth,
                        data.cameraColor,
                        data.cameraDepth,
                        data.colorOutput,
                        data.depthOutput,
                        data.recompositionDepthKeyword,
                        data.recompositionOnlySecondaryKeyword,
                        data.depthOutputKeyword,
                        );
                });
            }
        }

#endif
        #endregion

        public static void ExecutePass(
            CommandBuffer command,
            Material sceneRecompositionMaterial,
            RTHandle lowResColor,
            RTHandle lowResDepth,
            RTHandle cameraColor,
            RTHandle cameraDepth,
            RTHandle colorOutput,
            RTHandle depthOutput,
            RecompositionMode mode,
            GlobalKeyword recompositionDepthKeyword,
            GlobalKeyword recompositionOnlySecondaryKeyword,
            GlobalKeyword depthOutputKeyword
            )
        {
            command.SetGlobalTexture("_InputDepthTexture", cameraDepth, RenderTextureSubElement.Depth);
            command.SetGlobalTexture("_SecondaryTexture", lowResColor);
            command.SetGlobalTexture("_SecondaryDepthTexture", lowResDepth);
            switch (mode) {
                case RecompositionMode.DepthBasedMerge:
                    command.SetKeyword(recompositionDepthKeyword, true);
                    command.SetKeyword(recompositionOnlySecondaryKeyword, false);
                    break;
                default:
                    command.SetKeyword(recompositionDepthKeyword, false);
                    command.SetKeyword(recompositionOnlySecondaryKeyword, true);
                    break;
            }
            command.SetGlobalTexture("_Depth", cameraDepth, RenderTextureSubElement.Depth);
            Blitter.BlitCameraTexture(command, cameraColor, colorOutput, sceneRecompositionMaterial, 0);
            command.EnableKeyword(depthOutputKeyword);
            Blitter.BlitCameraTexture(command, cameraDepth, depthOutput, sceneRecompositionMaterial, 0);
            command.DisableKeyword(depthOutputKeyword);
        }



        /// <summary>
        /// Shader resources used by this pass.
        /// </summary>
        [Serializable]
        public sealed class ShaderResources
        {
            private readonly string SceneRecompositionShaderName = "Hidden/ProPixelizer/SRP/Internal/SceneRecomposition";
            public Shader SceneRecomposition;

            public ShaderResources Load()
            {
                SceneRecomposition = Shader.Find(SceneRecompositionShaderName);
                return this;
            }
        }

        /// <summary>
        /// Materials used by this pass.
        /// </summary>
        public sealed class MaterialLibrary
        {
            private ShaderResources Resources;
            public Material SceneRecomposition
            {
                get
                {
                    if (_SceneRecomposition == null)
                        _SceneRecomposition = new Material(Resources.SceneRecomposition);
                    return _SceneRecomposition;
                }
            }
            private Material _SceneRecomposition;

            public MaterialLibrary(ShaderResources resources)
            {
                Resources = resources;
            }
        }
    }
}