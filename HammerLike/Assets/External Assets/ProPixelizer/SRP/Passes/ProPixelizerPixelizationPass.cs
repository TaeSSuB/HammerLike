// Copyright Elliot Bentine, 2018-
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProPixelizer
{
    /// <summary>
    /// todo
    /// </summary>
    public class ProPixelizerPixelizationPass : ProPixelizerPass
    {
        public ProPixelizerPixelizationPass(
            ShaderResources shaders,
            ProPixelizerMetadataPass metadata,
            ProPixelizerLowResolutionDrawPass lowResolutionPass,
            ProPixelizerLowResolutionConfigurationPass configPass,
            ProPixelizerOutlineDetectionPass outlineDetectionPass,
            ProPixelizerLowResolutionTargetPass targetPass
            )
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            Materials = new MaterialLibrary(shaders);
            MetadataPass = metadata;
            LowResolutionPass = lowResolutionPass;
            ConfigPass = configPass;
            OutlineDetectionPass = outlineDetectionPass;
            TargetPass = targetPass;
        }

        private MaterialLibrary Materials;
        private ProPixelizerMetadataPass MetadataPass;
        private ProPixelizerLowResolutionDrawPass LowResolutionPass;
        private ProPixelizerLowResolutionConfigurationPass ConfigPass;
        private ProPixelizerOutlineDetectionPass OutlineDetectionPass;
        private ProPixelizerLowResolutionTargetPass TargetPass;

        /// <summary>
        /// Shader resources used by the PixelizationPass.
        /// </summary>
        [Serializable]
        public sealed class ShaderResources
        {
            public Shader PixelizationMap;
            public Shader CopyDepth;
            public Shader CopyMainTexAndDepth;
            public Shader ApplyPixelizationMap;

            public ShaderResources Load()
            {
                PixelizationMap = Shader.Find(PixelizationMapShaderName);
                CopyDepth = Shader.Find(CopyDepthShaderName);
                ApplyPixelizationMap = Shader.Find(ApplyPixelizationMapShaderName);
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

            private Material _PixelizationMap;
            public Material PixelizationMap
            {
                get
                {
                    if (_PixelizationMap == null)
                        _PixelizationMap = new Material(Resources.PixelizationMap);
                    return _PixelizationMap;
                }
            }
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
            private Material _ApplyPixelizationMap;
            public Material ApplyPixelizationMap
            {
                get
                {
                    if (_ApplyPixelizationMap == null)
                        _ApplyPixelizationMap = new Material(Resources.ApplyPixelizationMap);
                    return _ApplyPixelizationMap;
                }
            }

            public MaterialLibrary(ShaderResources resources)
            {
                Resources = resources;
            }
        }

        private RTHandle _PixelizationMap;
        private RTHandle _OriginalScene;
        private RTHandle _PixelatedScene;
        private RTHandle _PixelatedScene_Depth;
        private RTHandle _ProPixelizerOutlineObject;

#if BLIT_API
        private const string ApplyPixelMapDepthOutputKeywordString = "PIXELMAP_DEPTH_OUTPUT_ON";
        private GlobalKeyword ApplyPixelMapDepthOutputKeyword = GlobalKeyword.Create(ApplyPixelMapDepthOutputKeywordString);
#endif
        private const string CopyDepthShaderName = "Hidden/ProPixelizer/SRP/BlitCopyDepth";
        private const string CopyMainTexAndDepthShaderName = "Hidden/ProPixelizer/SRP/BlitCopyMainTexAndDepth";
        private const string PixelizationMapShaderName = "Hidden/ProPixelizer/SRP/Pixelization Map";
        private const string ApplyPixelizationMapShaderName = "Hidden/ProPixelizer/SRP/ApplyPixelizationMap";

        private Vector4 TexelSize;

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
            #endif
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cameraTextureDescriptor.useMipMap = false;
            cameraTextureDescriptor.width = ConfigPass.Width;
            cameraTextureDescriptor.height = ConfigPass.Height;

            var depthDescriptor = cameraTextureDescriptor;
            depthDescriptor.colorFormat = RenderTextureFormat.Depth;

            var pixelizationMapDescriptor = cameraTextureDescriptor;
            pixelizationMapDescriptor.colorFormat = RenderTextureFormat.ARGB32;
            pixelizationMapDescriptor.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;
            pixelizationMapDescriptor.depthBufferBits = 0;

#if URP_13
            RenderingUtils.ReAllocateIfNeeded(ref _PixelatedScene_Depth, cameraTextureDescriptor, name: "ProP_PixelatedScene_Depth", wrapMode: TextureWrapMode.Clamp);
            cameraTextureDescriptor.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref _PixelatedScene, cameraTextureDescriptor, name: "ProP_PixelatedScene");
            RenderingUtils.ReAllocateIfNeeded(ref _OriginalScene, cameraTextureDescriptor, name: "ProP_OriginalScene", wrapMode: TextureWrapMode.Clamp);
            _ProPixelizerOutlineObject = MetadataPass.MetadataObjectBuffer;
            RenderingUtils.ReAllocateIfNeeded(ref _PixelizationMap, pixelizationMapDescriptor, name: "ProP_PixelizationMap");
#else
            
            _PixelizationMap = RTHandles.Alloc("_PixelizationMap", name: "_PixelizationMap");
            _PixelatedScene = RTHandles.Alloc("_PixelatedScene", name: "_PixelatedScene");
            _PixelatedScene_Depth = RTHandles.Alloc("_PixelatedScene_Depth", name: "_PixelatedScene_Depth");
            _OriginalScene = RTHandles.Alloc("_OriginalScene", name: "_OriginalScene");
            _ProPixelizerOutlineObject = MetadataPass.MetadataObjectBuffer;

            var pixelatedTexDesc = cameraTextureDescriptor;
            pixelatedTexDesc.depthBufferBits = 0;
            cmd.GetTemporaryRT(Shader.PropertyToID(_PixelatedScene.name), pixelatedTexDesc);
            cmd.GetTemporaryRT(Shader.PropertyToID(_PixelatedScene_Depth.name), cameraTextureDescriptor.width, cameraTextureDescriptor.height, 32, FilterMode.Point, RenderTextureFormat.Depth);
            cmd.GetTemporaryRT(Shader.PropertyToID(_OriginalScene.name), cameraTextureDescriptor, FilterMode.Point);
            cmd.GetTemporaryRT(Shader.PropertyToID(_PixelizationMap.name), pixelizationMapDescriptor);
            
#endif

            TexelSize = new Vector4(
                1f / cameraTextureDescriptor.width,
                1f / cameraTextureDescriptor.height,
                cameraTextureDescriptor.width,
                cameraTextureDescriptor.height
            );
        }

        public const string PROFILER_TAG = "PIXELISATION";

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            #if DISABLE_PROPIX_PREVIEW_WINDOW
            // Inspector window in 2022.2 is bugged with a null render target.
            // https://forum.unity.com/threads/preview-camera-does-not-call-setuprenderpasses.1377363/
            // https://forum.unity.com/threads/nullreferenceexception-in-2022-2-1f1-urp-14-0-4-when-rendering-the-inspector-preview-window.1377240/
            if (renderingData.cameraData.cameraType == CameraType.Preview)
                return;
            #endif

            CommandBuffer buffer = CommandBufferPool.Get(PROFILER_TAG);
            buffer.name = "ProPixelizer Pixelisation";

            // Configure keywords for pixelising material.
            if (renderingData.cameraData.camera.orthographic)
                Materials.PixelizationMap.EnableKeyword("ORTHO_PROJECTION");
            else
                Materials.PixelizationMap.DisableKeyword("ORTHO_PROJECTION");

            if (renderingData.cameraData.camera.GetUniversalAdditionalCameraData().renderType == CameraRenderType.Overlay)
                Materials.PixelizationMap.EnableKeyword("OVERLAY_CAMERA"); 
            else
                Materials.PixelizationMap.DisableKeyword("OVERLAY_CAMERA");

            #if URP_13
                RTHandle ColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
                RTHandle cameraDepthTarget = renderingData.cameraData.renderer.cameraDepthTargetHandle;           
            #else
                RenderTargetIdentifier ColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
                RenderTargetIdentifier cameraDepthTarget = renderingData.cameraData.renderer.cameraDepthTarget;
            #endif

            // Blit scene into _OriginalScene - so that we can guarantee point filtering of colors.
            #if BLIT_API
                Blitter.BlitCameraTexture(buffer, TargetPass.Color, _OriginalScene);
            #else
                Blit(buffer, TargetPass.Color, _OriginalScene);
            #endif

            #if CAMERA_COLOR_TEX_PROP
                bool isOverlay = renderingData.cameraData.camera.GetUniversalAdditionalCameraData().renderType == CameraRenderType.Overlay;
            #else
                bool isOverlay = false;
            #endif

            // Create pixelization map, to determine how to pixelate the screen.
            buffer.SetGlobalTexture("_MainTex", _ProPixelizerOutlineObject);
            buffer.SetGlobalTexture("_SourceDepthTexture", MetadataPass.MetadataObjectBuffer_Depth, RenderTextureSubElement.Depth);
            #if URP_13
                buffer.SetGlobalTexture("_SceneDepthTexture", TargetPass.Depth);
            #else
                buffer.SetGlobalTexture("_SceneDepthTexture", TargetPass.Depth);
            #endif
            #if BLIT_API
                Blitter.BlitCameraTexture(buffer, _OriginalScene, _PixelizationMap, Materials.PixelizationMap, 0);
            #else
                Blit(buffer, _OriginalScene, _PixelizationMap, Materials.PixelizationMap);
            #endif


            // Pixelise the appearance texture
            // Note: For RTHandles we need to blit to a separate depth buffer.
            buffer.SetGlobalTexture("_MainTex", _OriginalScene);
            buffer.SetGlobalTexture("_PixelizationMap", _PixelizationMap);
