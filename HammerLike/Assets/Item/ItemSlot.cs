using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{

    public Image itemImage; // 슬롯의 아이템 이미지
    public Item currentItem; // 현재 슬롯의 아이템
    private Inventory inventory; // 부모 인벤토리 참조
    private Vector2 originalPosition; // 드래그 시작 위치
    private int slotIndex;
    public void SetItemSprite(Sprite sprite)
    {
        itemImage.sprite = sprite;
        itemImage.enabled = (sprite != null);
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            inventory = playerObject.GetComponent<Inventory>();
            slotIndex = inventory.itemSlots.IndexOf(this);
            if (inventory == null)
            {
                Debug.LogError("Player 오브젝트에 Inventory 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }

        originalPosition = itemImage.rectTransform.anchoredPosition;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        inventory.SelectItem(slotIndex);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 드래그 시작 시, 아이템 이미지의 위치를 저장
        originalPosition = itemImage.rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중에는 아이템 이미지를 마우스 커서를 따라 이동
        itemImage.rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료 시, 아이템 이미지를 원래 위치로 복원
        //itemImage.rectTransform.anchoredPosition = originalPosition;
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemSlot draggedSlot = eventData.pointerDrag.GetComponent<ItemSlot>();
        GameObject dropTargetObject = eventData.pointerCurrentRaycast.gameObject;
        ItemSlot dropTargetSlot = dropTargetObject != null ? dropTargetObject.GetComponent<ItemSlot>() : null;

        Debug.Log("드래그한 슬롯: " + (draggedSlot != null ? draggedSlot.name : "null"));
        Debug.Log("드롭 대상 객체: " + (dropTargetObject != null ? dropTargetObject.name : "null"));
        Debug.Log("드롭 대상 슬롯: " + (dropTargetSlot != null ? dropTargetSlot.name : "null"));
        if (draggedSlot != null && dropTargetSlot != null && draggedSlot != dropTargetSlot)
        {
            // 드래그한 슬롯과 드롭된 슬롯이 서로 다른 경우

            // 드래그한 슬롯과 드롭된 슬롯의 아이템을 교환
            Item tempItem = dropTargetSlot.currentItem;
            dropTargetSlot.SetItem(draggedSlot.currentItem);
            draggedSlot.SetItem(tempItem);

            // 인벤토리 UI 업데이트
            if (inventory != null)
            {
                inventory.UpdateInventoryUI();
            }
            else
            {
                Debug.LogError("Inventory 참조가 null입니다.");
            }
        }
        else
        {
            Debug.LogError("드래그한 슬롯이 null이거나 드롭 대상 슬롯과 동일합니다.");
        }
    }



    public void SetItem(Item item)
    {
        currentItem = item;
        itemImage.sprite = item != null ? item.itemImage : null;
        itemImage.enabled = (item != null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
