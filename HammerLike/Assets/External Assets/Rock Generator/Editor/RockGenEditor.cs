using UnityEngine;
using UnityEditor;

namespace JPBotelho
{
    [CustomEditor (typeof (RockGenerator))]
    public class RockGenEditor : Editor
    {
        RockGenerator generator;

        SerializedProperty settings;

        void OnEnable ()
        {
            settings = serializedObject.FindProperty("settings");

            generator = (RockGenerator)target; 
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
			EditorGUI.BeginChangeCheck();
            
                EditorGUILayout.PropertyField(settings, new GUIContent("Settings"), true);

            EditorGUILayout.Space();
            

                generator.seed = EditorGUILayout.IntSlider("Seed", generator.seed, 0, 10000);

                /*if (GUILayout.Button ("Random Seed"))
                {
                    generator.RandomSeed();
                }*/

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();

                /*if (GUILayout.Button("Preview Rock"))
                {
                    generator.GenerateRock(false);
                }*/

                if (GUILayout.Button("New Rock"))
                {
                    generator.GenerateRock(true);
                }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

                if (GUILayout.Button("Export Current"))
                {
                    generator.EditorExport(false);
                }

                if (GUILayout.Button("Export Random"))
                {
                    generator.EditorExport(true);
                }

            if (EditorGUI.EndChangeCheck())
            {
				generator.GenerateRock(false);
			}

            GUILayout.EndHorizontal();
        }        
    }
}