using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPBotelho
{
    [System.Serializable]
    public class RockSettings
    {
        public int verts = 250;

        public RandomType randomType = RandomType.Vector3;

        public Vector3 size = new Vector3 (1, 1, 1);

        public float radius = 2;

    }
}