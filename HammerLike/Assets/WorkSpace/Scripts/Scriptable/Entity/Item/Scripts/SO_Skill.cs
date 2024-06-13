using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "B_ScriptableObjects/Skills/BasicSkill")]
public class SO_Skill : ScriptableObject
{
    public string skillName;        // 액티브 스킬(차지강화)
    public string description;

    // 스킬의 세부적인 특성 및 효과 구현...
    public GameObject skillPrefab;

    public void ChargeSkill(Vector3 position, Transform parent)
    {
        if (skillPrefab != null)
        {
            GameObject skillInstance = Instantiate(skillPrefab, position, Quaternion.identity, parent);
            ISkill skill = skillInstance.GetComponent<ISkill>();
            if (skill != null)
            {
                skill.ChargeSkill(position, parent, this);
            }
        }
        else
        {
            // 기본 넉백 (스킬 프리팹이 없을 때)
        }
    }

    public void PassiveSkill()
    {

    }
}