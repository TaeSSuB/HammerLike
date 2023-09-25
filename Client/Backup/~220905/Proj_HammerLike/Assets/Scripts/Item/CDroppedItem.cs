using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Tresure, Weapon}

public class CDroppedItem : MonoBehaviour
{
    public ItemType type;
    public GameObject item;
    public SpriteRenderer itemSprite;

    private void OnEnable()
    {
        itemSprite.sprite = item.GetComponentInChildren<SpriteRenderer>().sprite;
    }

    private void OnTriggerEnter2D(Collider2D col2d)
    {
        if(col2d.CompareTag("Player"))
        {
            switch (type)
            {
                case ItemType.Weapon:
                    item.GetComponent<CWeapon>().PickUpWeapon(col2d.GetComponent<CPlayer>());
                    break;
                case ItemType.Tresure:
                    item.GetComponent<CTresure>().PickUpTresureInstance(col2d.GetComponent<CPlayer>());
                    break;
            }
            
            Destroy(gameObject);
        }
    }
}
