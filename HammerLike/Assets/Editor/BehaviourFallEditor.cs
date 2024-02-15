using UnityEngine;
using UnityEditor;
using RootMotion.Dynamics;

[CustomEditor(typeof(BehaviourFall))]
public class BehaviourFallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BehaviourFall script = (BehaviourFall)target;

        // Draw default inspector
        DrawDefaultInspector();

        // Add custom UI elements here. For example:
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Editor Elements", EditorStyles.boldLabel);

        // Example of custom editor code
        script.stateName = EditorGUILayout.TextField("State Name", script.stateName);
        script.transitionDuration = EditorGUILayout.FloatField("Transition Duration", script.transitionDuration);
        script.layer = EditorGUILayout.IntField("Layer", script.layer);
        script.fixedTime = EditorGUILayout.FloatField("Fixed Time", script.fixedTime);
        // Repeat for other fields as necessary

        // Save changes and undo
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
