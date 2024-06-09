using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;
using UnityEditor.Rendering;

/// <summary>
/// 인벤토리 오브젝트 클래스
/// 인벤토리 기능 관련 모음
/// </summary>

[CreateAssetMenu(fileName = "New Inventory", menuName = "B_ScriptableObjects/InventoryObject")]
public class SO_InventoryObject : ScriptableObject
{
    public string savePath;
    public SO_ItemDataBase database;
    //public int MAX_ITEMS;
    public B_Inventory Container;
    public int goldAmount = 0;

    public bool AddItem(B_Item _item, int _amount)
    {
        if (EmptySlotCount <= 0)
            return false;
        B_InventorySlot slot = FindItemOnInventory(_item);
        if (!database.GetItem[_item.index].stackable || slot == null)
        {
            Debug.Log("setting slot");
            SetEmptySlot(_item, _amount);
            return true;
        }
        slot.AddAmount(_amount);
        return true;
    }

    public void AddGold(int _amount)
    {
        goldAmount += _amount;
        Debug.Log("Gold Amount : " + goldAmount);
    }

    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < Container.Items.Length; i++)
            {
                if (Container.Items[i].item.index <= -1)
                {
                    counter++;
                }
            }
            return counter;
        }
    }

    public B_InventorySlot FindItemOnInventory(B_Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.index == _item.index)
            {
                return Container.Items[i];
            }
        }
        return null;
    }

    public B_InventorySlot FindSlotWithItemIndex(int idx)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.index == idx)
            {
                return Container.Items[i];
            }
        }
        return null;
    }

    public int FindSlotIndexWithItem(B_Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.index == _item.index)
            {
                return i;
            }
        }
        return -1;
    }

    public B_InventorySlot SetEmptySlot(B_Item _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.index <= -1)
            {
                Container.Items[i].UpdateSlot(_item, _amount);
                return Container.Items[i];
            }
        }
        return null;
    }

    public void SwapItems(B_InventorySlot item1, B_InventorySlot item2)
    {
        if (item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject))
        {
            B_InventorySlot temp = new B_InventorySlot(item2.item, item2.amount);
            item2.UpdateSlot(item1.item, item1.amount);
            item1.UpdateSlot(temp.item, temp.amount);
        }

    }

    #region SaveLoad

    [ContextMenu("Save")]
    public void Save()
    {
        //string saveData = JsonUtility.ToJson(Container, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //bf.Serialize(file, saveData);
        //file.Close();

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), Container);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            B_Inventory newContainer = (B_Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < Container.Items.Length; i++)
            {
                Container.Items[i].UpdateSlot(newContainer.Items[i].item, newContainer.Items[i].amount);
            }
            stream.Close();
        }
    }
    #endregion

    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }

}
[System.Serializable]
public class B_Inventory
{
    public B_InventorySlot[] Items = new B_InventorySlot[24];
    
    public void Clear()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] != null)
                Items[i].RemoveItem();
        }
    }

    public int GetFilledSlotSize()
    {
        int counter = 0;
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].item.index >= 0)
            {
                counter++;
            }
        }
        Debug.Log("Filled Slot Size : " + counter);

        return counter;
    }
}

[System.Serializable]
public class B_InventorySlot
{
    public ItemType[] AllowedItems = new ItemType[0];
    [System.NonSerialized]
    public UI_UserInterface parent;
    public B_Item item;
    public int amount;

    public SO_Item ItemObject
    {
        get
        {
            if (item.index >= 0)
            {
                B_InventoryManager.Instance.itemDataBase.GetItem.TryGetValue(item.index, out SO_Item itemObject);
                return itemObject;
            }
            return null;
        }
    }

    public B_InventorySlot()
    {
        item = null;
        amount = 0;
    }
    public B_InventorySlot(B_Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
    public void UpdateSlot(B_Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
    public void RemoveItem()
    {
        if (item != null)
            item = null;

        amount = 0;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
    public bool CanPlaceInSlot(SO_Item _itemObject)
    {
        if (AllowedItems.Length <= 0 || _itemObject == null || _itemObject.itemData.index < 0)
            return true;
        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (_itemObject.itemType == AllowedItems[i])
                return true;
        }
        return false;
    }
}
