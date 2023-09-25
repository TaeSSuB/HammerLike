using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ApartmentRoomSpawner), true)]
public class ApartmentRoomEditor : Editor
{
    ApartmentRoomSpawner generator;

    private void Awake()
    {
        generator = (ApartmentRoomSpawner)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("SpawnGrid"))
        {
            generator.SpawnGrid();
        }
        if (GUILayout.Button("MatchRoomToGrid"))
        {
            generator.MatchRoomToGrid();
        }
        if (GUILayout.Button("RandomConnectGrid"))
        {
            generator.RandomConnectGrid();
        }
        
    }
}
