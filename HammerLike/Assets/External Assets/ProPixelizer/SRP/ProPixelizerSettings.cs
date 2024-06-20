// Copyright Elliot Bentine, 2018-
using UnityEngine;

namespace ProPixelizer
{
    public struct ProPixelizerSettings
    {
        public PixelizationFilter Filter;
        public LayerMask PixelizedLayers;
        public bool UsePixelExpansion;
        /// <summary>
        /// The resolution scaling of the low-resolution target, relative to the screen. Lower values = greater pixelisation.
        /// </summary>
        public float DefaultLowResScale;
        public bool UseDepthTestingForEdgeOutlines;
        public bool UseNormalsForEdgeDetection;
        public float DepthTestThreshold;
        public bool UseDepthTestingForIDOutlines;
        public float NormalEdgeDetectionThreshold;
        public Texture2D FullscreenLUT;
        public bool UsePixelArtUpscalingFilter;
        public bool Enabled;
    }

    public enum PixelizationFilter
    {
        OnlyProPixelizer,
        FullScene,
        Layers
    }
}