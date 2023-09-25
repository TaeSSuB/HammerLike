using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
[PostProcess(typeof(EdgeDetectionRenderer), PostProcessEvent.AfterStack, "Custom/EdgeDetection")]
public sealed class EdgeDetectionEffect : PostProcessEffectSettings
{
    public ColorParameter edgeColor = new ColorParameter { value = Color.black };
    public FloatParameter edgeAmount = new FloatParameter { value = 1.0f };
}

public sealed class EdgeDetectionRenderer : PostProcessEffectRenderer<EdgeDetectionEffect>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Custom/EdgeDetection"));
        sheet.properties.SetColor("_EdgeColor", settings.edgeColor);
        sheet.properties.SetFloat("_EdgeAmount", settings.edgeAmount);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
