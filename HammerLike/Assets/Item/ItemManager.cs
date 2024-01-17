using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Player player;
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
    }

    void GiveItemToPlayer(int itemId)
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
