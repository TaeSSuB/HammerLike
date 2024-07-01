using System.Collections;
using UnityEngine;

public class B_Rat : B_Enemy
{
    [Header("Rat")]
    [SerializeField] GameObject weaponColliderObj;

    public GameObject WeaponColliderObj => weaponColliderObj;
}