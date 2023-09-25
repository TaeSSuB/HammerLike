using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMeleeUnit : CUnit
{
    //Atk Range
    //Float
    //Attack()
    //Attack Function
    [Header("Melee")]
    public Vector2 atkDir;
    public GameObject atkVFX;
    public float spawnDelay = 1f;

    void Attack()
    {
        StartCoroutine(CoAttack(croom.finder, 1f / atkSpeed));
    }

    void TargetKnockBack(GameObject obj)
    {
        obj.GetComponent<Rigidbody2D>().AddForce((obj.transform.position - gameObject.transform.position) * 10, ForceMode2D.Impulse);
    }

    private void Start()
    {
        UnitInitialize();
        StartCoroutine(FirstSpawn(spawnDelay));
    }

     IEnumerator CoAttack(CPathFinder finder, float delay)
    {
        while (currentHp > 0)
        {
            //// 현재 노드 xy 
            //int x, y;
            //croom.gridMap.GetXY(gameObject.transform.position, out x, out y);

            float rnd = Random.Range(0f, 0.5f);

            yield return new WaitForSeconds(delay - rnd);

            finder.PathFinding(gameObject.transform.position, targetObj.transform.position);

            var nodeList = finder.finalNodeList;

            if (nodeList.Count > 0 && nodeList.Count <= atkRange && croom.isEnterPlayer)
            {
                state = State.Attack;

                if (state == State.Attack)
                {
                    anim.SetTrigger("TAttack");

                    yield return new WaitForSeconds(0.1f);

                    KnockBack(GetTargetDirVec2_Normal() * 10f);

                    yield return new WaitForSeconds(0.1f);

                    //targetObj.GetComponent<CUnitBase>().currentHp -= atkDmg;
                    if (currentHp > 0)
                    {
                        targetObj.GetComponent<CPlayer>().UnitDamaged(atkDmg);
                        TargetKnockBack(targetObj);
                        //targetObj.GetComponent<CUnitBase>().UnitDamaged(atkDmg);
                    }

                    //state = State.Idle;
                }
            }
        }
    }

    IEnumerator FirstSpawn(float delay)
    {
        while (true)
        {
            if (croom != null)
            {
                bool isPlayer = croom.isEnterPlayer;

                if (isPlayer)
                {
                    gameObject.SetActive(true);

                    yield return new WaitForSeconds(delay);
                    //StartCoroutine(MoveRepeat());
                    Move();
                    //InvokeRepeating("InvokeMove", 2f, 1f);
                    StartCoroutine(CoAttack(croom.finder, 1f / atkSpeed));

                    break;
                }
            }
            yield return new WaitForSeconds(0.5f);
            //yield return new WaitForSeconds(1f);
        }
        //else
        //    StartCoroutine(FirstSpawn(delay));

    }

}
