using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWeaponSwitcher : MonoBehaviour
{
    public CWeapon currentWeapon;
    public int currentIdx = 0;
    public List<CWeapon> myWeaponsList;

    public void LootWeapon(CWeapon weapon)
    {
        //weapon.gameObject.transform.SetParent(gameObject.transform);
        var weaponObj = Instantiate(weapon.gameObject, gameObject.transform);
        myWeaponsList.Add(weaponObj.GetComponent<CWeapon>());
        currentIdx = myWeaponsList.Count - 1;
        SwitchWeapon(currentIdx);
        Debug.Log("Looting Weapon : " + weaponObj.name);
    }

    public void SwitchWeapon(int idx)
    {
        for (int i = 0; i < myWeaponsList.Count; i++)
        {
            myWeaponsList[i].gameObject.SetActive(false);
        }

        currentWeapon = myWeaponsList[idx];
        currentWeapon.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        //var myWeaponsArr = GetComponentsInChildren<CWeapon>();
        var dm = DataManager.GetDataInstance();
        for (int i = 0; i < dm.saveWeapons.Count; i++)
        {
            Instantiate(dm.saveWeapons[i], transform);
        }
        myWeaponsList = new List<CWeapon>(GetComponentsInChildren<CWeapon>());
        SwitchWeapon(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0) // forward
        {
            if (!currentWeapon.is_Attacking)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
                {
                    if (currentIdx + 1 < myWeaponsList.Count)
                        currentIdx++;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0) // backward
                {
                    if (currentIdx > 0)
                        currentIdx--;
                }

                SwitchWeapon(currentIdx);
            }
        }
        
    }
}
