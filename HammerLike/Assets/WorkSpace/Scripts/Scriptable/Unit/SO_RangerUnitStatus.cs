using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ScriptableObject for Ranger Unit Status
[CreateAssetMenu(fileName = "RangerUnitStatus", menuName = "B_ScriptableObjects/Unit/RangerUnitStatus", order = 1)]
public class SO_RangerUnitStatus : SO_UnitStatus
{
    //[SerializeField] private GameObject projectilePrefab;
    // Start is called before the first frame update
    //public float chasingRange = 20f;
    // projectile speed 
    public float projectileSpeed = 3f;
}
