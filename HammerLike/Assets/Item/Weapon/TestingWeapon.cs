using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Item.Weapon
{
    internal class TestingWeapon : Weapon
    {
        protected override void Skill()
        {
            Debug.Log(this.itemRarity_);
        }

    }
}
