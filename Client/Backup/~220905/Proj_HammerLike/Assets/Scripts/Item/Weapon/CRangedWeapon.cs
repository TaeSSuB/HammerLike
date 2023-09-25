using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRangedWeapon : CWeapon
{
    [Header("Ranged Weapon")]
    public int bulletMax;
    public int bulletCurrent;
    public int shotNum = 1;
    public float shotDelay = 0.5f;
    public float shoteachDelay = 0.1f;
    public Transform shotTr;
    public GameObject bulletPrefab;
    public bool isReloading = false;

    public bool isMouseDown = false;

    private void Start()
    {
        bulletCurrent = bulletMax;
    }

    public void OnEnable()
    {
        Debug.Log("enabled");
        isReloading = false;
        StartCoroutine(InfiniteCoShot(bulletPrefab, shotNum, shotDelay, shoteachDelay));
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            isMouseDown = true;
        }
        else
        {
            isMouseDown = false;
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    is_Attacking = true;
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    is_MouseDown = true;
        //    StartCoroutine(CoShot(bulletPrefab, 3, 0.5f));
        //    //Shot(bulletPrefab);
        //}
        
    }

    IEnumerator Reload(float delay)
    {
        isReloading = true;

        yield return new WaitForSeconds(delay);

        bulletCurrent = bulletMax;

        isReloading = false;
    }

    void Shot(GameObject _bulletPrefab)
    {
        if (bulletCurrent > 0)
        {
            _bulletPrefab.GetComponent<CWeapon>().atkData = atkData;

            Instantiate(_bulletPrefab, shotTr.position, shotTr.rotation, null);

            bulletCurrent--;
        }
    }

    IEnumerator CoShot(GameObject _bulletPrefab, int oneTime, float delay, float eachdelay = 0.1f)
    {
        if (!is_Attacking)
        {
            is_Attacking = true;
            for (int i = 0; i < oneTime; i++)
            {
                Shot(_bulletPrefab);

                yield return new WaitForSeconds(eachdelay);
            }

            yield return new WaitForSeconds(delay);

            is_Attacking = false;
        }
    }

    IEnumerator InfiniteCoShot(GameObject _bulletPrefab, int oneTime, float delay, float eachdelay = 0.1f)
    {
        while (true)
        {
            if (isMouseDown)
            {
                StartCoroutine(CoShot(_bulletPrefab, oneTime, delay, eachdelay));
                if (bulletCurrent <= 0 && !isReloading)
                {
                    StartCoroutine(Reload(3f));
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
