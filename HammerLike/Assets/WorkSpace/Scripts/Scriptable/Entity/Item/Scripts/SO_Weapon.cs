using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "B_ScriptableObjects/Inventory/Equip/Weapon")]
public class SO_Weapon : SO_Equipment
{
    // 필요한 추가적인 무기 특성
    //public GameObject prefab; // 무기 프리팹
    [Header("Weapon Data")]
    public SO_Skill itemSkill; // 무기 고유 스킬
    public EnvasionType evasionType = EnvasionType.Roll;

    public int attackPower = 0;
    public int knockbackPower = 0;

    public float attackSpeed;
    public float weaponRange;
    public float weaponSensivity; // 무기 감도
    
    //public Skill unlockableSkill; // 추가 스킬 해금. Skill은 별도로 정의된 ScriptableObject가 될 수 있습니다.

    // Weapon 클래스의 기능 구현...

    public override void Use()
    {
        base.Use();
        // Equip the item
        // Remove it from the inventory
        B_InventoryManager.Instance.playerWeaponContainer.AddItem(this.itemData, 1);
        GameManager.Instance.Player.EquipWeapon(this);
    }
    public void ActivateWeaponSkill(Vector3 position, Transform parent)
    {
        if (itemSkill != null)
        {
            itemSkill.ChargeSkill(position, parent);
        }
    }

    public void WeaponAttack(Vector3 position, Transform parent)
    {
        if (itemSkill != null)
        {
            itemSkill.WeaponAttack(position, parent);
        }
    }
}
