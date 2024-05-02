using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DatabaseConfigurator))]
public class DatabaseConfiguratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DatabaseConfigurator configurator = (DatabaseConfigurator)target;

        if (GUILayout.Button("Add New Condition"))
        {
            configurator.filterConfig.conditions.Add(new FilterCondition());
        }

        for (int i = 0; i < configurator.filterConfig.conditions.Count; i++)
        {
            var condition = configurator.filterConfig.conditions[i];
            EditorGUILayout.BeginHorizontal();
            condition.propertyName = EditorGUILayout.TextField("Property Name", condition.propertyName);
            condition.comparator = (FilterCondition.Comparator)EditorGUILayout.EnumPopup("Comparator", condition.comparator);
            condition.threshold = EditorGUILayout.FloatField("Threshold", condition.threshold);
            if (GUILayout.Button("-"))
            {
                configurator.filterConfig.conditions.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

        serializedObject.ApplyModifiedProperties();
    }
}