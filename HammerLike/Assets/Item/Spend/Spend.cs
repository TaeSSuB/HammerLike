using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TreeEditor.TreeEditorHelper;
using UnityEngine;

namespace Assets.Item.Spend
{
    abstract class Spend : ItemClass
    {
        /// Spend var
        internal protected int spendCount = 0;  // 사용횟수

        protected abstract void Effect();
        protected void Start()
        {
            CSVLoader._instance.SetData(itemNum_, this);
            showStat();
        }
        protected void showStat()
        {
            Debug.Log(itemNum_);
            Debug.Log(itemDesc_);
            Debug.Log(itemRarity_);
        }
    }
}
