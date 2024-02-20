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
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
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
        if(itemId==0)
        {
            SoundManager soundManager = SoundManager.Instance;
            soundManager.PlaySFX(soundManager.audioClip[1]);
        }
        player.inventory.AddItem(itemId);
    }

}
