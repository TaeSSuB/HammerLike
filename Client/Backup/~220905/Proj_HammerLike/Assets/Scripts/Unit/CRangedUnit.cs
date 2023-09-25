using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRangedUnit : CUnit
{
    //Atk Range
    //Float
    //Bullet Obj
    //GameObject(Prefab)
    //Bullet Speed
    //Float
    //Shot()
    //Shot Function
    //Reload()
    //Reload Function

    [Header("Ranged")]
    public float bulletSpeed;
    public int bulletMax;
    public GameObject bulletObj; // atkVFX binding at Bullet Prefab
    public GameObject centerObj;
    public GameObject shotPos;

    public float spawnDelay = 1f;

    void Aiming()
    {
        Vector3 targetPos = targetObj.transform.position;

        Vector2 shotDir = targetPos - centerObj.transform.position;

        // tan = y/x
        float rotZ = Mathf.Atan2(shotDir.y, shotDir.x) * Mathf.Rad2Deg;

        centerObj.transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    void Shot()
    {
        if (currentHp > 0)
        {
            Aiming();
            //targetObj
            Instantiate(bulletObj, shotPos.transform.position, centerObj.transform.rotation);
            //targetObj
        }
    }

    void Reload()
    {

    }

    private void Start()
    {
        //SetHP();
        UnitInitialize();
        StartCoroutine(FirstSpawn(spawnDelay));

        //Move();
    }

    IEnumerator CoShot(CPathFinder finder, float delay)
    {
        while (currentHp > 0)
        {
            float rnd = Random.Range(0f, 0.5f);

            yield return new WaitForSeconds(delay - rnd);
            //// 현재 노드 xy 
            //int x, y;
            //croom.gridMap.GetXY(gameObject.transform.position, out x, out y);
            finder.PathFinding(gameObject.transform.position, targetObj.transform.position);

            var nodeList = finder.finalNodeList;

            if (nodeList.Count > 0 && nodeList.Count <= atkRange && croom.isEnterPlayer)
            {
                state = State.Attack;

                if (state == State.Attack)
                {
                    anim.SetTrigger("TAttack");

                    yield return new WaitForSeconds(0.2f);

                    Shot();
                    //targetObj.GetComponent<CUnitBase>().currentHp -= atkDmg;
                    //targetObj.GetComponent<CUnitBase>().UnitDamaged(atkDmg);

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
                    StartCoroutine(CoShot(croom.finder, 1f / atkSpeed));

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
