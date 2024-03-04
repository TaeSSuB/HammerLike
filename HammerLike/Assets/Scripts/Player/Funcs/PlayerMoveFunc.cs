using System.Collections;
using System.Collections.Generic;

using UnityEngine;

//이동, 회피 등

public class PlayerMoveFunc : MonoBehaviour
{

    Player player;

    public float distortionOffset;


    public bool isRest;
    public float restTime;

    //public Vector3 preMoveDir;
    public Vector3 moveDir = Vector3.forward;
    public Vector3 lastMoveDir = Vector3.forward;

    public Vector3 preMoveDir;


    private void Awake()
	{
        player = GetComponent<Player>();
    }

    public bool Move(float moveSpd, Rewired.Player player)
    {
        float horizon = player.GetAxis("Horizontal");
        float vert = player.GetAxis("Vertical");

        Vector3 dir = Vector3.forward;

        if (horizon != 0f || vert != 0f) // '||' 연산자 사용
        {
            if (isRest)
            {
                restTime = 0f;
            }

            isRest = false;

            dir = new Vector3(horizon, 0f, vert).normalized;
            moveDir = dir;
            lastMoveDir = moveDir;

            dir.z *= distortionOffset;
            this.player.rd.velocity = dir * moveSpd; // 'this.player'가 올바른지 확인 필요

            return true;
        }
        else if (!isRest)
        {
            // 키보드 입력이 떼어졌을 때의 처리
            isRest = true;
            restTime = 0f; // 미끄러짐 지속 시간
        }

        if (isRest)
        {
            if (restTime > 0)
            {
                // 미끄러짐 효과
                this.player.rd.velocity = lastMoveDir * moveSpd; // 'this.player'가 올바른지 확인 필요
                restTime -= Time.deltaTime;
            }
            else
            {
                // 미끄러짐 후 완전히 멈춤
                this.player.rd.velocity = Vector3.zero;
            }
        }

        return false;
        transform.Translate(dir * moveSpd * Time.deltaTime);
    }

    /*public bool Move(float moveSpd)
    {
        float horizon = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 dir = Vector3.forward;

        if (horizon != 0f | vert != 0f)
        {
            if (isRest)
            {
                restTime = 0f;
            }

            isRest = false;

            dir = new Vector3(horizon, 0f, vert).normalized;
            moveDir = dir;
            lastMoveDir = moveDir;

            dir.z *= distortionOffset;
            player.rd.velocity = dir * moveSpd;

            return true;
        }
        *//*else
        {
            if (!isRest)
            {
                lastMoveDir = moveDir;
                //player.preMoveDir = Johnson.eGizmoDir.End;
            }
            else
            {
                restTime += Time.deltaTime;
            }


            isRest = true;

            return false;
        }*/
    /*else
    {
        // 키보드 입력이 없을 때 이동 중단
        player.rd.velocity = Vector3.zero;
        isRest = true;
        return false;
    }*//*
    else if (!isRest)
    {
        // 키보드 입력이 떼어졌을 때의 처리
        isRest = true;
        restTime = 0.3f; // 미끄러짐 지속 시간
    }

    if (isRest)
    {
        if (restTime > 0)
        {
            // 미끄러짐 효과
            player.rd.velocity = lastMoveDir * moveSpd;
            restTime -= Time.deltaTime;
        }
        else
        {
            // 미끄러짐 후 완전히 멈춤
            player.rd.velocity = Vector3.zero;
        }
    }

    return false;


    /// 2023-10-30 [강명진] **conflict : 이동관련 스크립트에서 42번째 줄부터 74번째줄 , 76번째 줄 부터 89번째 줄 중 상충하는 문제가 있는거 같습니다
    /// 어떤게 맞는건지 잘 몰라서 일단 임의로 수정했는데 나중에 확인 부탁드려요 
    //231020 22:30 플레이어 물리 영향 받는거 있다해서 움직이는 코드 수정
    transform.Translate(dir * moveSpd * Time.deltaTime);
}*/

    public void Run(float runSpd)
    { 
        
    
    }

    public void Envasion(EnvasionStat stat)
    {
		if (Input.GetKeyUp(KeyCode.Space))
		{
            player.fsm.SetNextState("Player_Envasion");
		}
	}


	private void OnDestroy()
	{
        player = null;
	}
}
