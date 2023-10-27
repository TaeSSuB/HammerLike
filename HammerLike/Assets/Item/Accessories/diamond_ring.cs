using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Item.Accessories
{
    internal class diamond_ring : Accesories
    {
        protected override void passive()
        {
            if(accessories_paasive_ == true)
            {
                Debug.Log("This ring is power overwhelming");
            }
        }
    }
}
