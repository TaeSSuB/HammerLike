using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Item.Weapon
{
    internal class WoodHammer : Weapon
    {
        protected override void Skill()
        {
            if (skill_ == true)
            {
                UnityEngine.Debug.Log(itemDesc_);
            }
        }

    }
}
