using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUnit : CUnitBase
{
    [Header("Unit")]
    //public List<CNode> nodeList;
    //public GridManager gridManager;
    public GameObject spriteTRObj;
    public Animator anim;
    public Image hpBar;
    public CRoom croom;
    //public Vector3 targetPos;
    public GameObject targetObj;

    public Rigidbody2D rigid;

    public bool isDamaged;

    public void UnitInitialize()
    {
        SetHP();
        if (anim == null)
            GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        targetObj = GameObject.FindGameObjectWithTag("Player");
    }

    public void SetRoom(CRoom _croom)
    {
        croom = _croom;
    }

    public Vector2 GetTargetPosVec2()
    {
        return targetObj.transform.position;
    }

    public Vector2 GetTargetDirVec2_Normal()
    {
        return (targetObj.transform.position - transform.position).normalized;
    }

    public void Idle()
    {
        state = State.Idle;
    }

    public void Move()
    {
        StartCoroutine(CoMove(croom.finder, moveDelay));
        //StartCoroutine(CoMove(croom.finder));
    }

    public void Damaged(int damage)
    {
        if (!isDamaged)
        {
            if (currentHp - damage <= 0)
            {
                UnitDamaged(damage);
                //Dead();
            }
            else
            {
                StartCoroutine(CoDamaged(0.5f, damage));
            }
            Dead();
            //GetComponent<Collider2D>().enabled = false;

            //Dead();
        }
    }

    public void KnockBack(Vector2 force)
    {
        if (force.magnitude / rigid.mass > 30f)
        {
            force = force.normalized * 30f;
        }

        rigid.AddForce(force, ForceMode2D.Impulse);
    }

    public void Disabled()
    {
        gameObject.SetActive(false);
    }

    public void Dead()
    {
        hpBar.fillAmount = (float)currentHp / (float)maxHp;

        if (currentHp <= 0 && state != State.Dead)
        {
            state = State.Dead;
            Debug.Log(gameObject.name + " is Dead");

            anim.SetTrigger("TDead");

            Invoke("Disabled", 1f);
            
            //Destroy(gameObject, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D col2d)
    {
        //if (croom == null)
        if (col2d.CompareTag("Room"))
            croom = col2d.GetComponent<CRoom>();
            
    }

    private void OnCollisionEnter2D(Collision2D col2d)
    {
        Dead();
    }

    IEnumerator CoDamaged(float delay, int damage)
    {
        isDamaged = true;
        //currentHp -= damage;
        UnitDamaged(damage);
        anim.SetTrigger("TDamaged");
        //GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(delay);
        //GetComponent<Collider2D>().enabled = true;
        isDamaged = false;
    }

    IEnumerator CoMove(CPathFinder finder, float delay)
    {
        CNode currentNode = default;

        if (finder == null)
            finder = croom.finder;

        //Debug.Log(croom.finder);

        while (currentHp > 0)
        {
            state = State.Move;

            targetObj = GameObject.FindGameObjectWithTag("Player");

            finder = croom.finder;

            if (state == State.Move && !isDamaged)
            {
                rigid.velocity = Vector2.zero;

                var targetPos = targetObj.transform.position;

                if (state != State.Attack)
                {
                    if (targetPos.x < gameObject.transform.position.x)
                        spriteTRObj.transform.localScale = new Vector3Int(-1, 1, 1);
                    //spriteObj.GetComponent<SpriteRenderer>().flipX = true;
                    else
                        spriteTRObj.transform.localScale = new Vector3Int(1, 1, 1);
                    //spriteObj.GetComponent<SpriteRenderer>().flipX = false;
                }

                float timer = 0f;

                while (timer < delay && currentHp > 0)
                {
                    timer += Time.deltaTime;

                    finder.PathFinding(gameObject.transform.position, targetPos);

                    var nodeList = finder.finalNodeList;

                    if (nodeList.Count > atkRange) // »ç°Å¸® ¹Ù±ù
                    {
                        currentNode = nodeList[1];
                        //anim.SetTrigger("TMove");
                        anim.SetBool("BMove", true);

                        Vector2 movePos = croom.gridMap.GetWorldPosition(currentNode.x, currentNode.y) + new Vector2(0.5f, 0.5f);

                        rigid.MovePosition(Vector2.MoveTowards(gameObject.transform.position, movePos, Time.deltaTime * moveSpeed));

                        //if (myX == currentNode.x && myY == currentNode.y)
                        if ((Vector2)gameObject.transform.position == movePos)
                            break;
                        //if (timer > delay)
                        //    break;
                        if (isDamaged)
                            break;
                    }
                    else
                    {
                        anim.SetBool("BMove", false);
                        break;
                    }
                    //yield return new WaitForSeconds(0.01f);
                    yield return new WaitForFixedUpdate();
                }
            }

            float rnd = Random.Range(0f, 0.5f);

            //yield return new WaitForSeconds(rnd);
            //yield return new WaitForFixedUpdate();
            //yield return new WaitForSeconds(0.5f - rnd);
            yield return new WaitForSeconds(0.01f);

        }
    }

}
