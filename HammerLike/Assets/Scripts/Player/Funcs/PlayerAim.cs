using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
	//레이 쏴서 하는 방식

	Vector3 mouseWorldPos;
	Vector3 playerToMouseDir;

	Transform testCube;

	private void Awake()
	{
		testCube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
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
		if (Physics.Raycast(mouseRay, out hit,500f, LayerMask.GetMask("Ground"))) 
		{
			
			Debug.Log("dd");
			mouseWorldPos= hit.point;
			//mouseWorldPos.y = transform.position.y;

			//231016 0816 지금 이거 정확한 위치가 아님.
			//아마 실질적인 작동 카메라랑 최종적 렌더링 카메라에서 나오는 단차인듯 함.
			//다 스크린 좌표로 바꿔서 해보자
			testCube.position = mouseWorldPos;

			
			playerToMouseDir = (mouseWorldPos - transform.position).normalized;
		}

		return playerToMouseDir;
	}


}
