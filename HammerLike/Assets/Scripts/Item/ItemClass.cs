using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using ItemInfo;
public abstract class ItemClass : MonoBehaviour
{
    [Header("ItemInformation")]
    internal protected string itemId_;                       // 아이템 id
    [SerializeField]
    internal protected int itemNum_;                         // 아이템 Num
    internal protected ItemRarity itemRarity_;               // 아이템 레어도 
    internal protected bool sell_;                           // 구매가능한 아이템인지
    internal protected ItemType itemType_;                   // 소모품인지
    internal protected string itemDesc_;                     // 아이템 설명
}