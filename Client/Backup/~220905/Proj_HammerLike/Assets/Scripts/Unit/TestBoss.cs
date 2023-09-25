using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoss : CUnit
{
    public CUnit mobCUnit;
    public Transform mobTransforms;
    public float dashForce = 10f;
    public bool isPattern = false;
    // targetObj = player position

    void SetFace()
    {
        var dir = GetTargetDirVec2_Normal();

        if (dir.x < 0)
            spriteTRObj.transform.localScale = new Vector3Int(-1, 1, 1);
        else
            spriteTRObj.transform.localScale = new Vector3Int(1, 1, 1);
    }

    void Start()
    {
        //SetHP();
        UnitInitialize();

        StartCoroutine(CoBossPatternAll());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D col2d)
    {
        if(col2d.gameObject.CompareTag("Player"))
        {
            targetObj.GetComponent<CPlayer>().UnitDamaged(atkDmg);
        }
    }

    IEnumerator CoBossPatternAll()
    {
        while (true)
        {
            if (!isPattern)
            {
                int rnd = Random.Range(0, 4);
                
                switch(rnd)
                {
                    case 0:
                        StartCoroutine(CoBossPattern_Spawn());
                        break;
                    case 1:
                        StartCoroutine(CoBossPattern_Rolling());
                        break;
                    case 2:
                        StartCoroutine(CoBossPattern_Jump());
                        break;
                    case 3:
                        StartCoroutine(CoBossMove());
                        break;
                    default:
                        Debug.Log("Molu");
                        break;
                }
            }
            else
            {
                Debug.Log("Idle");
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator CoBossPattern_Spawn()
    {
        isPattern = true;
        yield return new WaitForSeconds(3f);

        foreach (Transform pos in mobTransforms.GetComponentsInChildren<Transform>())
        {
            //croom.SpawnMob(mobCUnit, pos.position);
            mobCUnit.croom = croom;
            var slimeObj = Instantiate(mobCUnit.gameObject, pos.position, mobCUnit.transform.rotation);
            //slimeObj.GetComponent<CUnit>().croom = croom;
        }

        yield return new WaitForSeconds(1f);
        isPattern = false;
    }

    IEnumerator CoBossPattern_Rolling()
    {
        isPattern = true;
        yield return new WaitForSeconds(2f);

        var dir = GetTargetDirVec2_Normal();
        //var targetPos = GetTargetPosVec2();
        SetFace();
        yield return new WaitForSeconds(0.5f);

        anim.SetTrigger("TRolling");

        //rigid.AddForce(dir * dashForce * rigid.mass, ForceMode2D.Impulse);
        KnockBack(dir * dashForce * rigid.mass);

        //Instantiate(dashVFX, gameObject.transform.position, dashVFX.transform.rotation);
        yield return new WaitForSeconds(2f);

        rigid.velocity = Vector2.zero;
        isPattern = false;
    }

    IEnumerator CoBossPattern_Jump()
    {
        isPattern = true;
        yield return new WaitForSeconds(1f);

        rigid.simulated = false;
        anim.SetTrigger("TJump");
        yield return new WaitForSeconds(5f);

        SetFace();
        transform.position = GetTargetPosVec2();
        yield return new WaitForSeconds(1f);

        anim.SetTrigger("TLanding");
        yield return new WaitForSeconds(0.15f);
        rigid.simulated = true;
        isPattern = false;
    }

    IEnumerator CoBossMove()
    {
        isPattern = true;
        SetFace();
        yield return new WaitForSeconds(1f);

        isPattern = false;
    }


}
