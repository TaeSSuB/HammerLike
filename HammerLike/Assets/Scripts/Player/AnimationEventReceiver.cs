using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public GameObject weaponObject; // `TrailRenderer`가 있는 무기 오브젝트

    public void EnableWeaponTrail()
    {
        //weaponObject.GetComponent<WeaponTrail>().EnableTrail();
    }

    public void DisableWeaponTrail()
    {
        //weaponObject.GetComponent<WeaponTrail>().DisableTrail();
    }
}
