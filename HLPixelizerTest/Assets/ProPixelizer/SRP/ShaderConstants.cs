// Copyright Elliot Bentine, 2018-
using UnityEngine.Rendering;

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

    public static class ProPixelizerOutlineKeywords
    {
        public const string DEPTH_TEST_OUTLINES_ON = "DEPTH_TEST_OUTLINES_ON";
        public const string DEPTH_TEST_NORMAL_EDGES_ON = "DEPTH_TEST_NORMAL_EDGES_ON";
        public const string NORMAL_EDGE_DETECTION_ON = "NORMAL_EDGE_DETECTION_ON";
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

        /// <summary>
        /// Indicates whether the scene is being rendered to the backbuffer.
        /// 
        /// On Dx platforms, drawing to the backbuffer results in a flip.
        /// When working with RTs, Unity magic prevents this from happening.
        /// 
        /// To account for this behavior, normally UNITY_UV_STARTS_FROM_TOP and _TextureName_TexelSize.y < 0 are used to
        /// check for (i) a platform that flips and (ii) that we are reading from a backbuffer.
        /// However, when drawing objects and sampling the outline buffer we find that we are in a situation
        /// where we may be drawing into a backbuffer and sampling an (unflipped) render texture.
        /// In this case, we can't rely on TexelSize.y because it will always be positive as the RT is never flipped.
        /// Instead, we set the BACKBUFFER_FLAG in these instances to indicate the outline buffer must be flipped
        /// to match the flip of the screen backbuffer.
        /// </summary>
        public const string BACKBUFFER_FLAG = "_ProPixelizer_BackBufferFlag";
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

    public static class ProPixelizerMatrices
    {
        public const string LOW_RES_VIEW_INVERSE = "_ProPixelizer_LowRes_I_V";
        public const string LOW_RES_PROJECTION_INVERSE = "_ProPixelizer_LowRes_I_P";
    }

    public static class ProPixelizerTargets
    {
        public const string PROPIXELIZER_LOWRES = "ProPixelizer_LowRes";
        public const string PROPIXELIZER_LOWRES_DEPTH = "ProPixelizer_LowRes_Depth";
        public const string OUTLINE_BUFFER = "_ProPixelizerOutlines";
        public const string PROPIXELIZER_METADATA_BUFFER = "ProPixelizer_Metadata";
        public const string PROPIXELIZER_METADATA_BUFFER_DEPTH = "ProPixelizer_Metadata_Depth";
        public const string PROPIXELIZER_PIXELIZATION_MAP = "_PixelizationMap";
        public const string PROPIXELIZER_RECOMPOSITION_COLOR = "ProPixelizer_Recomp_Color";
        public const string PROPIXELIZER_RECOMPOSITION_DEPTH = "ProPixelizer_Recomp_Depth";
        public const string PROPIXELIZER_FULLSCREEN_COLOR_GRADING = "ProPixelizer_FullScreenColorGrading";
    }
}