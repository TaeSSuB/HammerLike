using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public GameObject invenUI; // 인벤토리 UI 오브젝트
    public List<Item> items = new List<Item>(16); // 인벤토리 아이템 리스트
    public List<ItemSlot> itemSlots; // 아이템 슬롯 리스트
    public TMP_Text goldText; // 골드 수량을 표시할 Text Mesh Pro UI 컴포넌트
    public int goldItemId; // 골드 아이템의 ID

    private int selectedSlotIndex = -1; // 현재 선택된 슬롯 인덱스
    private Item selectedItem; // 현재 선택된 아이템

    [Header("Quick Slot UI")]
    public Image[] quickSlotUIImages;
    public TMP_Text[] quickSlotQuantityTexts;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // 필요한 초기화 코드 추가
    }

    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            items.Add(null);
        }
        UpdateQuickSlotUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
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

        if (invenUI.activeSelf)
        {
            HandleInput();
        }

        // 아이템 사용 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseItem(0); // 0번 슬롯 아이템 사용
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseItem(1); // 1번 슬롯 아이템 사용
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseItem(2); // 2번 슬롯 아이템 사용
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseItem(3); // 3번 슬롯 아이템 사용
        }
    }


    private void UseItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < items.Count)
        {
            Item itemToUse = items[slotIndex];
            if (itemToUse != null && itemToUse.itemID == 1) // Item ID가 1인 경우
            {
                Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                if (player != null)
                {
                    player.RecoverHp(30); // HP 30 회복
                    RemoveItem(slotIndex); // 아이템 사용 후 제거
                }
            }
        }
    }

    // 아이템 제거 메서드
    private void RemoveItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < items.Count && items[slotIndex] != null)
        {
            // 아이템 수량 감소
            items[slotIndex].quantity--;

            // 수량이 0이하가 되면 슬롯을 null로 설정
            if (items[slotIndex].quantity <= 0)
            {
                items[slotIndex] = null;
            }

            UpdateInventoryUI();
            UpdateQuickSlotUI();
        }
    }

    public void AddItem(int itemId)
    {
        Item itemToAdd = ItemDB.Instance.GetItemByID(itemId);
        if (itemToAdd == null)
        {
            Debug.LogError("Item not found in ItemDB!");
            return;
        }

        // Check for existing or empty QuickSlot (0-3) for Used items
        if (itemToAdd.itemType == Item.ItemType.Used)
        {
            for (int i = 0; i < 4; i++)
            {
                if (items[i] != null && items[i].itemID == itemToAdd.itemID && items[i].quantity < itemToAdd.limitNumber)
                {
                    items[i].quantity++;
                    UpdateInventoryUI();
                    UpdateQuickSlotUI();
                    return;
                }
                else if (items[i] == null)
                {
                    items[i] = new Item(itemToAdd.itemName, itemToAdd.itemID, itemToAdd.itemType, itemToAdd.itemImage, itemToAdd.limitNumber, 1);
                    UpdateInventoryUI();
                    UpdateQuickSlotUI();
                    return;
                }
            }
        }

        // Check for stackable items or empty slot from slot 4
        for (int i = 4; i < items.Count; i++)
        {
            if (items[i] != null && items[i].itemID == itemToAdd.itemID && items[i].quantity < itemToAdd.limitNumber)
            {
                items[i].quantity++;
                UpdateInventoryUI();
                return;
            }
            else if (items[i] == null)
            {
                items[i] = new Item(itemToAdd.itemName, itemToAdd.itemID, itemToAdd.itemType, itemToAdd.itemImage, itemToAdd.limitNumber, 1);
                UpdateInventoryUI();
                return;
            }
        }

        Debug.Log("Inventory is full or no appropriate slot found!");
    }

    private void UpdateQuickSlotUI()
    {
        for (int i = 0; i < 4; i++) // 첫 4개의 슬롯만 확인
        {
            if (items[i] != null)
            {
                quickSlotUIImages[i].sprite = items[i].itemImage;
                quickSlotUIImages[i].color = new Color(1f, 1f, 1f, 1f); // 풀 알파 값으로 설정 (불투명)
                quickSlotUIImages[i].enabled = true;

                if (items[i].quantity > 1) // 수량이 1보다 클 경우에만 수량 표시
                {
                    quickSlotQuantityTexts[i].text = items[i].quantity.ToString();
                    quickSlotQuantityTexts[i].enabled = true;
                }
                else
                {
                    quickSlotQuantityTexts[i].enabled = false;
                }
            }
            else
            {
                quickSlotUIImages[i].color = new Color(1f, 1f, 1f, 0f); // 알파 값을 0으로 설정 (투명)
                quickSlotUIImages[i].enabled = false;
                quickSlotQuantityTexts[i].enabled = false;
            }
        }
    }


    public void UpdateInventoryUI()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i] != null)
            {
                if (i < items.Count && items[i] != null)
                {
                    itemSlots[i].SetItem(items[i]);
                }
                else
                {
                    itemSlots[i].SetItem(null);
                }
            }
        }

        UpdateGoldUI();
    }

    public void UpdateGoldUI()
    {
        int totalGold = GetTotalGoldAmount(goldItemId);
        goldText.text = totalGold.ToString() + "G";
    }

    private int GetTotalGoldAmount(int goldItemId)
    {
        int totalGold = 0;
        foreach (Item item in items)
        {
            if (item != null && item.itemID == goldItemId)
            {
                totalGold += item.quantity;
            }
        }
        return totalGold;
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
