using UnityEngine;
using UnityEditor;
using System.Linq;

public class SortHierarchyEditor : EditorWindow
{
    private int sortOptionIndex = 0;
    private string[] sortOptions = new string[] { "Sort By Object Name", "Sort By Position" };

    [MenuItem("Tools/Sort Hierarchy")]
    public static void ShowWindow()
    {
        GetWindow<SortHierarchyEditor>("Sort Hierarchy");
    }

    void OnGUI()
    {
        sortOptionIndex = EditorGUILayout.Popup("Sort Option", sortOptionIndex, sortOptions);

        if (GUILayout.Button("Sort Hierarchy"))
        {
            SortSelectedObjects();
        }
    }

    void SortSelectedObjects()
    {
        string sortOption = sortOptions[sortOptionIndex];
        GameObject[] selectedObjects = Selection.gameObjects;

        if (sortOption == "Sort By Object Name")
        {
            selectedObjects = selectedObjects.OrderBy(go => go.name).ToArray();
        }
        else if (sortOption == "Sort By Position")
        {
            GameObject firstObject = selectedObjects[0];
            selectedObjects = selectedObjects.OrderBy(go => Vector3.Distance(go.transform.position, firstObject.transform.position)).ToArray();
        }

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            selectedObjects[i].transform.SetSiblingIndex(i);
        }

        Debug.Log("Hierarchy Sorted by " + sortOption);
    }
}