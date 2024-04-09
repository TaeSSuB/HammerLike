﻿// Copyright Elliot Bentine, 2018-
#if UNITY_EDITOR
using ProPixelizer.Tools.Migration;
using System;
using UnityEditor;
using UnityEngine;


public class KMJPixelizedWithOutlineShaderGUI : ShaderGUI
{
    bool showColor, showAlpha, showPixelize, showLighting, showOutline;
    bool useColorGrading, useNormalMap, useEmission, useObjectPosition, useAlpha, useShadows;
    private Gradient currentGradient = new Gradient();
    Material Material;
    private MaterialEditor _materialEditor;

    public const string COLOR_GRADING_ON = "COLOR_GRADING_ON";
    public const string NORMAL_MAP_ON = "NORMAL_MAP_ON";
    public const string USE_EMISSION_ON = "USE_EMISSION_ON";
    public const string USE_OBJECT_POSITION_ON = "USE_OBJECT_POSITION_ON";
    public const string ALPHA_ON = "USE_ALPHA_ON";
    public const string RECEIVE_SHADOWS_ON = "RECEIVE_SHADOWS_ON";

    /// <summary>
    /// 2023-11-15
    /// LigthingRamp를 에디터상에서 즉각적으로 만들기 위해서
    /// GUI 내부에
    /// Gradient를 제작하고 제작한 Gradient를 png 파일(texture)형태로 저장하고 나서 저장한 텍스쳐를 
    /// 머터리얼로 활용할 때 texture 를 Lighting Ramp를 활용하자 
    /// </summary>
    /// <param name="gradient"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    private Texture2D GradientToTexture(Gradient gradient, int width = 256)
    {
        Texture2D texture = new Texture2D(width, 1);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        for (int i = 0; i < width; i++)
        {
            float t = i / (float)(width - 1);
            texture.SetPixel(i, 0, gradient.Evaluate(t));
        }

        texture.Apply();
        return texture;
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        materialEditor.serializedObject.Update();
        Material = materialEditor.target as Material;
        useColorGrading = Material.IsKeywordEnabled(COLOR_GRADING_ON);
        useEmission = Material.IsKeywordEnabled(USE_EMISSION_ON);
        useNormalMap = Material.IsKeywordEnabled(NORMAL_MAP_ON);
        useObjectPosition = Material.IsKeywordEnabled(USE_OBJECT_POSITION_ON);
        useAlpha = Material.IsKeywordEnabled(ALPHA_ON);
        useShadows = Material.IsKeywordEnabled(RECEIVE_SHADOWS_ON);

        string materialPath = AssetDatabase.GetAssetPath(Material);
    string directory = System.IO.Path.GetDirectoryName(materialPath);
    string materialName = System.IO.Path.GetFileNameWithoutExtension(materialPath);
    string gradientTexturePath = System.IO.Path.Combine(directory, materialName + "_GradientTexture.asset");
    
    // 저장된 텍스처를 불러오기
    Texture2D savedGradientTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(gradientTexturePath);

    // 저장된 텍스처가 있다면, 그 텍스처를 기반으로 Gradient 초기화
    if (savedGradientTexture != null)
    {
        currentGradient = TextureToGradient(savedGradientTexture);
    }

        EditorGUILayout.LabelField("ProPixelizer | Appearance+Outline Material", EditorStyles.boldLabel);
        if (GUILayout.Button("User Guide")) Application.OpenURL("https://sites.google.com/view/propixelizer/user-guide");
        EditorGUILayout.Space();

        if (CheckForUpdate(materialEditor.serializedObject))
            return;


        DrawAppearanceGroup(materialEditor, properties);
        DrawLightingGroup(materialEditor, properties);
        DrawPixelizeGroup(materialEditor, properties);
        //DrawAlphaGroup(materialEditor, properties);
        DrawOutlineGroup(materialEditor, properties);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //var enableInstancing = EditorGUILayout.ToggleLeft("Enable GPU Instancing", Material.enableInstancing);
        //Material.enableInstancing = enableInstancing;
        Material.enableInstancing = false;
        var renderQueue = EditorGUILayout.IntField("Render Queue", Material.renderQueue);
        Material.renderQueue = renderQueue;
        var dsgi = EditorGUILayout.ToggleLeft("Double Sided Global Illumination", Material.doubleSidedGI);
        Material.doubleSidedGI = dsgi;

        //EditorUtility.SetDirty(Material);
        materialEditor.serializedObject.ApplyModifiedProperties();
    }

