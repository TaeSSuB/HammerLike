using UnityEngine;
using UnityEditor;

public class MeshDestroyTool : EditorWindow
{
    GameObject selectedObject;
    MeshDestroy meshDestroyScript;

    [MenuItem("Tools/MeshDestroyTool")]
    public static void ShowWindow()
    {
        GetWindow<MeshDestroyTool>("MeshDestroyTool");
    }

    void OnGUI()
    {
        GUILayout.Label("Select an Object to Destroy", EditorStyles.boldLabel);

        selectedObject = EditorGUILayout.ObjectField("Object", selectedObject, typeof(GameObject), true) as GameObject;

        if (selectedObject != null)
        {
            if (GUILayout.Button("Destroy Mesh"))
            {
                if (selectedObject.GetComponent<MeshDestroy>() == null)
                {
                    meshDestroyScript = selectedObject.AddComponent<MeshDestroy>();
                }
                else
                {
                    meshDestroyScript = selectedObject.GetComponent<MeshDestroy>();
                }

                meshDestroyScript.DestroyMesh();
                EditorUtility.SetDirty(selectedObject);
            }
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}
