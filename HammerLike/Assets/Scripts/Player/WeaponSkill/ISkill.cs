using UnityEngine;

public interface ISkill
{
    void ChargeSkill(Vector3 position, Transform parent, SO_Skill skillData);
    void WeaponAttack(Vector3 position); // 추가
   
}