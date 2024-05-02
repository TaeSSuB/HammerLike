// Copyright Elliot Bentine, 2018-
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProPixelizer
{
    /// <summary>
    /// Abstract base for ProPixelizer passes
    /// </summary>
    public abstract class ProPixelizerPass : ScriptableRenderPass
    {
        public ProPixelizerPass()
        {
        }

        public ProPixelizerSettings Settings { get; private set; }

        public void ConfigureForSettings(ProPixelizerSettings settings)
        {
            Settings = settings;
        }

        public void SetColorAndDepthTargets(CommandBuffer buffer, ref RenderingData renderingData) {
            // Note that for 2020 and 2021, the cameraDepthTarget only works for the Game tab, and not the Scene tab camera. Instead, you have to
            // blit to the depth element of the ColorTarget, which works for Scene view.
            // HOWEVER, the issue is that this element doesn't exist for the Metal API, and so an error gets thrown. It seems there is no way
            // to blit to the scene tab's depth buffer for Metal on 2020/2021.
            // 
            // https://forum.unity.com/threads/scriptablerenderpasses-cannot-consistently-access-the-depth-buffer.1263044/
            //var device = SystemInfo.graphicsDeviceType;
            //if (device == GraphicsDeviceType.Metal)
            //{
            //	buffer.SetRenderTarget(ColorTarget(ref renderingData), DepthTarget(ref renderingData));
            //}
            //else
            //{
            //	buffer.SetRenderTarget(ColorTarget(ref renderingData)); //use color target depth buffer.
            //}
            //
            // Solution may be below - still to check:
#if UNITY_2022_3_OR_NEWER
            // https://github.com/Unity-Technologies/Graphics/blob/008fceea35df3bd1fd61878ae560d50de04b0b8d/Packages/com.unity.render-pipelines.universal/Runtime/ScriptableRenderer.cs#L2024C17-L2024C80
            if (renderingData.cameraData.renderer.cameraDepthTargetHandle.nameID == BuiltinRenderTextureType.CameraTarget)
                buffer.SetRenderTarget(renderingData.cameraData.renderer.cameraColorTargetHandle, renderingData.cameraData.renderer.cameraColorTargetHandle); // color both
            else
                buffer.SetRenderTarget(ColorTarget(ref renderingData), DepthTarget(ref renderingData));
#else
            if (renderingData.cameraData.renderer.cameraDepthTarget == BuiltinRenderTextureType.CameraTarget)
                //buffer.SetRenderTarget(ColorTarget(ref renderingData), ColorTarget(ref renderingData)); // color both
                buffer.SetRenderTarget(ColorTarget(ref renderingData)); // color both
            else
                buffer.SetRenderTarget(ColorTarget(ref renderingData), DepthTarget(ref renderingData));
#endif
        }

        /// <summary>
        /// Sets view and projection matrices used to render to the low-res target.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="renderingData"></param>
        /// <param name="OrthoWidth"></param>
        /// <param name="OrthoHeight"></param>
        public void SetLowResViewProjectionMatrices(CommandBuffer cmd, ref RenderingData renderingData, float OrthoWidth, float OrthoHeight, Vector3 LowResCameraDeltaWS)
        {
            Matrix4x4 projectionMatrix;
            if (renderingData.cameraData.camera.orthographic)
            {
                var camera = renderingData.cameraData.camera;
                projectionMatrix = Matrix4x4.Ortho(
                    -OrthoWidth / 2f, OrthoWidth / 2f,
                    -OrthoHeight / 2f, OrthoHeight / 2f,
                    camera.nearClipPlane, camera.farClipPlane
                    );
            } else
            {
#if CAMERADATA_MATRICES
                projectionMatrix = renderingData.cameraData.GetProjectionMatrix();
#else
                projectionMatrix = renderingData.cameraData.camera.projectionMatrix;
#endif
            }
#if CAMERADATA_MATRICES
            var viewMatrix = renderingData.cameraData.GetViewMatrix();
#else
            var viewMatrix = renderingData.cameraData.camera.worldToCameraMatrix;
#endif
            viewMatrix = viewMatrix * Matrix4x4.Translate(LowResCameraDeltaWS);
#if CAMERADATA_MATRICES
            cmd.SetViewMatrix(viewMatrix);
            cmd.SetProjectionMatrix(projectionMatrix);
#else
            cmd.SetViewMatrix(viewMatrix);
            cmd.SetProjectionMatrix(projectionMatrix);
#endif
            cmd.SetGlobalMatrix("_ProPixelizer_LowRes_I_V", viewMatrix.inverse);
            cmd.SetGlobalMatrix("_ProPixelizer_LowRes_I_P", projectionMatrix.inverse);
        }


#if URP_13
        public RTHandle ColorTarget(ref RenderingData data) => data.cameraData.renderer.cameraColorTargetHandle;
        public RTHandle DepthTarget(ref RenderingData data) => data.cameraData.renderer.cameraDepthTargetHandle;         
#else
        public RenderTargetIdentifier ColorTarget(ref RenderingData data) => data.cameraData.renderer.cameraColorTarget;
        public RenderTargetIdentifier DepthTarget(ref RenderingData data) => data.cameraData.renderer.cameraDepthTarget;
#endif

        private const string PROPIXELIZER_METADATA_LIGHTMODE = "ProPixelizer";
        
        protected static ShaderTagId ProPixelizerMetadataLightmodeShaderTag = new ShaderTagId(PROPIXELIZER_METADATA_LIGHTMODE);

        public void SetPixelGlobalScale(CommandBuffer buffer, float scale)
        {
            buffer.SetGlobalFloat(ProPixelizerFloats.PIXEL_SCALE, scale);
        }

    }
}