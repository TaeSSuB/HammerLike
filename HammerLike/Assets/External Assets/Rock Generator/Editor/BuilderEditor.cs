using UnityEngine;
using UnityEditor;

namespace JPBotelho
{
    [CustomEditor(typeof(RockBuilder))]
    public class BuilderEditor : Editor
    {
        RockBuilder builder;

        void OnEnable ()
        {         
            builder = (RockBuilder)target;
        }

        public override void OnInspectorGUI()
        {
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			base.DrawDefaultInspector();

			EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();

            /*if (GUILayout.Button("Preview Rock"))
            {
                builder.PreviewRock(false);
            }*/

            if (GUILayout.Button("New Rock"))
            {
                builder.RandomSeed();
                builder.PreviewRock(false);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Export Current"))
            {
                builder.PreviewRock(true);
            }

            if (GUILayout.Button("Export Random"))
            {
                builder.RandomSeed();
                builder.PreviewRock(true);
            }

            if(EditorGUI.EndChangeCheck())
            {
				builder.PreviewRock(false);
            }

            GUILayout.EndHorizontal();
			serializedObject.ApplyModifiedProperties();
		}
    }
}