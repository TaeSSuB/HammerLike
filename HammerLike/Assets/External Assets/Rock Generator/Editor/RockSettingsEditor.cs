using UnityEditor;
using UnityEngine;

namespace JPBotelho
{
    [CustomPropertyDrawer(typeof(RockSettings))]
    public class RockSettingsEditor : PropertyDrawer
    {
        bool fold;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {            
            property.serializedObject.Update();        

            SerializedProperty verts = property.FindPropertyRelative("verts");
            SerializedProperty randomType = property.FindPropertyRelative("randomType");
            SerializedProperty max = property.FindPropertyRelative("size");
            SerializedProperty product = property.FindPropertyRelative("radius");

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(verts, new GUIContent("Vertices"));

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(randomType, new GUIContent("Type"));

            if (randomType.enumValueIndex == 0)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(max, new GUIContent("Dimensions"));
            }
            else
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(product, new GUIContent("Radius"));
            }
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}