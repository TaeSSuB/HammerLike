using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Johnson;

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
        Debug.Log("Last : " + lastMoveDir);
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

            //player.animCtrl.SetLayerWeight(2, 1f);
        }
        else
        {
            if (!isRest)
            {
                lastMoveDir = moveDir;
            }
            else
            {
                restTime += Time.deltaTime;
            }


            isRest = true;
            //player.animCtrl.SetLayerWeight(2, 0f);
            player.rd.velocity = Vector3.zero;
        }

        
//        Debug.Log("cur : " + moveDir);
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
