using System.Collections;
using System.Collections.Generic;

using UnityEngine;

//이동, 회피 등

public class PlayerMove : MonoBehaviour
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

	public void Move(float moveSpd)
    {
        float horizon = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");


        //preMoveDir = moveDir;
        //Debug.Log("pre : " + preMoveDir);
        //Debug.Log("Last : " + lastMoveDir);
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

            player.animCtrl.SetLayerWeight(1, 1f);
        }
        else
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
            player.animCtrl.SetLayerWeight(1, 0f);
        }


        /// 2023-10-30 [강명진] **conflict : 이동관련 스크립트에서 42번째 줄부터 74번째줄 , 76번째 줄 부터 89번째 줄 중 상충하는 문제가 있는거 같습니다
        /// 어떤게 맞는건지 잘 몰라서 일단 임의로 수정했는데 나중에 확인 부탁드려요 
        //231020 22:30 플레이어 물리 영향 받는거 있다해서 움직이는 코드 수정
        //transform.Translate(dir * moveSpd * Time.deltaTime);
    }

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
