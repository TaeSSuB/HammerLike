#if UNITY_2023_3_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif
using UnityEngine.Rendering;
using UnityEngine;
using static ProPixelizer.ProPixelizerLowResolutionConfigurationPass;


namespace ProPixelizer.RenderGraphResources
{
#if UNITY_2023_3_OR_NEWER
    /// <summary>
    /// The low-resolution target in ProPixelizer
    /// </summary>
    public class LowResTargetData : ContextItem
    {
        public TextureHandle Color;
        public TextureHandle Depth;

        public override void Reset()
        {
        }

        /// <summary>
        /// Creates a render texture descriptor for the low resolution target, given the input camera texture.
        /// </summary>
        public RenderTextureDescriptor ColorDescriptor(RenderTextureDescriptor cameraTexture, 
        config)
        {
            var colorDescriptor = cameraTexture;
            colorDescriptor.useMipMap = false;
            colorDescriptor.width = config.Width;
            colorDescriptor.height = config.Height;
            colorDescriptor.depthBufferBits = 0;
            return colorDescriptor;
        }

        public RenderTextureDescriptor DepthDescriptor(RenderTextureDescriptor cameraTexture, ProPixelizerTargetConfiguration config)
        {
            var depthDescriptor = cameraTexture;
            depthDescriptor.useMipMap = false;
            depthDescriptor.width = config.Width;
            depthDescriptor.height = config.Height;
            depthDescriptor.colorFormat = RenderTextureFormat.Depth;
            depthDescriptor.depthBufferBits = 32;
            return depthDescriptor;
        }
    }

    /// <summary>
    /// The meta-data buffer used for the ProPixelizer metadata pass.
    /// </summary>
    public class MetadataTargetData : ContextItem
    {
        public TextureHandle Color;
        public TextureHandle Depth;

        public override void Reset()
        {
        }
    }

    /// <summary>
    /// The outline buffer used for ProPixelizer edges and outlines.
    /// </summary>
    public class OutlineTargetData : ContextItem
    {
        public TextureHandle Color;
        public override void Reset()
        {
        }
    }

    public class PixelizationMapData : ContextItem
    {
        public TextureHandle Map;
        public override void Reset()
        {
        }
    }

    public class ProPixelizerRecompositionData : ContextItem
    {
        public TextureHandle Color;
        public TextureHandle Depth;

        public override void Reset()
        {
        }
    }

    /// <summary>
    /// Camera targets before redirection
    /// </summary>
    public class ProPixelizerOriginalCameraTargets : ContextItem
    {
        public TextureHandle Color;
        public TextureHandle Depth;

        public override void Reset()
        {
        }
    }
#endif
}