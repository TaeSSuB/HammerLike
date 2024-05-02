// Copyright Elliot Bentine, 2018-
namespace ProPixelizer
{
	public enum LowResolutionMask
	{
		/// <summary>
		/// Materials using ProPixelizer shaders are pixelated.
		/// </summary>
		ProPixelizerShaders,

		/// <summary>
		/// A layer mask is rendered as pixelated.
		/// </summary>
		Layers,

		// This mode will not be added until RenderLayers are stable and documented by Unity:
		// RenderLayers
	}
}