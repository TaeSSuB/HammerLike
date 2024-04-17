using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Equipment/Weapon")]
public class Weapon : EquipItem
{    
    // 필요한 추가적인 무기 특성
    public GameObject prefab; // 무기 프리팹

    public int damage; 
    public float weight;
    public float knockback;

    public ElementalType elementalType;
    public Skill unlockableSkill; // 추가 스킬 해금. Skill은 별도로 정의된 ScriptableObject가 될 수 있습니다.

    // Weapon 클래스의 기능 구현...
}
