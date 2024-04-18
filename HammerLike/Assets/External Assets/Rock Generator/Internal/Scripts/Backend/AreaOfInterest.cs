using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPBotelho
{
    [System.Serializable]
    public class AreaOfInterest : MonoBehaviour
    {  
        public int scale = 1;    

        void OnDrawGizmos ()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(scale, scale, scale));
        }

    }
}
