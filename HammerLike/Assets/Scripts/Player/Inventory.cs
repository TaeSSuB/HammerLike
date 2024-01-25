using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject invenUI; // SetActive를 반전시킬 대상 오브젝트
    public List<Item> items = new List<Item>(16); // 인벤토리 아이템들
    public List<ItemSlot> itemSlots;
    private int selectedSlotIndex = -1; // 현재 선택된 슬롯 인덱스
    private Item selectedItem; // 현재 선택된 아이템

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

        if(invenUI.activeSelf)
        {
            HandleInput();
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
                //Debug.Log("아이템이 인벤토리 슬롯 " + i + "에 추가되었습니다.");
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
            //Debug.Log("슬롯 " + i + " 업데이트: " + (sprite != null ? sprite.name : "빈 슬롯"));
        }
    }

    public void SwapItems(int index1, int index2)
    {
        if (index1 < 0 || index1 >= items.Count || index2 < 0 || index2 >= items.Count)
            return;

        Item temp = items[index1];
        items[index1] = items[index2];
        items[index2] = temp;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelection(4); // 아래로 이동
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelection(-4); // 위로 이동
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelection(-1); // 왼쪽으로 이동
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelection(1); // 오른쪽으로 이동
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmSelection(); // 선택 확정
        }
        else if (Input.GetKeyDown(KeyCode.Home))
        {
            SwapSlots(0, 1);
        }
    }

    private void MoveSelection(int offset)
    {
        if (selectedSlotIndex == -1) return;

        int newSlotIndex = Mathf.Clamp(selectedSlotIndex + offset, 0, items.Count - 1);
        if (newSlotIndex != selectedSlotIndex)
        {
            // 아이템 데이터 교환
            Item tempItem = items[newSlotIndex];
            items[newSlotIndex] = items[selectedSlotIndex];
            items[selectedSlotIndex] = tempItem;

            // 슬롯 UI 업데이트
            itemSlots[newSlotIndex].SetItem(items[newSlotIndex]);
            itemSlots[selectedSlotIndex].SetItem(items[selectedSlotIndex]);

            UpdateInventoryUI(); // UI 업데이트
            selectedSlotIndex = newSlotIndex; // 선택된 슬롯 업데이트
        }
    }


    private void SwapFirstTwoSlots()
    {
        if (items.Count >= 2)
        {
            // 첫 번째와 두 번째 아이템 교환
            Item temp = items[0];
            items[0] = items[1];
            items[1] = temp;

            // 슬롯 UI 업데이트
            itemSlots[0].SetItem(items[0]);
            itemSlots[1].SetItem(items[1]);

            UpdateInventoryUI(); // UI 업데이트
        }
    }

    private void SwapSlots(int index1, int index2)
    {
        // 유효한 인덱스 확인
        if (index1 >= 0 && index1 < items.Count && index2 >= 0 && index2 < items.Count)
        {
            // 아이템 교환
            Item temp = items[index1];
            items[index1] = items[index2];
            items[index2] = temp;

            // 슬롯 UI 업데이트
            itemSlots[index1].SetItem(items[index1]);
            itemSlots[index2].SetItem(items[index2]);

            UpdateInventoryUI(); // UI 업데이트
        }
    }

    private void ConfirmSelection()
    {
        // 최종 선택한 슬롯에 아이템 배치
        selectedItem = null;
        selectedSlotIndex = -1;
    }

    // 아이템을 선택하는 메서드 (예: 아이템 클릭 시 호출)
    public void SelectItem(int slotIndex)
    {
        selectedSlotIndex = slotIndex;
        selectedItem = itemSlots[slotIndex].currentItem;
    }
}
