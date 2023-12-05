using UnityEngine;
using UnityEditor;

public class RoundTransformWindow : EditorWindow
{
    [MenuItem("Tools/Round Transform")]
    public static void ShowWindow()
    {
        GetWindow<RoundTransformWindow>("Round Transform");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Round Selected Transforms"))
        {
            RoundTransforms();
        }
    }

    private static void RoundTransforms()
    {
        foreach (var selectedTransform in Selection.transforms)
        {
            selectedTransform.position = RoundVector3(selectedTransform.position);
            selectedTransform.eulerAngles = RoundVector3(selectedTransform.eulerAngles);
            selectedTransform.localScale = RoundVector3(selectedTransform.localScale);
        }
    }

    private static Vector3 RoundVector3(Vector3 v)
    {
        return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
    }
}