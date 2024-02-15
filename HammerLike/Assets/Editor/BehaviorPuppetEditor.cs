using UnityEngine;
using UnityEditor;
using RootMotion.Dynamics;

[CustomEditor(typeof(BehaviourPuppet))]
public class BehaviourPuppetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BehaviourPuppet script = (BehaviourPuppet)target;

        // 기본 Inspector 옵션 보여주기
        DrawDefaultInspector();

        EditorGUILayout.Space();

        // 상태 설정
        script.masterProps.normalMode = (BehaviourPuppet.NormalMode)EditorGUILayout.EnumPopup("Normal Mode", script.masterProps.normalMode);
        script.masterProps.mappingBlendSpeed = EditorGUILayout.FloatField("Mapping Blend Speed", script.masterProps.mappingBlendSpeed);
        script.masterProps.activateOnStaticCollisions = EditorGUILayout.Toggle("Activate On Static Collisions", script.masterProps.activateOnStaticCollisions);
        script.masterProps.activateOnImpulse = EditorGUILayout.FloatField("Activate On Impulse", script.masterProps.activateOnImpulse);

        EditorGUILayout.Space();

        // Muscle Props 설정
        SerializedProperty muscleProps = serializedObject.FindProperty("defaults");
        EditorGUILayout.PropertyField(muscleProps, true);

        EditorGUILayout.Space();

        // Muscle Props Group 설정
        SerializedProperty musclePropsGroups = serializedObject.FindProperty("groupOverrides");
        EditorGUILayout.PropertyField(musclePropsGroups, true);

        EditorGUILayout.Space();

        // 기타 설정
        script.groundLayers = EditorGUILayout.LayerField("Ground Layers", script.groundLayers);
        script.collisionLayers = EditorGUILayout.LayerField("Collision Layers", script.collisionLayers);
        script.collisionThreshold = EditorGUILayout.FloatField("Collision Threshold", script.collisionThreshold);

        EditorGUILayout.Space();

        // 이벤트
        SerializedProperty onGetUpProne = serializedObject.FindProperty("onGetUpProne");
        EditorGUILayout.PropertyField(onGetUpProne);

        SerializedProperty onGetUpSupine = serializedObject.FindProperty("onGetUpSupine");
        EditorGUILayout.PropertyField(onGetUpSupine);

        // 변경사항 적용
        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
