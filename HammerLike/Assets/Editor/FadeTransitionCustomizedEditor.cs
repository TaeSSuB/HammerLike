using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FadeTransitionExtended))]
public class FadeTransitionCustomizedEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FadeTransitionExtended script = (FadeTransitionExtended)target;

        if (GUILayout.Button("Move Up"))
        {
            MoveElement(script, -1);
        }

        if (GUILayout.Button("Move Down"))
        {
            MoveElement(script, 1);
        }

        base.OnInspectorGUI();
    }

    private void MoveElement(FadeTransitionExtended script, int direction)
    {
        int newSelected = script.selectedElement + direction;

        if (newSelected < 0 || newSelected >= script.imagesSettings.Length)
            return;

        // 선택된 요소와 교환할 요소의 위치를 바꿉니다.
        var temp = script.imagesSettings[newSelected];
        script.imagesSettings[newSelected] = script.imagesSettings[script.selectedElement];
        script.imagesSettings[script.selectedElement] = temp;

        // 선택된 요소의 인덱스도 업데이트합니다.
        script.selectedElement = newSelected;
    }

}
