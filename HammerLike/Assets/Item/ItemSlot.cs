using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IDropHandler
{

    public Image itemImage; // 슬롯의 아이템 이미지
    private Item currentItem; // 현재 슬롯의 아이템
    private Inventory inventory; // 부모 인벤토리 참조
    private Vector2 originalPosition; // 드래그 시작 위치

    public void SetItemSprite(Sprite sprite)
    {
        itemImage.sprite = sprite;
        itemImage.enabled = (sprite != null);
    }
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponentInParent<Inventory>();
        originalPosition = itemImage.rectTransform.anchoredPosition;
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
        itemImage.rectTransform.anchoredPosition = originalPosition;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 다른 슬롯에 드롭했을 때의 처리
        ItemSlot droppedSlot = eventData.pointerDrag.GetComponent<ItemSlot>();
        if (droppedSlot != null && droppedSlot != this)
        {
            // 아이템 교환 처리
            Item tempItem = currentItem;
            SetItem(droppedSlot.currentItem);
            droppedSlot.SetItem(tempItem);
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
