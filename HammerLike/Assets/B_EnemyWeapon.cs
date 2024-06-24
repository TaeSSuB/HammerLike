using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_EnemyWeapon : MonoBehaviour
{
    [SerializeField] private bool isParried = false;

    public bool IsParried { get => isParried; }

    public void Init()
    {
        gameObject.layer = LayerMask.NameToLayer("MonsterWeapon");
        isParried = false;    
    }

    public void SetParried(bool value)
    {
        isParried = value;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon") && other.CompareTag("WeaponCollider"))
        {
            isParried = true;
            gameObject.layer = LayerMask.NameToLayer("PlayerWeapon");
        }
    }

    // protected void OnTriggerExit(Collider other)
    // {
    //     if(other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon") && other.CompareTag("WeaponCollider"))
    //     {
    //         isParried = false;
    //         gameObject.layer = LayerMask.NameToLayer("MonsterWeapon");
    //     }
    // }


}
