using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DatabaseFromNotion))]
public class DatabaseFromNotionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DatabaseFromNotion script = (DatabaseFromNotion)target;

        // Add or edit conditions
        if (GUILayout.Button("Add New Condition"))
        {
            script.filterConfig.conditions.Add(new FilterCondition());
        }

        for (int i = 0; i < script.filterConfig.conditions.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Condition " + (i + 1), EditorStyles.boldLabel);
            // Dropdown for selecting property name
            var condition = script.filterConfig.conditions[i];
            int selected = DatabaseFromNotion.PropertyNames.IndexOf(condition.propertyName);
            int choice = EditorGUILayout.Popup("Property Name", selected, DatabaseFromNotion.PropertyNames.ToArray());
            condition.propertyName = DatabaseFromNotion.PropertyNames[choice >= 0 ? choice : 0];

            // Other condition fields
            condition.comparator = (FilterCondition.Comparator)EditorGUILayout.EnumPopup("Comparator", condition.comparator);
            condition.threshold = EditorGUILayout.FloatField("Threshold", condition.threshold);

            // Remove condition button
            if (GUILayout.Button("Remove Condition"))
            {
                script.filterConfig.conditions.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        // Sorting configuration
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sorting Configuration", EditorStyles.boldLabel);
        if (script.filterConfig.sortConfig == null)
            script.filterConfig.sortConfig = new SortConfig();  // Ensure there is a sort config to edit

        // Dropdown for sorting property
        if (DatabaseFromNotion.PropertyNames.Count > 0)
        {
            int sortSelectedIndex = DatabaseFromNotion.PropertyNames.IndexOf(script.filterConfig.sortConfig.sortByProperty);
            int sortPropertyIndex = EditorGUILayout.Popup("Sort By Property", sortSelectedIndex, DatabaseFromNotion.PropertyNames.ToArray());
            script.filterConfig.sortConfig.sortByProperty = DatabaseFromNotion.PropertyNames[sortPropertyIndex >= 0 ? sortPropertyIndex : 0];
        }

        // Toggle for sorting order
        script.filterConfig.sortConfig.ascending = EditorGUILayout.Toggle("Ascending Order", script.filterConfig.sortConfig.ascending);

        // Toggle for "Only Buildable"
        script.onlyBuildable = EditorGUILayout.Toggle("Only Buildable", script.onlyBuildable);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
        }
    }
}
