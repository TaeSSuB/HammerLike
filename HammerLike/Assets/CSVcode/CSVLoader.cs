using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Text;
using UnityEditor.Build.Content;
using UnityEngine;

using ItemInfo;
using Assets.Item.Weapon;
using ItemInfo.Eweapon;
using Assets.Item.Accessories;
using Assets.Item.Spend;

public class CSVLoader : MonoBehaviour
{
    public static CSVLoader _instance = null;
    private TextAsset data = null;
    private Dictionary<int, Field> _itemDictionary = new();  // itemNum을 key로,내용을 밸류로

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            LoadInfo(); // 처음 시작할때 data를 받아온다.
            // 이렇게 하면 데이터도 싱글톤패턴처럼 전역적으로 접근할 수 있나?
            //DontDestroyOnLoad(_instance);
        }
        else
        {
            if (_instance != null)
            {
                Destroy(this.gameObject);
            }
        }

    }
    private void LoadInfo()
    {
        data = Resources.Load("ItemInfo") as TextAsset;
        if (data != null)
        {
            /// 모든 내용을 줄단위로 string배열에 넣는다.
            string[] lines = Resources.Load<TextAsset>("ItemInfo").text.Split('\n');

            foreach (string line in lines)
            {
                string[] fields = line.Split(',');      // ,로 나눠지는 string배열 한줄

                if(fields.Length != 1)
                {
                    // itemData(한줄)에 data모두 집어넣기
                    ItemInfo.Field readData = new();
                    if (int.TryParse(fields[0], out int value0))
                    {
                        readData.itemNum_ = value0;
                    }
                    readData.itemId_ = fields[1];
                    if (Enum.TryParse(fields[2], out ItemRarity parsedValue))
                    {
                        readData.itemRarity = parsedValue;
                    }
                    if (bool.TryParse(fields[3], out bool value3))    //bool 값 변수
                    {
                        readData.sell_ = value3;
                    }
                    if (Enum.TryParse(fields[4], out ItemType fields4))
                    {
                        readData.itemType_ = fields4;
                    }
                    readData.itemDesc_ = fields[5];
                    /// weapon
                    if (float.TryParse(fields[6], out float Value6))
                    {
                        readData.attackPoint_ = Value6;
                    }
                    if (float.TryParse(fields[7], out float Value7))
                    {
                        readData.attackPoint_ = Value7;
                    }
                    if (float.TryParse(fields[8], out float Value8))
                    {
                        readData.konckbackPoint_ = Value8;
                    }
                    if (float.TryParse(fields[9], out float Value9))
                    {
                        readData.konckbackPoint_ = Value9;
                    }
                    if (Enum.TryParse(fields[8], out ItemInfo.Eweapon.AnimationType Value10))
                    {
                        readData.animationType_ = Value10;
                    }
                    if (bool.TryParse(fields[9], out bool Value11))
                    {
                        readData.skill_ = Value11;
                    }
                    if (Enum.TryParse(fields[12], out ItemInfo.Eweapon.DodgeType value12))      // M
                    {
                        readData.dodgeType_ = value12;
                    }
                    /// spend
                    if (int.TryParse(fields[13], out int Value13))                              // N
                    {
                        readData.spendCount_ = Value13;
                    }
                    /// accesories
                    if (bool.TryParse(fields[14], out bool Value14))
                    {
                        readData.accessories_paasive_ = Value14;
                    }

                    /// add all var
                    _itemDictionary.Add(readData.itemNum_, readData);
                    Debug.Log("sucess to add : " + readData.itemNum_);
                }
                else
                {
                    break;
                }
  
            }
        }
        else
        {
            Debug.LogWarning("cannot read ItemInfo csvFile");
        }
    }
    internal void SetData(int itemNumber, Weapon outputWeapon)
    {
        if (_itemDictionary.TryGetValue(itemNumber, out Field fieldValue))
        {
            if (itemNumber == outputWeapon.itemNum_)
            {
                outputWeapon.itemId_ = fieldValue.itemId_;
                outputWeapon.itemRarity_ = fieldValue.itemRarity;
                outputWeapon.sell_ = fieldValue.sell_;
                outputWeapon.itemType_ = fieldValue.itemType_;
                outputWeapon.itemDesc_ = fieldValue.itemDesc_;
                ///Weapon
                outputWeapon.attackPoint_ = fieldValue.attackPoint_;
                outputWeapon.defensePoint_ = fieldValue.defensePoint_;
                outputWeapon.konckbackPoint_ = fieldValue.konckbackPoint_;
                outputWeapon.attackSpeed_ = fieldValue.attackSpeed_;
                outputWeapon.animationType_ = (Weapon.AnimationType)fieldValue.animationType_;
                outputWeapon.skill_ = fieldValue.skill_;
                outputWeapon.dodgeType_ = (Weapon.DodgeType)fieldValue.dodgeType_;
            }
        }
        else
        {
            Debug.Log("CSVLoader::Wrong itemNum : " + itemNumber);
        }
    }

    internal void SetData(int itemNumber, Accesories outputAccesories)
    {
        if (_itemDictionary.TryGetValue(itemNumber, out Field fieldValue))
        {
            if (itemNumber == outputAccesories.itemNum_)
            {
                outputAccesories.itemId_ = fieldValue.itemId_;
                outputAccesories.itemRarity_ = fieldValue.itemRarity;
                outputAccesories.sell_ = fieldValue.sell_;
                outputAccesories.itemType_ = fieldValue.itemType_;
                outputAccesories.itemDesc_ = fieldValue.itemDesc_;
                /// Accesories
                outputAccesories.attackPoint_ = fieldValue.attackPoint_;
                outputAccesories.defensePoint_ = fieldValue.defensePoint_;
                outputAccesories.attackSpeed_ = fieldValue.attackSpeed_;
                outputAccesories.konckbackPoint_ = fieldValue.konckbackPoint_;

                outputAccesories.accessories_paasive_ = fieldValue.accessories_paasive_;
            }
        }
    }
    internal void SetData(int itemNumber, Spend outputSpend)
    {
        if (_itemDictionary.TryGetValue(itemNumber, out Field fieldValue))
        {
            if(itemNumber == outputSpend.itemNum_)
            {
                outputSpend.itemId_ = fieldValue.itemId_;
                outputSpend.itemRarity_ = fieldValue.itemRarity;
                outputSpend.sell_ = fieldValue.sell_;
                outputSpend.itemType_ = fieldValue.itemType_;
                outputSpend.itemDesc_ = fieldValue.itemDesc_;
                /// Spend
                outputSpend.spendCount = fieldValue.spendCount_;
                
            }
        }
    }

}
