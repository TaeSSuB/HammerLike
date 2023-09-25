using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBullet : CWeapon
{
    [Header("Bullet")]
    public float bulletSpeed = 3f;
    public GameObject vfxObj;
    public string targetTag;
    
    void Update()
    {
        transform.Translate(bulletSpeed * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D col2d)
    {

        //if (col2d.gameObject.layer == LayerMask.NameToLayer("Wall") ||
        //    col2d.gameObject.CompareTag("Golem"))
        //{
        //    if (col2d.gameObject.CompareTag("Golem"))
        //    {
        //        col2d.gameObject.GetComponent<CUnit>().hp -= atkData;
        //    }

        //    if (vfxObj != null)
        //        Instantiate(vfxObj, col2d.contacts[0].point, vfxObj.transform.rotation, null);

        //    Destroy(gameObject);
        //}

        if (col2d.CompareTag("Wall") ||
            col2d.CompareTag(targetTag))
        {
            if (col2d.CompareTag(targetTag))
            {
                if (targetTag == "Player")
                {
                    var unitbase = col2d.GetComponent<CPlayer>();
                    if (!unitbase.isDodge)
                    {
                        unitbase.UnitDamaged(atkData);
                        if (vfxObj != null)
                            Instantiate(vfxObj, gameObject.transform.position, vfxObj.transform.rotation, null);
                        Destroy(gameObject);
                    }
                }
                else
                {
                    var unitbase = col2d.GetComponent<CUnitBase>();
                    if (!unitbase.isDodge)
                    {
                        unitbase.UnitDamaged(atkData);
                        if (vfxObj != null)
                            Instantiate(vfxObj, gameObject.transform.position, vfxObj.transform.rotation, null);
                        Destroy(gameObject);
                    }
                }
            }

            if (col2d.CompareTag("Wall"))
            {
                if (vfxObj != null)
                    Instantiate(vfxObj, gameObject.transform.position, vfxObj.transform.rotation, null);
                Destroy(gameObject);
            }
        }
        
    }
}
