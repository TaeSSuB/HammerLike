using UnityEngine;

[CreateAssetMenu(fileName = "New MiscItem", menuName = "Inventory/Misc")]
public class MiscItem : ItemBase
{
    // 기타 아이템에 특화된 속성 추가 가능

    public override void Use()
    {
        // 기타 아이템 사용 로직 구현
        Debug.Log(itemName + " used in a special way!");
    }
}