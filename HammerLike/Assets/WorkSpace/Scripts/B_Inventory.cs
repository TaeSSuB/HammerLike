using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Item Slot
[System.Serializable]
public class B_ItemSlot
{
    public SO_Item item;
    public Image slotImage;
    public Image iconImage;
    public int amount = 0;
    public bool isEmpty = true;

    public B_ItemSlot()
    {
        if (amount <= 0)
        {
            isEmpty = true;
        }
    }
}

[System.Serializable]
public class B_EquipmentSlots
{
    public B_ItemSlot head;
    public B_ItemSlot body;
    public B_ItemSlot legs;
    public B_ItemSlot feet;
    public B_ItemSlot leftHand;
    public B_ItemSlot rightHand;
    public B_ItemSlot accessory1;
    public B_ItemSlot accessory2;

    public B_EquipmentSlots()
    {
        head = new B_ItemSlot();
        body = new B_ItemSlot();
        legs = new B_ItemSlot();
        feet = new B_ItemSlot();
        leftHand = new B_ItemSlot();
        rightHand = new B_ItemSlot();
        accessory1 = new B_ItemSlot();
        accessory2 = new B_ItemSlot();
    }

    public List<B_ItemSlot> GetAllSlots()
    {
        List<B_ItemSlot> slots = new List<B_ItemSlot>
        {
            head,
            body,
            legs,
            feet,
            leftHand,
            rightHand,
            accessory1,
            accessory2
        };
        return slots;
    }

    public B_ItemSlot GetSlot(EquipmentCategory type)
    {
        switch (type)
        {
            case EquipmentCategory.Helmet:
                return head;
            case EquipmentCategory.Chest:
                return body;
            case EquipmentCategory.Gloves:
                return legs;
            case EquipmentCategory.Boots:
                return feet;
            case EquipmentCategory.Weapon:
                return leftHand;
            case EquipmentCategory.Shield:
                return rightHand;
            case EquipmentCategory.Accessory:
                return accessory1;
            default:
                return null;
        }
    }
}

public class B_Inventory : MonoBehaviour
{
    public List<B_ItemSlot> B_ItemSlots = new List<B_ItemSlot>();
    public B_EquipmentSlots equipmentSlots = new B_EquipmentSlots();
    public Sprite slotImage;
    public float iconScale = 0.5f;
    public int capacity = 20;

    public Transform inventoryUITr;
    public Transform equipmentUITr;

    private void Start()
    {
        Initialize();
    }

    private void InitializeSlot(B_ItemSlot b_ItemSlot, Transform parentTr)
    {
        GameObject inItemUI = new GameObject();

        b_ItemSlot.slotImage = inItemUI.AddComponent<Image>();
        b_ItemSlot.slotImage.sprite = slotImage;
        b_ItemSlot.slotImage.type = Image.Type.Sliced;

        GameObject inItemUIIcon = new GameObject();
        inItemUIIcon.transform.SetParent(inItemUI.transform);

        b_ItemSlot.iconImage = inItemUIIcon.AddComponent<Image>();
        b_ItemSlot.iconImage.sprite = null;

        inItemUI.transform.SetParent(parentTr);

        inItemUI.transform.localScale = Vector3.one;

        b_ItemSlot.iconImage.rectTransform.sizeDelta = b_ItemSlot.slotImage.rectTransform.sizeDelta * iconScale;
    }

    private void Initialize()
    {
        for (int i = 0; i < capacity; i++)
        {
            var emptySlot = new B_ItemSlot();
            B_ItemSlots.Add(emptySlot);

            InitializeSlot(emptySlot, inventoryUITr);
        }

        foreach (var slot in equipmentSlots.GetAllSlots())
        {
            var emptySlot = new B_ItemSlot();

            InitializeSlot(emptySlot, equipmentUITr);
        }
    }

    public void AddItem(SO_Item item, int amount = 1)
    {
        foreach (var slot in B_ItemSlots)
        {
            if (slot.isEmpty)
            {
                slot.item = item;
                slot.amount += amount;
                slot.iconImage.sprite = item.itemIcon;
                slot.isEmpty = false;
                break;
            }
        }
    }

    public void RemoveItem(SO_Item item, int amount = 1)
    {
        foreach (var slot in B_ItemSlots)
        {
            if (slot.item == item)
            {
                slot.item = null;
                slot.amount -= amount;
                if (slot.amount <= 0)
                {
                    slot.iconImage.sprite = null;
                    slot.amount = 0;
                    slot.isEmpty = true;
                }
                break;
            }
        }
    }

    public void UseItem(Item item)
    {
        //item.Use();
    }

    public void DropItem(Item item)
    {
        //item.Drop();
    }
}