    public void DrawAppearanceGroup(MaterialEditor editor, MaterialProperty[] properties)
    {
        showColor = EditorGUILayout.BeginFoldoutHeaderGroup(showColor, "Appearance");
        if (showColor)
        {
            var albedo = FindProperty("_Albedo", properties);
            editor.TextureProperty(albedo, "Albedo", true);

            var baseColor = FindProperty("_BaseColor", properties);
            editor.ColorProperty(baseColor, "Color");

            var colorGrading = FindProperty("COLOR_GRADING", properties);
            editor.ShaderProperty(colorGrading, "Use Color Grading");
            if (colorGrading.floatValue > 0f)
            {
                var lut = FindProperty("_PaletteLUT", properties);
                editor.TextureProperty(lut, "Palette", false);
            }

            var normal = FindProperty("_NormalMap", properties);
            editor.TextureProperty(normal, "Normal Map", true);

            var emission = FindProperty("_Emission", properties);
            editor.TextureProperty(emission, "Emission", true);

            var emissiveColor = FindProperty("_EmissionColor", properties);
            editor.ColorProperty(emissiveColor, "Emission Color");
            EditorGUILayout.HelpBox("Emission Color is multiplied by the Emission texture to determine the emissive output. The default emissive color and texture and both black.", MessageType.Info);

            var diffuseVertexColorWeight = FindProperty("_DiffuseVertexColorWeight", properties);
            editor.RangeProperty(diffuseVertexColorWeight, "Diffuse Vertex Color Weight");
            var emissiveVertexColorWeight = FindProperty("_EmissiveVertexColorWeight", properties);
            editor.RangeProperty(emissiveVertexColorWeight, "Emissive Vertex Color Weight");
            EditorGUILayout.HelpBox("The vertex color sliders control how the emissive and diffuse colors are affected by a mesh's vertex colors. Vertex colors are used for many per-particle color properties by Unity's particle system.", MessageType.Info);

            var use_alpha_clip = FindProperty("PROPIXELIZER_DITHERING", properties);
            editor.ShaderProperty(use_alpha_clip, "Use Dithered Transparency");
            var threshold = FindProperty("_AlphaClipThreshold", properties);
            editor.ShaderProperty(threshold, "Alpha Clip Threshold");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }



    public void DrawLightingGroup(MaterialEditor editor, MaterialProperty[] properties)
{
    showLighting = EditorGUILayout.BeginFoldoutHeaderGroup(showLighting, "Lighting");
    if (showLighting)
    {
         EditorGUI.BeginChangeCheck();
        currentGradient = EditorGUILayout.GradientField("Lighting Ramp", currentGradient);
        if (EditorGUI.EndChangeCheck())
        {
            Texture2D gradientTexture = GradientToTexture(currentGradient);
            Material.SetTexture("_LightingRamp", gradientTexture);

            // 머터리얼의 경로와 이름을 사용하여 gradient texture의 저장 경로 생성
            string materialPath = AssetDatabase.GetAssetPath(Material);
            string directory = System.IO.Path.GetDirectoryName(materialPath);
            string materialName = System.IO.Path.GetFileNameWithoutExtension(materialPath);
            string gradientTexturePath = System.IO.Path.Combine(directory, materialName + "_GradientTexture.asset");

            // 시작: 에셋으로 저장하는 부분
            if (AssetDatabase.LoadAssetAtPath<Texture2D>(gradientTexturePath) == null)
            {
                AssetDatabase.CreateAsset(gradientTexture, gradientTexturePath);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Texture2D existingTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(gradientTexturePath);
                existingTexture.SetPixels(gradientTexture.GetPixels());
                existingTexture.Apply();
                EditorUtility.SetDirty(existingTexture);
                AssetDatabase.SaveAssets();
            }
            // 끝: 에셋으로 저장하는 부분
        }

        var ambient = FindProperty("_AmbientLight", properties);
        editor.ShaderProperty(ambient, "Ambient Light");
        EditorGUILayout.HelpBox("The Ambient Light alpha value can be used to blend between scene ambient color and spherical harmonics (alpha zero) or the color of the Ambient Light property (alpha one).", MessageType.Info);

        var receiveShadows = FindProperty("RECEIVE_SHADOWS", properties);
        editor.ShaderProperty(receiveShadows, "Receive shadows");
    }
    EditorGUILayout.EndFoldoutHeaderGroup();
}

    private Gradient TextureToGradient(Texture2D texture)
{
    Gradient gradient = new Gradient();

    int maxKeys = 8;
    int step = texture.width / maxKeys;

    GradientColorKey[] colorKeys = new GradientColorKey[maxKeys];

    for (int i = 0; i < maxKeys; i++)
    {
        colorKeys[i].color = texture.GetPixel(i * step, 0);
        colorKeys[i].time = i / (float)(maxKeys - 1);
    }

    gradient.colorKeys = colorKeys;
    gradient.alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) }; // Assuming full alpha

