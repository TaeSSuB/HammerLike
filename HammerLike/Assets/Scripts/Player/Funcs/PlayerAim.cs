using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
	//레이 쏴서 하는 방식

	Vector3 mouseWorldPos;
	Vector3 playerToMouseDir;

	private void Awake()
	{
		
	}

	public Vector3 Aiming(float mouseSpd = 1f)
	{
		//Vector2 mousePos = Input.mousePosition;
		//Vector2 playerScreenPos = Camera.main.WorldToScreenPoint(transform.position);
		//Debug.Log("MousePos : " + mousePos + "\nplayer Pos : " + playerScreenPos); 
		//Vector2 dir = 


		Vector3 mouseScreenPos = Input.mousePosition;
		Ray mouseRay = Camera.main.ScreenPointToRay(mouseScreenPos); 
		RaycastHit hit;
		if (Physics.Raycast(mouseRay, out hit,100f, LayerMask.GetMask("Ground"))) 
		{
			Debug.Log("dd");
			mouseWorldPos= hit.point;
			mouseWorldPos.y = transform.position.y;

			playerToMouseDir = (mouseWorldPos - transform.position).normalized;
		}

		return playerToMouseDir;
	}


}
