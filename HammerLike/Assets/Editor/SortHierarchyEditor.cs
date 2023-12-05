using UnityEngine;
using UnityEditor;
using System.Linq;

public class SortHierarchyWindow : EditorWindow
{
    [MenuItem("Tools/Sort Hierarchy")]
    public static void ShowWindow()
    {
        GetWindow<SortHierarchyWindow>("Sort Hierarchy");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Sort Selected Objects"))
        {
            SortSelectedObjects();
        }
    }

    private static void SortSelectedObjects()
    {
        foreach (var selectedObject in Selection.gameObjects)
        {
            SortChildren(selectedObject.transform);
        }
    }

    private static void SortChildren(Transform parent)
    {
        var children = parent.Cast<Transform>().OrderBy(t => t.name).ToList();
        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);
        }
    }
}