using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Status
{
    public int hp = 0;
    public int atk = 0;
    public int charge = 0;
    public int range = 0;
    public int knockback = 0;
    public float movespeed = 0;
}

public class CTresure : CItem
{
    [Header("Tresure")]
    public Status stat;
    public CPlayer player;
    public GameObject spriteObj;
    public GameObject uiObj;
    public bool isPickUp = false;
    public bool isActive = false;

    public void SetPlayer(CPlayer _player)
    {
        player = _player;
    }

    public void PickUpTresure(CPlayer _player)
    {
        _player.tresureSocket.Add(this);
        GetComponent<Collider2D>().enabled = false;
        spriteObj.SetActive(false);
        transform.SetParent(_player.InventorySocket.transform);

        isPickUp = true;
        SetPlayer(_player);
    }

    public void PickUpTresureInstance(CPlayer _player)
    {
        var instance = Instantiate(this);
        _player.tresureSocket.Add(instance);
        instance.GetComponent<Collider2D>().enabled = false;
        instance.spriteObj.SetActive(false);
        instance.transform.SetParent(_player.InventorySocket.transform);

        var tresureLayout = GameObject.Find("TresureLayout");
        var Icon = Instantiate(instance.uiObj, tresureLayout.transform);
        Icon.GetComponent<Image>().sprite = instance.spriteObj.GetComponent<SpriteRenderer>().sprite;

        instance.transform.localPosition = Vector3Int.zero;
        instance.isPickUp = true;
        instance.SetPlayer(_player);
    }

    private void OnTriggerEnter2D(Collider2D col2d)
    {
        if (col2d.CompareTag("Player"))
        {
            var _player = col2d.GetComponent<CPlayer>();
            PickUpTresure(_player);
            Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }
}
