using UnityEditor;
using UnityEngine;

public class RoundTransformWindow : EditorWindow
{
    [MenuItem("Tools/Round Transform")]
    public static void ShowWindow()
    {
        GetWindow<RoundTransformWindow>("Round Transform");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Round Transform"))
        {
            RoundSelectedTransforms();
        }
    }

    private void RoundSelectedTransforms()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            RoundTransform rt = obj.AddComponent<RoundTransform>();
            rt.RoundTransformValues();
            DestroyImmediate(rt);
        }
    }
}