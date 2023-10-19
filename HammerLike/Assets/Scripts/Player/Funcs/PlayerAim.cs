using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Johnson;

public class PlayerAim : MonoBehaviour
{
	//레이 쏴서 하는 방식

	Vector3 mouseWorldPos;
	Vector3 playerToMouseDir;

	[SerializeField]
	PixelPerfectCamera zoomPixelCam;
	[SerializeField]
	Camera zoomCam;

	Ray mouseRay;
	Vector3 rayResultPoint;


#if UNITY_EDITOR
	Transform testCube;
#endif

	//[SerializeField]
	//Transform renderPlane;
	//public RenderTexture renderTex;

	private void Awake()
	{
#if UNITY_EDITOR
		testCube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
		testCube.transform.localScale *= 4f;
#endif
	}

	public Vector3 Aiming(float mouseSpd = 1f)
	{

		//해상도 바뀌면 렌더 텍스쳐 사이즈 바꾸기
		//231020 0146 
		//해상도 16:9 비율 고정 한다고 함.
		//만약 해상도 비율이 바뀐다면,
		//1. 렌더텍스쳐 해상도 해당 비율의 최소값으로 바꿔주고
		//2. 렌더 텍스쳐 쓰는 Plane Scale 비율도 바꿔주고
		//3. 그거에 맞춰서 zoom Cam의 Max Height 바꿔주셈
		//직교투영 카메라 size가 10이면 Unity Scale 기준 세로 반지름 10만큼 렌더링함.
		
		//=> 즉 해상도를 4:3으로 바꾼다면 최소 해상도를 800*600으로 잡고
		//1. 렌더 텍스쳐 해상도 800*600 으로 교체
		//2. plane의 Scale 4,1,3 으로 교체
		//3. zoomCam의 MaxHeight 30으로 (Plane Scale 1 =  Unity Scale 10) 교체
		//하면 작동 됨.

		//나중가서는 GameManager 같은 곳에서 16:9 해상도 고정해주고
		//해상도 바뀔때만 처리해주면 좋을듯
		
		
		float curArea = Screen.width * Screen.height;
		float areaScaleRatio = curArea / (Defines.minHeight * Defines.minWidth);
		//너비의 비율이니까 다시 제곱 해줘야함. 걍 한쪽 변 길이 나누기 해도 되고 ㅋㅋ
		areaScaleRatio = Mathf.Sqrt(areaScaleRatio);
		Debug.Log("areaScaleRatio : " + areaScaleRatio);

		Vector3 mouseScreenPos = Input.mousePosition;

		Vector2 mouseViewPos_main = Camera.main.ScreenToViewportPoint(mouseScreenPos);
		Vector2 mouseViewPos_zoom = zoomCam.ScreenToViewportPoint(mouseScreenPos);

		float zoomRatio = zoomCam.orthographicSize / zoomPixelCam.maxCameraHalfHeight;
		//Debug.Log("Ratio : " + ratio);

		//0.5빼서 중점 맞춰주고 
		//비율 곱해주고
		//다시 0.5 더해서 뷰포트 좌표에 맞춰주기?
		Vector2 result = new Vector2(mouseViewPos_main.x - 0.5f, mouseViewPos_main.y - 0.5f);
		result *= zoomRatio;
		result += new Vector2(0.5f, 0.5f);
		result /= areaScaleRatio;
		//Debug.Log("main ViewPort : " + mouseViewPos_main);
		//Debug.Log("Result viewPort : " + result);


		mouseRay = Camera.main.ViewportPointToRay(result);
		RaycastHit hit;
		if (Physics.Raycast(mouseRay, out hit, 500f, LayerMask.GetMask("Ground")))
		{

			Debug.Log("dd");
			mouseWorldPos = hit.point;
			//mouseWorldPos.y = transform.position.y;

			//231016 0816 지금 이거 정확한 위치가 아님.
			//아마 실질적인 작동 카메라랑 최종적 렌더링 카메라에서 나오는 단차인듯 함.
			//둘 다 스크린 좌표로 바꿔서 계산해보기?
			//231017 1413
			//둘 다 스크린 좌표로 바꾸더라도 차이가 날것임. 결국에는 최종적 스크린 좌표로 나오는 카메라 자체가 다르니까.
			//그렇다면 마우스 좌표를 보정해줘야 할 거 같은디

			//스크린 좌표를 사용하는거 보다
			//Zoom카메라의 줌 값에 따른 뷰포트의 차이를 보정해주고 그걸로 
			//다시 레이를 쏴서 체크하는 방식이 어쩌면 더 정확할거 같음.

#if UNITY_EDITOR
			testCube.position = mouseWorldPos;
#endif
			rayResultPoint = mouseWorldPos;

			playerToMouseDir = (mouseWorldPos - transform.position).normalized;

			
		}

		return playerToMouseDir;
		
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Camera.main.transform.position, rayResultPoint);
	}
}


