using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TreeEditor.TreeEditorHelper;
using UnityEngine;

namespace Assets.Item.Weapon
{
    abstract class Weapon : ItemClass
    {
        internal protected enum AnimationType           // 애니메이션 타입
        {
            one_handed = 1,
            two_handed
        }
        internal protected enum DodgeType               // 이동기
        {
            dash=1,
            tumbling,
            Shunbo
        }
        [Header("ItemStatus")]
        internal protected float attackPoint_ = 0.0f;                            // 공격력
        internal protected float defensePoint_ = 0.0f;                           // 방어력
        internal protected float konckbackPoint_ = 0.0f;                         // 넉백
        internal protected float attackSpeed_ = 0.0f;                            // 공격속도
        internal protected AnimationType animationType_ = (AnimationType)1;      // 애니메이션 타입
        internal protected bool skill_ = false;                                  // 스킬이있는 아이템인지
        internal protected DodgeType dodgeType_ = (DodgeType)1;                  // 이동기
        protected abstract void Skill();    // 모든 무기에는 스킬이 있다고 가정
        protected void Start()
        {
            CSVLoader._instance.SetData(itemNum_, this);
            showStat();
        }

        protected void showStat()
        {
            Debug.Log(itemNum_);
            Debug.Log(itemDesc_);
            Debug.Log(dodgeType_);
        }
    }
}
