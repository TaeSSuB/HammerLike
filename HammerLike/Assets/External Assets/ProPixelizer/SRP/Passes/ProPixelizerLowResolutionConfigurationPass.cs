// Copyright Elliot Bentine, 2018-
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProPixelizer
{
    /// <summary>
    /// Configures the ProPixelizer low resolution target.
    /// </summary>
    public class ProPixelizerLowResolutionConfigurationPass : ProPixelizerPass
    {
        public ProPixelizerLowResolutionConfigurationPass() : base() 
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        }

        public LowResolutionMask Mask;

        public int Width { get; private set; }
        public int Height { get; private set; }

        /// <summary>
        /// Full span of the orthographic projection's width, in world-space units.
        /// </summary>
        public float OrthoLowResWidth { get; private set; }

        /// <summary>
        /// Full span of the orthographic projection's height, in world-space units.
        /// </summary>
        public float OrthoLowResHeight { get; private set; }

        public float CameraOrthoHeight { get; private set; }

        public float CameraOrthoWidth {  get; private set; }

        /// <summary>
        /// Offset of the low-resolution camera relative to the high-res screen camera, in units of low-res uv coordinates.
        /// </summary>
        public Vector2 LowResCameraDeltaUV { get; private set; }

        public Vector3 LowResCameraDeltaWS { get; private set; }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Store the render target's size in a global variable.
            // This is required because Unity's RenderScale is not reliable, see
            // e.g. https://forum.unity.com/threads/_scaledscreenparameters-and-render-target-subregion.1336277/
            // 
            // The _ProPixelizer_RenderTargetSize variable is defined in the following way:
            // * x: low res target px width
            // * y: low res target px height
            // * z: fraction of target render region width over render target width.
            // * w: fraction of target render region height over render target height.
            // 
            // The _ProPixelizer_ScreenTargetSize variable is defined in the following way:
            // * x: screen target px width
            // * y: screen target px height
            // * z: fraction of target render region width over render target width.
            // * w: fraction of target render region height over render target height.
#if URP_13
            var handleProps = RTHandles.rtHandleProperties;
            cmd.SetGlobalVector(ProPixelizerVec4s.RENDER_TARGET_INFO, new Vector4(Width, Height, handleProps.rtHandleScale.x, handleProps.rtHandleScale.y));
            cmd.SetGlobalVector(ProPixelizerVec4s.SCREEN_TARGET_INFO, new Vector4(cameraTextureDescriptor.width, cameraTextureDescriptor.height, handleProps.rtHandleScale.x, handleProps.rtHandleScale.y));
#else
            cmd.SetGlobalVector(ProPixelizerVec4s.RENDER_TARGET_INFO, new Vector4(Width, Height, 1.0f, 1.0f));
            cmd.SetGlobalVector(ProPixelizerVec4s.SCREEN_TARGET_INFO, new Vector4(cameraTextureDescriptor.width, cameraTextureDescriptor.height, 1.0f, 1.0f));
#endif
            cmd.SetGlobalVector(ProPixelizerVec4s.ORTHO_SIZES, new Vector4(OrthoLowResWidth, OrthoLowResHeight, CameraOrthoWidth, CameraOrthoHeight));
            cmd.SetGlobalVector(ProPixelizerVec4s.LOW_RES_CAMERA_DELTA_UV, new Vector4(LowResCameraDeltaUV.x, LowResCameraDeltaUV.y, 0f, 0f));
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;

            ProPixelizerCamera cameraSnap = null;
            if (renderingData.cameraData.camera != null)
            {
                cameraSnap = renderingData.cameraData.camera.GetComponent<ProPixelizerCamera>();
            }

            if (cameraSnap != null && cameraSnap.LowResTargetHeight > 0 && cameraSnap.LowResTargetWidth > 0) 
                // width and height can be zero in editor mode, if the camera update has not run.
            {
                Width = cameraSnap.LowResTargetWidth;
                Height = cameraSnap.LowResTargetHeight;
                OrthoLowResWidth = cameraSnap.OrthoLowResWidth;
                OrthoLowResHeight = cameraSnap.OrthoLowResHeight;
                LowResCameraDeltaUV = cameraSnap.LowResCameraDeltaUV;
                LowResCameraDeltaWS = cameraSnap.LowResCameraDeltaWS;
            } else {
                ProPixelizerUtils.CalculateLowResTargetSize(descriptor.width, descriptor.height, Settings.DefaultLowResScale, out int width, out int height, withMargin: false);
                // Don't use padding if a snapped camera can't be found.

                Width = width;
                Height = height;
                if (renderingData.cameraData.camera != null && renderingData.cameraData.camera.orthographic)
                {
                    OrthoLowResHeight = 2.0f * renderingData.cameraData.camera.orthographicSize;
                    OrthoLowResWidth = OrthoLowResHeight / Height * Width;
                }
                LowResCameraDeltaWS = new Vector3();
            }
            // Store sizes of the unpixelated view.
            CameraOrthoHeight = 2.0f * renderingData.cameraData.camera.orthographicSize;
            CameraOrthoWidth = CameraOrthoHeight * renderingData.cameraData.cameraTargetDescriptor.width / renderingData.cameraData.cameraTargetDescriptor.height;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // nothing
        }
    }
}