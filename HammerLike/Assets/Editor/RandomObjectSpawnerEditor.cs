using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(RandomObjectSpawner))]
public class RandomObjectSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 기본 인스펙터 UI를 그립니다.

        RandomObjectSpawner script = (RandomObjectSpawner)target;
        if (GUILayout.Button("Spawn Random Object"))
        {
            script.ReplaceObject(); // 버튼 클릭 시 함수 호출
        }
    }
}
