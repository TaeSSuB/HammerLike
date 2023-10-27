using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TreeEditor.TreeEditorHelper;
using UnityEngine;

namespace Assets.Item.Accessories
{
    abstract class Accesories : ItemClass
    {
        // 임시로 넣은 변수
        internal protected float attackPoint_ = 0.0f;                            // 공격력
        internal protected float defensePoint_ = 0.0f;                           // 방어력
        internal protected float konckbackPoint_ = 0.0f;                         // 넉백
        internal protected float attackSpeed_ = 0.0f;                            // 공격속도

        internal protected bool accessories_paasive_ = false;                     // 악세서리 패시브
        protected void Start()
        {
            CSVLoader._instance.SetData(itemNum_, this);
            showStat();
        }
        protected void showStat()
        {
            Debug.Log(itemNum_);
            Debug.Log(itemDesc_);
        }

        protected abstract void passive();
    }
}
