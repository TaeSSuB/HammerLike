using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ItemTool : EditorWindow
{
    GameObject model;
    Shader shader;
    MonoScript script;

    [MenuItem("Tools/Item Creator")]
    public static void ShowWindow()
    {
        GetWindow<ItemTool>("Item Creator");
    }

    void OnGUI()
    {
        model = (GameObject)EditorGUILayout.ObjectField("Model", model, typeof(GameObject), false);
        shader = (Shader)EditorGUILayout.ObjectField("Shader", shader, typeof(Shader), false);
        script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);

        if (GUILayout.Button("Create Item"))
        {
            CreateItemPrefab();
        }
    }

    void CreateItemPrefab()
    {
        if (model == null || shader == null || script == null)
        {
            Debug.LogError("All fields must be filled out");
            return;
        }

        GameObject item = Instantiate(model);
        item.GetComponent<Renderer>().material.shader = shader;
        item.AddComponent(script.GetClass());

        // 태그 할당 부분
        item.tag = "Item";

        string path = "Assets/Item/Prefabs/" + model.name + "_Item.prefab";
        PrefabUtility.SaveAsPrefabAsset(item, path);

        DestroyImmediate(item);

        Debug.Log("Item prefab created: " + path);
    }

}
