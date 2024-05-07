using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;

public abstract class UI_UserInterface : MonoBehaviour
{
    public SO_InventoryObject inventory;
    private SO_InventoryObject previousInventory;

    public Dictionary<GameObject, B_InventorySlot> slotsOnInterface = new Dictionary<GameObject, B_InventorySlot>();

    protected void Awake()
    {
        if (inventory == null)
        {
            inventory = B_InventoryManager.Instance.playerInventory;
        }
        else
        {
            Debug.Log($"{this.name} : My Inventory already!");
        }

        Init();
    }

    public void Init()
    {
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            inventory.Container.Items[i].parent = this;
        }
        CreateSlots();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    public abstract void CreateSlots();

    public void UpdateInventoryLinks()
    {
        int i = 0;
        foreach (var key in slotsOnInterface.Keys.ToList())
        {
            slotsOnInterface[key] = inventory.Container.Items[i];
            i++;
        }
    }


    public void Update()
    {
        // 인벤토리 임의 변경 시 인터페이스에 반영
        if (previousInventory != inventory)
        {
            UpdateInventoryLinks();
        }
        previousInventory = inventory;
        slotsOnInterface.UpdateSlotDisplay();

    }
    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (!trigger) { Debug.LogWarning("No EventTrigger component found!"); return; }
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        Debug.Log("OnEnter");
        MouseData.slotHoveredOver = obj;
    }
    public void OnEnterInterface(GameObject obj)
    {
        Debug.Log("OnEnterInterface");
        MouseData.interfaceMouseIsOver = obj.GetComponent<UI_UserInterface>();
    }
    public void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }

    public void OnExit(GameObject obj)
    {
        MouseData.slotHoveredOver = null;
    }

    public void OnDragStart(GameObject obj)
    {
        MouseData.tempItemBeingDragged = CreateTempItem(obj);
    }
    private GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;
        if (slotsOnInterface[obj].item.index >= 0)
        {
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            tempItem.transform.SetParent(transform.parent);
            var img = tempItem.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].ItemObject.itemIcon;
            img.raycastTarget = false;
        }
        return tempItem;
    }
    public void OnDragEnd(GameObject obj)
    {

        Destroy(MouseData.tempItemBeingDragged);

        if (MouseData.interfaceMouseIsOver == null)
        {
            slotsOnInterface[obj].RemoveItem();
            return;
        }
        if (MouseData.slotHoveredOver)
        {
            B_InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
        }
    }

    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemBeingDragged != null)
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
    }
}

public static class MouseData
{
    public static UI_UserInterface interfaceMouseIsOver;
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoveredOver;
}

public static class ExtensionMethods
{
    public static void UpdateSlotDisplay(this Dictionary<GameObject, B_InventorySlot> _slotsOnInterface)
    {
        foreach (KeyValuePair<GameObject, B_InventorySlot> _slot in _slotsOnInterface)
        {
            if (_slot.Value.item.index <= -1)
            {
                _slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = null;
                _slot.Key.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
            else
            {
                _slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = _slot.Value.ItemObject.itemIcon;
                _slot.Key.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
            }
        }
    }
}