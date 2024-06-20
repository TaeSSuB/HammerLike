// Copyright Elliot Bentine, 2018-
#if (UNITY_EDITOR)

using UnityEditor;
using UnityEngine;

namespace ProPixelizer.Tools
{
    [CustomEditor(typeof(ProPixelizerCamera))]
    [DisallowMultipleComponent]
    public class ProPixelizerCameraEditor : Editor
    {
        public const string SHORT_HELP = "The ProPixelizerCamera component eliminates pixel creep " +
            "for (i) static GameObjects, and (ii) for moving GameObjects with an ObjectRenderSnapable component. It also provides options to configure ProPixelizer per-camera.";

        //bool showPixelisation = true;
        //bool showOverrides = true;

        public override void OnInspectorGUI()
        {
            ProPixelizerCamera snap = (ProPixelizerCamera)target;

            EditorGUILayout.LabelField("ProPixelizer | Camera", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(SHORT_HELP, MessageType.None);
            EditorGUILayout.LabelField("");

            var cameraComponent = snap.gameObject.GetComponent<Camera>();
            if (cameraComponent == null) {
                EditorGUILayout.HelpBox("No camera found on this GameObject.", MessageType.Info);
                return;
            }
            if (!cameraComponent.orthographic)
                EditorGUILayout.HelpBox("The camera snap behaviour is intended for orthographic cameras. Pixel creep cannot be completely eradicted for perspective projections because the object size changes as it moves across the screen.", MessageType.Info);

            serializedObject.Update();

            EditorGUILayout.LabelField("Pixel size control", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Mode"));
            switch (snap.Mode)
            {
                case ProPixelizerCamera.PixelSizeMode.FixedWorldSpacePixelSize:
                    EditorGUILayout.HelpBox("The size of a pixel in the low-res target is defined in world-space units.", MessageType.None);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("PixelSize"));
                    // Give an idea of the current world space pixel size.
#if UNITY_2022_3_OR_NEWER
                    if (cameraComponent.orthographic)
                    {
                        // Believe it or not, GetRenderingResolution doesn't get the rendering resolution, it gets the window size.
                        // So if you have it on anything other than 'Free Aspect', it will give you the wrong answer.
                        
                        PlayModeWindow.GetRenderingResolution(out uint gameWidth, out uint gameHeight);
                        var gameViewPixelSize = 2f * cameraComponent.orthographicSize / gameHeight;
                        //EditorGUILayout.LabelField(string.Format("Current GameView in Free Aspect mode is ({0} x {1}).", gameWidth, gameHeight), EditorStyles.miniLabel);

                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField(string.Format("Note: GameView in Free Aspect mode currently has world-space pixel size={0}.", gameViewPixelSize), EditorStyles.wordWrappedMiniLabel);
                        EditorGUI.indentLevel--;
                    }
#endif
                    // 
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ClampCameraOrthoSize"));
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TargetScale"));
                    GUI.enabled = true;
                    if (snap.CannotMaintainWSPixelSize)
                    {
                        EditorGUILayout.HelpBox("While running, the desired world-space pixel size could not be maintained for this the screen resolution and camera orthographic size. Either increase the pixel size, or enable ortho size clamping.", MessageType.Warning);
                    }
                    break;
                case ProPixelizerCamera.PixelSizeMode.FixedDownscalingRatio:
                    EditorGUILayout.HelpBox("The resolution of the low-res target is a fixed multiple of the viewport resolution.", MessageType.None);
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("PixelSize"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ClampCameraOrthoSize"));
                    GUI.enabled = true;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TargetScale"));
                    break;
            }
            //EditorGUILayout.Separator();
            

            //EditorGUILayout.PropertyField(serializedObject.FindProperty("EnableProPixelizer"));
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Render Feature Overrides", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = true;
            var enableProPixelizer = serializedObject.FindProperty("EnableProPixelizer");
            enableProPixelizer.boolValue = EditorGUILayout.Toggle(enableProPixelizer.boolValue, GUILayout.MaxWidth(15f));
            GUI.enabled = enableProPixelizer.boolValue;
            EditorGUILayout.LabelField("Enable ProPixelizer");
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = true;
            var overrideColorGrading = serializedObject.FindProperty("overrideColorGrading");
            overrideColorGrading.boolValue = EditorGUILayout.Toggle(overrideColorGrading.boolValue, GUILayout.MaxWidth(15f));
            GUI.enabled = overrideColorGrading.boolValue;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FullscreenColorGradingLUT"));
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = true;
            var overridePixelExpansion = serializedObject.FindProperty("overrideUsePixelExpansion");
            overridePixelExpansion.boolValue = EditorGUILayout.Toggle(overridePixelExpansion.boolValue, GUILayout.MaxWidth(15f));
            GUI.enabled = overridePixelExpansion.boolValue;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UsePixelExpansion"));
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();


            //EditorGUILayout.BeginHorizontal();
            //snap.overridePixelisationFilter = EditorGUILayout.Toggle(snap.overridePixelisationFilter, GUILayout.MaxWidth(15f));
            //GUI.enabled = snap.overridePixelisationFilter;
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("PixelizationFilter"));
            //GUI.enabled = true;
            //EditorGUILayout.EndHorizontal();


            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif