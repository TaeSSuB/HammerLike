using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BSPGenerator))]
public class BSPGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); 

        BSPGenerator script = (BSPGenerator)target;

        // ReGenerate 버튼 추가
        if (GUILayout.Button("ReGenerate"))
        {
            script.ReGenerator();
        }
    }
}