    return gradient;
}


    public void DrawPixelizeGroup(MaterialEditor editor, MaterialProperty[] properties)
    {
        showPixelize = EditorGUILayout.BeginFoldoutHeaderGroup(showPixelize, "Pixelize");
        if (showPixelize)
        {
            var pixelSize = FindProperty("_PixelSize", properties);
            editor.ShaderProperty(pixelSize, "Pixel Size");

            var useObjectPosition = FindProperty("USE_OBJECT_POSITION", properties);
            editor.ShaderProperty(useObjectPosition, "Object Position");
            EditorGUILayout.HelpBox("Whether to use the object position as the zero coordinate for the pixel grid. For more information, see the 'Aligning Pixel Grids' section in the user guide.", MessageType.Info);
            if (useObjectPosition.floatValue < 0.5f)
            {
                var gridPosition = FindProperty("_PixelGridOrigin", properties);
                editor.ShaderProperty(gridPosition, "Origin (world space)");
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    public void DrawOutlineGroup(MaterialEditor editor, MaterialProperty[] properties)
    {
        showOutline = EditorGUILayout.BeginFoldoutHeaderGroup(showOutline, "Outline");
        if (showOutline)
        {
            var oID = FindProperty("_ID", properties);
            editor.ShaderProperty(oID, "ID");
            EditorGUILayout.HelpBox("The ID is an 8-bit number used to identify different objects in the " +
                "buffer for purposes of drawing outlines. Outlines are drawn when a pixel is next to a pixel " +
                "of different ID value.", MessageType.Info);
            var outlineColor = FindProperty("_OutlineColor", properties);
            editor.ShaderProperty(outlineColor, "Outline Color");
            var edgeHighlightColor = FindProperty("_EdgeHighlightColor", properties);
            editor.ShaderProperty(edgeHighlightColor, "Edge Highlight");
            EditorGUILayout.HelpBox("Use color values less than 0.5 to darken edge highlights. " +
                "Use color values greater than 0.5 to lighten edge highlights. " + 
                "Use color values of 0.5 to make edge highlights invisible.\n\n"+
                "You may need to enable the setting in the Pixelisation Feature on your Forward Renderer asset.", MessageType.Info);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

 
    public bool CheckForUpdate(SerializedObject so)
    {
        try
        {
            var updater = new ProPixelizer1_8MaterialUpdater();
            if (updater.CheckForUpdate(so))
            {
                EditorGUILayout.HelpBox(
                    "Properties from a previous version of ProPixelizer detected. " +
                    "Press the button below to migrate material properties to new names.",
                    MessageType.Warning);
                if (GUILayout.Button("Update"))
                    updater.DoUpdate(so);
                EditorGUILayout.Space();
                return true;
            }
        } catch (Exception)
        {
            EditorGUILayout.HelpBox(
            "Error occured while attempting to check if material requires migration.",
            MessageType.Warning);
        }
        return false;
    }
}
#endif