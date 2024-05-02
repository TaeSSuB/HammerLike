﻿// Copyright Elliot Bentine, 2018-
#if (UNITY_EDITOR)

using UnityEditor;

namespace ProPixelizer.Tools
{
    [CustomEditor(typeof(ProPixelizerRenderFeature))]
    public class PixelisationFeatureGUI : Editor
    {
        public override void OnInspectorGUI()
        {
            ProPixelizerRenderFeature feature = (ProPixelizerRenderFeature)target;
            EditorGUILayout.HelpBox("This Render Feature adds all passes required for ProPixelizer.", MessageType.None);
            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Pixelization", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UsePixelExpansion"));
            EditorGUILayout.HelpBox(
                "ProPixelizer's Pixel Expansion method allows you to have per-object pixel sizes. It also" +
                " allows pixelated objects to move relative to one another at increments smaller than their " +
                "pixel size, which can help motion feel smooth and responsive.",
                MessageType.None);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("PixelizationFilter"));
            switch (feature.PixelizationFilter)
            {
                case PixelizationFilter.FullScene:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("UsePixelArtUpscalingFilter"));
                    break;
                case PixelizationFilter.Layers:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("PixelizedLayers"));
                    break;
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DefaultLowResScale"));
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Outline Detection Controls", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseDepthTestingForIDOutlines"));
            if (feature.UseDepthTestingForIDOutlines)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("DepthTestThreshold"));            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseNormalsForEdgeDetection"));
            if (feature.UseNormalsForEdgeDetection)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("NormalEdgeDetectionThreshold"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UseDepthTestingForEdgeOutlines"));
            }

            //EditorGUILayout.LabelField("");
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Fullscreen Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "These settings are applied to the entire screen. ProPixelizer also supports per-object color grading" +
                " and edge control through individual materials. You may also override these properties for each camera.",
                MessageType.None);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FullscreenColorPalette"));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Editor Scene Tab", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EditorFullscreenColorGrading"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EditorPixelExpansion"));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Debugging", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GenerateWarnings"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif