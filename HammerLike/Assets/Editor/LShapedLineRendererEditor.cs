using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LShapedLineRenderer))]
public class LShapedLineRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default inspector

        LShapedLineRenderer script = (LShapedLineRenderer)target;

        if (GUILayout.Button("Setup Line Positions"))
        {
            script.SetupLinePositions(); // Call the function when the button is clicked
        }
    }

    private void OnEnable()
    {
        LShapedLineRenderer script = (LShapedLineRenderer)target;
        if (script.pointA != null && script.pointB != null)
        {
            script.SetupLinePositions(); // Automatically setup when the script is loaded in the editor
        }
    }
}
