using UnityEditor;
using UnityEngine;
using System.Linq; 

[CustomEditor(typeof(RandomTileUV))]
public class RandomTileUVEditor : Editor
{
    SerializedProperty materialsDataList;

    private void OnEnable()
    {
        materialsDataList = serializedObject.FindProperty("materialsData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        RandomTileUV myScript = (RandomTileUV)target;
        Renderer rend = myScript.GetComponent<Renderer>();

        if (rend && rend.sharedMaterials.Length > 0)
        {
            string[] materialNames = rend.sharedMaterials.Select(m => m.name).ToArray();

            for (int i = 0; i < materialsDataList.arraySize; i++)
            {
                SerializedProperty materialData = materialsDataList.GetArrayElementAtIndex(i);

                SerializedProperty materialProp = materialData.FindPropertyRelative("material");
                SerializedProperty tilesXProp = materialData.FindPropertyRelative("tilesX");
                SerializedProperty tilesYProp = materialData.FindPropertyRelative("tilesY");

                int selectedIndex = System.Array.IndexOf(rend.sharedMaterials, materialProp.objectReferenceValue as Material);
                selectedIndex = EditorGUILayout.Popup("Target Material", selectedIndex, materialNames);
                if (selectedIndex >= 0 && selectedIndex < rend.sharedMaterials.Length)
                {
                    materialProp.objectReferenceValue = rend.sharedMaterials[selectedIndex];
                }

                EditorGUILayout.PropertyField(tilesXProp);
                EditorGUILayout.PropertyField(tilesYProp);
            }

            if (GUILayout.Button("Add Material Data"))
            {
                materialsDataList.InsertArrayElementAtIndex(materialsDataList.arraySize);
            }

            if (materialsDataList.arraySize > 0 && GUILayout.Button("Remove Last Material Data"))
            {
                materialsDataList.DeleteArrayElementAtIndex(materialsDataList.arraySize - 1);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No materials found!", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
