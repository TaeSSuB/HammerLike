using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "B_ScriptableObjects/Skills/BasicSkill")]
public class SO_Skill : ScriptableObject
{
    public string skillName;
    public string description;
    // 스킬의 세부적인 특성 및 효과 구현...
}