using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PixelateCam : MonoBehaviour
{
    [Range(1, 200)] public int pixelate = 100;
    public bool outline;
    public Shader pixelateShader;

    private Material pixelateMaterial;

    private void Start()
    {
        if (!pixelateMaterial && pixelateShader)
            pixelateMaterial = new Material(pixelateShader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (pixelateMaterial)
        {
            pixelateMaterial.SetFloat("_PixelSizeX", pixelate);
            pixelateMaterial.SetFloat("_PixelSizeY", pixelate);
            pixelateMaterial.SetFloat("_Outline", outline ? 1 : 0);

            Graphics.Blit(source, destination, pixelateMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
