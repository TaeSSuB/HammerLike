using UnityEngine;
using UnityEditor;

public class RoundTransformEditor : EditorWindow
{
    bool roundPosition = true;
    bool roundRotation = true;
    bool roundScale = true;

    [MenuItem("Tools/Round Transform")]
    public static void ShowWindow()
    {
        GetWindow<RoundTransformEditor>("Round Transform");
    }

    void OnGUI()
    {
        GUILayout.Label("Select Transform Components to Round", EditorStyles.boldLabel);
        roundPosition = EditorGUILayout.Toggle("Round Position", roundPosition);
        roundRotation = EditorGUILayout.Toggle("Round Rotation", roundRotation);
        roundScale = EditorGUILayout.Toggle("Round Scale", roundScale);

        if (GUILayout.Button("Round Transform"))
        {
            RoundSelectedTransforms();
        }
    }

    void RoundSelectedTransforms()
    {
        foreach (GameObject selected in Selection.gameObjects)
        {
            if (roundPosition)
            {
                Vector3 position = selected.transform.position;
                position.x = RoundToHalf(position.x);
                position.y = RoundToHalf(position.y);
                position.z = RoundToHalf(position.z);
                selected.transform.position = position;
            }

            if (roundRotation)
            {
                Vector3 rotation = selected.transform.eulerAngles;
                rotation.x = RoundToHalf(rotation.x);
                rotation.y = RoundToHalf(rotation.y);
                rotation.z = RoundToHalf(rotation.z);
                selected.transform.eulerAngles = rotation;
            }

            if (roundScale)
            {
                Vector3 scale = selected.transform.localScale;
                scale.x = RoundToHalf(scale.x);
                scale.y = RoundToHalf(scale.y);
                scale.z = RoundToHalf(scale.z);
                selected.transform.localScale = scale;
            }
        }
    }

    float RoundToHalf(float value)
    {
        return Mathf.Round(value * 2f) / 2f;
    }
}