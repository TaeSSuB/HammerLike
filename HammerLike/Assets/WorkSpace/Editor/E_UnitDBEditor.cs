using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_UnitDB))]
public class E_UnitDBEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SO_UnitDB unitDB = (SO_UnitDB)target;

        if (GUILayout.Button("Add Unit"))
        {
            DatabaseFromNotion databaseFromNotion = new DatabaseFromNotion();

            EditorCoroutineUtility.StartCoroutineOwnerless(databaseFromNotion.LoadDatas());
        }

    }
}
