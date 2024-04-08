using UnityEngine;

public class B_Equipment : MonoBehaviour
{
    public Transform rightHand; // 플레이어의 오른손 본 Transform. Unity 에디터에서 할당
    private Weapon equippedWeapon;
    [SerializeField] Weapon testWeapon;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 장착할 무기를 찾아서 장착
            Weapon weapon = testWeapon;
            if (weapon != null)
            {
                EquipWeapon(weapon);
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            // 장착된 무기 해제
            UnequipWeapon();
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (equippedWeapon != null)
        {
            // 이미 장착된 무기가 있다면, 장착 해제 또는 교체 로직 처리
            Debug.Log("Weapon already equipped. Consider unequipping first.");
            return;
        }

        equippedWeapon = weapon;
        // Instantiate the weapon prefab and attach it to the right hand
        GameObject weaponInstance = Instantiate(weapon.prefab, rightHand);
        weaponInstance.name = weapon.itemName; // 예시로 prefab이 Weapon 클래스에 포함되어 있다고 가정

        Debug.Log(weapon.itemName + " equipped.");
    }

    // 선택적: 무기 장착 해제 메소드
    public void UnequipWeapon()
    {
        if (equippedWeapon != null)
        {
            // 장착된 무기 제거 로직
            foreach (Transform child in rightHand)
            {
                if (child.name == equippedWeapon.itemName)
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
            equippedWeapon = null;
            Debug.Log("Weapon unequipped.");
        }
    }
}
