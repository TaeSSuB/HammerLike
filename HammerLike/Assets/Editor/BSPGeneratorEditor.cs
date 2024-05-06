using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BSPGenerator))]
public class BSPGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector layout
        DrawDefaultInspector();

        // Get a reference to the target BSPGenerator object
        BSPGenerator bspGenerator = (BSPGenerator)target;

        // Add a "ReGenerate" button to the inspector
        if (GUILayout.Button("ReGenerate"))
        {
            // Call the ReGenerator method on the BSPGenerator object
            bspGenerator.ReGenerator();
        }
    }
}
