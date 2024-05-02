// Copyright Elliot Bentine, 2018-
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProPixelizer
{
    /// <summary>
    /// A set of rendering utility functions to help me implement functionality in a Unity Engine Version agnostic way.
    /// </summary>
    public static class ProPixelizerUtils {

        public static void SetCameraMatrices(CommandBuffer cmd, ref CameraData cameraData)
        {
#if CAMERADATA_MATRICES
            cmd.SetViewMatrix(cameraData.GetViewMatrix());
            cmd.SetProjectionMatrix(cameraData.GetProjectionMatrix());
#else
            cmd.SetViewMatrix(cameraData.camera.worldToCameraMatrix);
            cmd.SetProjectionMatrix(cameraData.camera.projectionMatrix);
#endif
        }

        /// <summary>
        /// Size of the margin around the visible low-res pixel target, in pixels.
        /// </summary>
        public const int LOW_RESOLUTION_TARGET_MARGIN = 5;

        /// <summary>
        /// Calculates a low resolution target size
        /// </summary>
        /// <param name="cameraWidth">width of the camera render texture in pixels</param>
        /// <param name="cameraHeight">height of the camera render texture in pixels</param>
        /// <param name="scale">resolution scale of the low res target. Values < 1 are pixelated.</param>
        /// <param name="width">output width in pixels</param>
        /// <param name="height">output height in pixels</param>
        public static void CalculateLowResTargetSize(int cameraWidth, int cameraHeight, float scale, out int width, out int height, bool withMargin = true)
        {
            // see matching define in ScreenUtils.hlsl
            width = (int)Mathf.Round(cameraWidth * scale);// + 2;
            height = (int)Mathf.Round(cameraHeight * scale);// + 2;
            //force odd dimensions by /2*2+1
            width = 2 * (width / 2) + 1 + (withMargin ? 2 * LOW_RESOLUTION_TARGET_MARGIN : 0);
            height = 2 * (height / 2) + 1 + (withMargin ? 2 * LOW_RESOLUTION_TARGET_MARGIN : 0);

            width = Mathf.Min(width, cameraWidth + 1 + (withMargin ? 2 * LOW_RESOLUTION_TARGET_MARGIN : 0));
            height = Mathf.Min(height, cameraHeight + 1 + (withMargin ? 2 * LOW_RESOLUTION_TARGET_MARGIN : 0));
        }

        public const string USER_GUIDE_URL = "https://sites.google.com/view/propixelizer/user-guide";
    }
}