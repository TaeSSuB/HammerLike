using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NuelLib;

/// <summary>
/// 인벤토리 매니저
/// 플레이어 인벤토리, 착장 정보 DB(SO)
/// </summary>
public class B_InventoryManager : SingletonMonoBehaviour<B_InventoryManager>
{
    public SO_InventoryObject playerInventory;
    public SO_InventoryObject playerEquipContainer;

    public void OnApplicationQuit()
    {
        playerInventory.Container.Clear();
        playerEquipContainer.Container.Clear();
        playerInventory.Container.Items = new B_InventorySlot[28];
    }
}
