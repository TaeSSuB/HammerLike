// Copyright Elliot Bentine, 2018-
namespace ProPixelizer
{
    public static class ProPixelizerKeywords
    {
        /// <summary>
        /// ProPixelizer's pixel expansion mode is active.
        /// </summary>
        public const string PIXEL_EXPANSION = "PROPIXELIZER_PIXEL_EXPANSION";
        
        /// <summary>
        /// ProPixelizer has the Pixelisation Filter set to Full Scene.
        /// </summary>
        public const string FULL_SCENE = "PROPIXELIZER_FULL_SCENE";

        /// <summary>
        /// Determines whether a ProPixelizer material uses an externally specified Pixel Grid Origin.
        /// </summary>
        public const string USE_OBJECT_POSITION_FOR_PIXEL_GRID_ON = "USE_OBJECT_POSITION_FOR_PIXEL_GRID_ON";

        /// <summary>
        /// Determines whether a ProPixelizer material uses color grading.
        /// </summary>
        public const string COLOR_GRADING = "COLOR_GRADING";

        /// <summary>
        /// Determines whether a ProPixelizer material uses dithering for alpha transparency.
        /// </summary>
        public const string PROPIXELIZER_DITHERING = "PROPIXELIZER_DITHERING";

        /// <summary>
        /// Determines whether a ProPixelizer material receives shadows.
        /// </summary>
        public const string RECEIVE_SHADOWS = "RECEIVE_SHADOWS";
    }

    public static class ProPixelizerFloats
    {
        /// <summary>
        /// Value of 1 enables rendering for ProPixelizer materials, 0 disables.
        /// </summary>
        public const string PROPIXELIZER_PASS = "_ProPixelizer_Pass";

        /// <summary>
        /// Multiplier used to control global pixel scale.
        /// </summary>
        public const string PIXEL_SCALE = "_ProPixelizer_Pixel_Scale";
    }

    public static class ProPixelizerVec4s
    {
        /// <summary>
        /// Information about the ProPixelizer low-resolution target.
        /// x: width, y: height, zw: rt handle scale factors.
        /// </summary>
        public const string RENDER_TARGET_INFO = "_ProPixelizer_RenderTargetInfo";

        /// <summary>
        /// Information about the screen target.
        /// x: width, y: height, zw: rt handle scale factors.
        /// </summary>
        public const string SCREEN_TARGET_INFO = "_ProPixelizer_ScreenTargetInfo";

        /// <summary>
        /// Orthographic projection sizes.
        /// x/y: orthographic width and height of the low-res view.
        /// z/w: orthographic width and height of the camera view.
        /// </summary>
        public const string ORTHO_SIZES = "_ProPixelizer_OrthoSizes";

        public const string LOW_RES_CAMERA_DELTA_UV = "_ProPixelizer_LowResCameraDeltaUV";
    }
}