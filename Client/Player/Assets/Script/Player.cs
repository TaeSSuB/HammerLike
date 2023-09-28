using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerStat
{
	public float maxHp;
	public float curHp;

	public float maxStamina;
	public float curStamina;

	public float moveSpd;

	public float upperRotSpd;
	public float lowerRotSpd;
}


public class Player : MonoBehaviour
{

	public PlayerStat stat;



	[Space(10f)]
	public PlayerMove moveScript;
	public PlayerAttack atkScript;


	private void Awake()
	{
		if (!moveScript)
		{ moveScript = GetComponent<PlayerMove>(); }
	}

	void Start()
    {
        
    }

    void Update()
    {
		moveScript.Move();
    }


	private void OnEnable()
	{
		
	}

	private void OnDisable()
	{
		

	}

	
}
