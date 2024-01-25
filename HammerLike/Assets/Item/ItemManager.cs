using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Player player;
    //public bool isDropped = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Insert))
        {
            GiveItemToPlayer(1);
            GiveItemToPlayer(0);
        }
        
    }

    public void DropItem(int itemId, Vector3 position)
    {
        Item itemToDrop = ItemDB.Instance.GetItemByID(itemId);
        if (itemToDrop != null)
        {
            GameObject droppedItem = Instantiate(itemToDrop.itemPrefab, position, Quaternion.identity);
            Rigidbody rb = droppedItem.AddComponent<Rigidbody>();

            FloatingItem floatingItem = droppedItem.AddComponent<FloatingItem>();
            floatingItem.itemManager = this; // ItemManager 참조 추가
            floatingItem.itemId = itemToDrop.itemID; // 아이템 ID 설정
        }
    }




    public void GiveItemToPlayer(int itemId)
    {
        // AddItem 메소드를 한 번만 호출
        bool isAdded = player.inventory.AddItem(itemId);
        if (isAdded)
        {
            Debug.Log("아이템이 성공적으로 추가되었습니다.");
        }
        else
        {
            Debug.LogError("아이템을 추가하지 못했습니다.");
        }
    }

}
