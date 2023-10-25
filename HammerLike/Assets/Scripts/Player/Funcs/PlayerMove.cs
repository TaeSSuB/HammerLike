using System.Collections;
using System.Collections.Generic;

using UnityEngine;

//이동, 회피 등

public class PlayerMove : MonoBehaviour
{

    Player player;

    public float distortionOffset;

    public Vector3 preMoveDir;

    private void Awake()
	{
        player = GetComponent<Player>();
    }

	public void Move(float moveSpd)
    {
        float horizon = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");


        if (horizon != 0f | vert != 0f)
        {
            player.animCtrl.SetLayerWeight(2, 1f);

            Vector3 dir = new Vector3(horizon, 0f, vert).normalized;
            dir.z *= distortionOffset;
            player.rd.velocity = dir * moveSpd;
        }
        else
        {
            player.animCtrl.SetLayerWeight(2, 0f);

            player.rd.velocity = Vector3.zero;
        }

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
