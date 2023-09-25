using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
[PostProcess(typeof(PixelizerRenderer), PostProcessEvent.AfterStack, "Custom/Pixelizer")]
public sealed class PixelizerEffect : PostProcessEffectSettings
{
    [Range(1, 100), Tooltip("Pixel Size.")]
    public IntParameter pixelSize = new IntParameter { value = 1 };
}

public sealed class PixelizerRenderer : PostProcessEffectRenderer<PixelizerEffect>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Custom/Pixelizer"));
        sheet.properties.SetFloat("_PixelSize", settings.pixelSize);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
