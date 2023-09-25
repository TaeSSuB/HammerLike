using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CMeleeWeapon : CWeapon
{
    [Header("Melee Weapon")]
    public Animator anim;
    //public GameObject colObj;
    public GameObject playerObj;
    public GameObject vfxObj;
    public GameObject charge_vfxObj;

    public int charge = 1;
    public int maxcharge = 3;
    public bool isCharging;

    Coroutine chargeCoroutine;
    Rigidbody2D rigid;
    private void OnEnable()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponentInParent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        rigid.simulated = false;

        Physics2D.IgnoreCollision(playerObj.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
    }

    private void Update()
    {
        if (playerObj.GetComponent<CPlayer>().state != State.Dead)
        {
            if (Input.GetMouseButtonDown(0))
            {
                chargeCoroutine = StartCoroutine(CoCharge());
            }
            if (Input.GetMouseButtonUp(0))
            {
                //StopAllCoroutines();
                if (chargeCoroutine != null)
                    StopCoroutine(chargeCoroutine);
                StartCoroutine(CoAttack());
            }
        }
    }

    void Attack()
    {
        anim.SetTrigger("TAttack");
    }

    IEnumerator CoCharge()
    {
        if (!isCharging && !is_Attacking && !playerObj.GetComponent<CPlayer>().isInterAction)
        {
            //Debug.Log("Charge!!");
            isCharging = true;
            charge = 1;
            anim.SetBool("BCharge", true);

            while (isCharging && !is_Attacking)
            {
                yield return new WaitForSeconds(0.5f);
                if (charge < maxcharge)
                {
                    charge++;
                    charge_vfxObj.transform.localScale = new Vector2((float)charge / 2, (float)charge / 2);
                    Instantiate(charge_vfxObj, gameObject.transform);
                }
            }
        }
    }

    IEnumerator CoAttack()
    {
        if (!is_Attacking && isCharging && !playerObj.GetComponent<CPlayer>().isInterAction)
        {
            is_Attacking = true;
            isCharging = false;
            anim.SetBool("BAttack", true);
            anim.SetBool("BCharge", false);
            anim.SetTrigger("TAttack");

            //colObj.GetComponent<Collider2D>().

            //colObj.SetActive(true);
            rigid.simulated = true;
            yield return new WaitForSeconds(0.15f);

            //colObj.SetActive(false);
            rigid.simulated = false;
            is_Attacking = false;

            yield return new WaitForSeconds(0.35f);

            anim.SetBool("BAttack", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D col2d)
    {
        if (col2d.CompareTag("Monster"))
        {
            Debug.Log("Hit " + col2d.name);

            if (vfxObj != null)
                Instantiate(vfxObj, gameObject.transform.position, vfxObj.transform.rotation, null);

            var unit = col2d.GetComponent<CUnit>();
            unit.Damaged(atkData * charge);
            unit.KnockBack((col2d.transform.position - gameObject.transform.position) * knockbackData * charge);

            //col2d.GetComponent<Rigidbody2D>().AddForce((col2d.transform.position - gameObject.transform.position) * knockbackData * charge, ForceMode2D.Impulse);
        }

        if (col2d.CompareTag("Destroyable"))
        {
            Debug.Log("Hit " + col2d.name);
            //col2d.GetComponent<Rigidbody2D>().AddForce((col2d.transform.position - gameObject.transform.position) * knockbackData * charge, ForceMode2D.Impulse);
            col2d.GetComponent<Explodable>().explode();
        }

        if (col2d.CompareTag("Piece"))
        {
            col2d.GetComponent<Rigidbody2D>().AddForce((col2d.transform.position - gameObject.transform.position) * knockbackData * charge, ForceMode2D.Impulse);
        }
        
    }

    //private void OnCollisionEnter2D(Collision2D col2d)
    //{
    //    if (col2d.gameObject.CompareTag("Monster"))
    //    {
    //        Debug.Log("Hit " + col2d.gameObject.name);

    //        if (vfxObj != null)
    //            Instantiate(vfxObj, col2d.contacts[0].point, vfxObj.transform.rotation, null);

    //        col2d.gameObject.GetComponent<CUnit>().Hitted(atkData * charge);
    //        col2d.gameObject.GetComponent<Rigidbody2D>().AddForce((col2d.transform.position - gameObject.transform.position) * knockbackData * charge, ForceMode2D.Impulse);
    //    }
    //}

}
