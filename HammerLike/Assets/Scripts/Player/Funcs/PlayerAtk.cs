using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAtk : MonoBehaviour
{

	public Player player;
	public float curCharging;

	//Move처럼 아예 그냥 Unity 에디터 설정 내에서 Attack용 키설정을 해주신담에
	//받아오시면 패드까지 설정이 편합니당

	//Attack은 그냥 상체 애니메이션만 재생해주면 되니까
	//이거 그냥 함수로 만들어서 실행시켜주시는것도 괜춘할거 같아서 
	//따로 Attack State를 만들지는 않았읍니다.

	private void Awake()
	{
		player = GetComponent<Player>();

	}

	public void Charging()
	{
		

	}

	public void Attack()
	{

		if (Input.GetMouseButton(1))
		{
			curCharging += Time.deltaTime;
			Debug.Log("우측키 누름");
		}
		if (Input.GetMouseButtonUp(1))
		{
			player.animCtrl.SetTrigger("tAtk");
			player.animCtrl.SetFloat("fAtkVal", curCharging);
            Debug.Log("우측키 땜");
        }

		//만약 공격 애니메이션 재생 중이거나 공격속도? 연사력 같은 스텟 있으시면
		//그거일때는 공격 몬하도록 해주심 될듯


	}

}
