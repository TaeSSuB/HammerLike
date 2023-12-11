using UnityEngine;
using UnityEditor;

public class ChangePivotByParentWindow : EditorWindow
{
    private GameObject selectedParent;

    [MenuItem("Tools/Change Pivot by Parent")]
    public static void ShowWindow()
    {
        GetWindow<ChangePivotByParentWindow>("Change Pivot by Parent");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select the reference parent object", EditorStyles.boldLabel);
        selectedParent = (GameObject)EditorGUILayout.ObjectField(selectedParent, typeof(GameObject), true);

        if (GUILayout.Button("Change"))
        {
            ChangePivot();
        }
    }

    private void ChangePivot()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedParent && selectedObject)
        {
            Undo.RecordObject(selectedObject.transform, "Change Pivot");
            selectedObject.transform.SetParent(selectedParent.transform, true);
            selectedObject.transform.localPosition = Vector3.zero;
            selectedObject.transform.localRotation = Quaternion.identity;
            selectedObject.transform.SetParent(null, true);
        }
    }
}