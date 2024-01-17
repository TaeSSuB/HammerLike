using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject invenUI; // SetActive를 반전시킬 대상 오브젝트
    public List<Item> items = new List<Item>(16); // 인벤토리 아이템들
    public List<ItemSlot> itemSlots;
    void Start()
    {
        // 빈 아이템으로 초기화하거나 미리 정의된 아이템으로 채움
        for (int i = 0; i < 16; i++)
        {
            items.Add(null);
        }
    }


    // Update is called once per frame
    void Update()
    {
        // ESC 키가 눌렸는지 확인
        if (Input.GetKeyDown(KeyCode.I))
        {
            // targetObject의 활성 상태를 반전
            invenUI.SetActive(!invenUI.activeSelf);
            if (invenUI.activeSelf)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    public void AddItem(Item item)
    {
        bool isAdded = false;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                UpdateInventoryUI(); // 인벤토리 UI 업데이트
                isAdded = true;
                Debug.Log("아이템이 인벤토리 슬롯 " + i + "에 추가되었습니다.");
                break;
            }
        }

        if (!isAdded)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
        }
    }


        public bool AddItem(int itemId)
    {
        // ItemDB에서 아이템 찾기
        Item itemToAdd = ItemDB.Instance.GetItemByID(itemId);

        if (itemToAdd == null)
        {
            Debug.LogError("Item not found in ItemDB!");
            return false;
        }

        // 인벤토리에 빈 공간 찾아 아이템 추가
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = itemToAdd;
                // UI 업데이트나 기타 처리
                UpdateInventoryUI();
                return true;
            }
        }

        // 인벤토리에 공간이 없는 경우
        Debug.Log("Inventory is full!");
        return false;
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            Sprite sprite = (i < items.Count && items[i] != null) ? items[i].itemImage : null;
            itemSlots[i].SetItemSprite(sprite);
            Debug.Log("슬롯 " + i + " 업데이트: " + (sprite != null ? sprite.name : "빈 슬롯"));
        }
    }
}
