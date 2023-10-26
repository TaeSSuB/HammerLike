using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomTileMapping))]
public class RandomTileMappingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RandomTileMapping script = (RandomTileMapping)target;

        // 기본 스크립트 변수들을 표시
        DrawDefaultInspector();

        // 현재 오브젝트의 렌더러에 연결된 머터리얼들만 표시
        Renderer renderer = script.gameObject.GetComponent<Renderer>();
        if (renderer)
        {
            int selectedIndex = -1;
            string[] options = new string[renderer.sharedMaterials.Length];
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                options[i] = renderer.sharedMaterials[i].name;
                if (renderer.sharedMaterials[i] == script.targetMaterial)
                {
                    selectedIndex = i;
                }
            }
            selectedIndex = EditorGUILayout.Popup("Target Material", selectedIndex, options);
            if (selectedIndex >= 0 && selectedIndex < renderer.sharedMaterials.Length)
            {
                script.targetMaterial = renderer.sharedMaterials[selectedIndex];
            }
        }
    }
}
#endif
