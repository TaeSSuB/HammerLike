using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public GameObject weaponObject; // `TrailRenderer`�� �ִ� ���� ������Ʈ

    public void EnableWeaponTrail()
    {
        //weaponObject.GetComponent<WeaponTrail>().EnableTrail();
    }

    public void DisableWeaponTrail()
    {
        //weaponObject.GetComponent<WeaponTrail>().DisableTrail();
    }
}
