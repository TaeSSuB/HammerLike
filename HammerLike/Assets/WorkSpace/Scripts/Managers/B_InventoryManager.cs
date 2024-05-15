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
    public SO_ItemDataBase itemDataBase;
    public SO_InventoryObject playerInventory;
    public SO_InventoryObject playerEquipContainer;
    public SO_InventoryObject playerWeaponContainer;

    private void Start() {

    }

    private void Update() {

        WeaponSwitchWithWheel();

    }

    public int GetCurrentWeaponSlotIndex()
    {
        var currentWeaponObj = GameManager.Instance.Player.WeaponData;
        //var equipWeaponIndex = playerWeaponContainer.FindItemOnInventory(currentWeaponObj.itemData).item.index;
        var equipWeaponSlotIndex = playerWeaponContainer.FindSlotIndexWithItem(currentWeaponObj.itemData);

        if(equipWeaponSlotIndex == -1)
        {
            Debug.LogError("Current Weapon Slot Index is -1");
            playerWeaponContainer.AddItem(currentWeaponObj.itemData, 1);
            equipWeaponSlotIndex = playerWeaponContainer.FindSlotIndexWithItem(currentWeaponObj.itemData);
        }

        return equipWeaponSlotIndex;
    }

    void WeaponSwitchWithWheel() {

        if(Input.GetAxis("Mouse ScrollWheel") == 0) return;

        var currentWeaponSlotIndex = GetCurrentWeaponSlotIndex();
        int nextWeaponIndex = currentWeaponSlotIndex;
        int maxWeaponIndex = playerWeaponContainer.Container.GetFilledSlotSize() - 1;

        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            // Scroll Up
            Debug.Log("Scroll Up");

            nextWeaponIndex = currentWeaponSlotIndex + 1;
            
            if (nextWeaponIndex > maxWeaponIndex) {
                nextWeaponIndex = 0;
            }

        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            // Scroll Down
            Debug.Log("Scroll Down");

            nextWeaponIndex = currentWeaponSlotIndex - 1;

            if (nextWeaponIndex < 0) {
                nextWeaponIndex = maxWeaponIndex;
            }

        }

        if(currentWeaponSlotIndex == nextWeaponIndex) return;

        GameManager.Instance.Player.EquipWeapon(playerWeaponContainer.Container.Items[nextWeaponIndex].ItemObject as SO_Weapon);
    }

    public void OnApplicationQuit()
    {
        playerInventory.Container.Clear();
        playerEquipContainer.Container.Clear();
        playerWeaponContainer.Container.Clear();
        playerInventory.Container.Items = new B_InventorySlot[28];
        //playerEquipContainer.Container.Items = new B_InventorySlot[5];
        playerWeaponContainer.Container.Items = new B_InventorySlot[6];
    }
}