#if BLIT_API
            // I would prefer to do this with local keywords, but they are broken w/buffers at least in 2022.2
            buffer.DisableKeyword(ApplyPixelMapDepthOutputKeyword);
            Blitter.BlitCameraTexture(buffer, _OriginalScene, _PixelatedScene, Materials.ApplyPixelizationMap, 0);
            // need to check which camera depth target to use here.
            buffer.EnableKeyword(ApplyPixelMapDepthOutputKeyword);
            Blitter.BlitCameraTexture(buffer, cameraDepthTarget, _PixelatedScene_Depth, Materials.ApplyPixelizationMap, 0);
#else
            buffer.SetRenderTarget(_PixelatedScene, (RenderTargetIdentifier)_PixelatedScene_Depth);
            buffer.SetViewMatrix(Matrix4x4.identity);
            buffer.SetProjectionMatrix(Matrix4x4.identity);
            buffer.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, Materials.ApplyPixelizationMap);
#endif

            // Blit Pixelised Color and Depth back into the low res target.
            buffer.SetGlobalTexture("_MainTex", _PixelatedScene);
            buffer.SetGlobalTexture("_SourceTex", _PixelatedScene); // so that it works for fallback pass for platforms that fail to compile BCMT&D
            buffer.SetGlobalTexture("_Depth", _PixelatedScene_Depth);
#if BLIT_API
            Blitter.BlitCameraTexture(buffer, _PixelatedScene, TargetPass.Color);
            Blitter.BlitCameraTexture(buffer, _PixelatedScene_Depth, TargetPass.Depth, Materials.CopyDepth, 0);
#else
            buffer.SetRenderTarget(TargetPass.Color, TargetPass.Depth);
            buffer.SetViewMatrix(Matrix4x4.identity);
            buffer.SetProjectionMatrix(Matrix4x4.identity);
            buffer.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, Materials.CopyMainTexAndDepth);
#endif

            // ...and restore transformations:
            ProPixelizerUtils.SetCameraMatrices(buffer, ref renderingData.cameraData);

            context.ExecuteCommandBuffer(buffer);
            CommandBufferPool.Release(buffer);
        }


#if URP_13
        public void Dispose()
        {
            _PixelatedScene?.Release();
            _PixelatedScene_Depth?.Release();
            _OriginalScene?.Release();
            _PixelizationMap?.Release();
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
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(_PixelizationMap.name));
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(_PixelatedScene.name));
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(_PixelatedScene_Depth.name));
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(_OriginalScene.name));
#endif
        }
    }
}