using ItemInfo.Eweapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ItemInfo
{
    public enum ItemRarity              // 아이템 등급
    {
        normal = 1,
        rare,
        epic,
        legendary
    }
    public enum ItemType
    {
        weapon = 1,
        accesories,
        spend
    }

    namespace Eweapon
    {
        public enum AnimationType           // 애니메이션 타입
        {
            one_handed = 1,
            two_handed
        }
        public enum DodgeType               // 이동기
        {
            dash = 1,
            tumbling,
            Shunbo
        }
    }

    internal struct Field
    {
        public int itemNum_;
        public string itemId_;
        public ItemRarity itemRarity;         // enum   1=normal, 2=rare, 3=epic, 4=legendary
        public bool sell_;
        public ItemType itemType_;          // enum   1=weapon, 2=accesories, 3=spend
        public string itemDesc_;
        /// weapon var
        public float attackPoint_;
        public float defensePoint_;
        public float konckbackPoint_;
        public float attackSpeed_;
        public Eweapon.AnimationType animationType_;     // enum 1=one_handed, 2=two_handed
        public bool skill_;
        public Eweapon.DodgeType dodgeType_;         // enum 1=dash, 2=tumbling, 3=Shunbo
        /// accesories var      // 악세사리에 들어갈 변수들
    }

}