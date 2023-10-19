using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
	//레이 쏴서 하는 방식

	Vector3 mouseWorldPos;
	Vector3 playerToMouseDir;



	Transform testCube;

	[SerializeField]
	PixelPerfectCamera zoomPixelCam;
	[SerializeField]
	Camera zoomCam;

	Ray mouseRay;

	Vector3 rayResultPoint;

	private void Awake()
	{
		testCube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
		testCube.transform.localScale *= 4f;
	}

	//public Vector3 Aiming(float mouseSpd = 1f)
	//{
	//    //Vector2 mousePos = Input.mousePosition;
	//    //Vector2 playerScreenPos = Camera.main.WorldToScreenPoint(transform.position);
	//    //Debug.Log("MousePos : " + mousePos + "\nplayer Pos : " + playerScreenPos); 
	//    //Vector2 dir = 



	//    Vector3 mouseScreenPos = Input.mousePosition;


	//    Vector2 mouseViewPos_main = Camera.main.ScreenToViewportPoint(mouseScreenPos);
	//    Vector2 mouseViewPos_zoom = zoomCam.ScreenToViewportPoint(mouseScreenPos);
	//    float zoomCamSize = zoomCam.orthographicSize;
	//    float ratio = zoomCamSize / zoomPixelCam.maxCameraHalfHeight;

	//    Debug.Log("main View : " + mouseViewPos_main + "\nZoom View : " + mouseViewPos_zoom);
	//    //당연히 다른 값이 나옴...? 
	//    //23.10.17 1602 왜 또 같노?
	//    //이거 보니까 화면 해상도에 따라서 다름.


	//    Ray mouseRay = Camera.main.ScreenPointToRay(mouseScreenPos);
	//    //줌 카메라로 바꾼다면 레이 처리 자체가 안될꺼임.


	//    RaycastHit hit;
	//    if (Physics.Raycast(mouseRay, out hit, 500f, LayerMask.GetMask("Ground")))
	//    {

	//        Debug.Log("dd");
	//        mouseWorldPos = hit.point;
	//        //mouseWorldPos.y = transform.position.y;

	//        //231016 0816 지금 이거 정확한 위치가 아님.
	//        //아마 실질적인 작동 카메라랑 최종적 렌더링 카메라에서 나오는 단차인듯 함.
	//        //둘 다 스크린 좌표로 바꿔서 계산해보기?
	//        //231017 1413
	//        //둘 다 스크린 좌표로 바꾸더라도 차이가 날것임. 결국에는 최종적 스크린 좌표로 나오는 카메라 자체가 다르니까.
	//        //그렇다면 마우스 좌표를 보정해줘야 할 거 같은디

	//        //스크린 좌표를 사용하는거 보다
	//        //Zoom카메라의 줌 값에 따른 뷰포트의 차이를 보정해주고 그걸로 
	//        //다시 레이를 쏴서 체크하는 방식이 어쩌면 더 정확할거 같음.
	//        testCube.position = mouseWorldPos;


	//        playerToMouseDir = (mouseWorldPos - transform.position).normalized;
	//    }

	//    return playerToMouseDir;
	//}


	public Vector3 Aiming(float mouseSpd = 1f)
	{
		Vector3 mouseScreenPos = Input.mousePosition;

		Vector2 mouseViewPos_main = Camera.main.ScreenToViewportPoint(mouseScreenPos);
		Vector2 mouseViewPos_zoom = zoomCam.ScreenToViewportPoint(mouseScreenPos);
		Debug.Log("main ViewPort : " + mouseViewPos_main + "\nZoom ViewPort : " + mouseViewPos_zoom);


		//float zoomCamSize = zoomCam.orthographicSize;
		float ratio = zoomCam.orthographicSize / zoomPixelCam.maxCameraHalfHeight;
		Debug.Log("Ratio : " + ratio);
		//Vector2 offsetViewport = new Vector2(mouseViewPos_main.x - 0.5f, mouseViewPos_main.y - 0.5f);
		//offsetViewport *= ratio;
		//offsetViewport += new Vector2(0.5f, 0.5f);

		//Zoom 카메라의 사이즈 비율에 따라서 Main에서 어디 위치로 찍어야 하는지
		//이건 일단 해상도와 
		//Vector2 offsetViewport = new Vector2(, );


		//Debug.Log("Ratio : " + ratio + "\noffset ViewPort : " + offsetViewport);

		//float temp  = Test()


		//0.5빼서 중점 맞춰주고 
		//비율 곱해주고
		//다시 0.5 더해서 뷰포트 좌표에 맞춰주기?
		Vector2 result = new Vector2(mouseViewPos_main.x - 0.5f, mouseViewPos_main.y - 0.5f);
		result *= ratio;
		result += new Vector2(0.5f, 0.5f);
		//231017 시발 잘되노;

		//이제 할건 해상도에 따라서 차이나는 부분을 보정해주는거
		//픽셀 퍼펙트 카메라 오얼 썸띵의 기능


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
			testCube.position = mouseWorldPos;
			
			rayResultPoint = mouseWorldPos;

			playerToMouseDir = (mouseWorldPos - transform.position).normalized;

			
		}

		return playerToMouseDir;
		//return Vector3.zero;
	}


	public float Test(float val, float ratio)
	{
		//float offset = ((ratio - 1f) * 0.5f) + 1;
		//Vector2 clamp = new Vector2(0.5f - (1))

		return val < 0.5f ? 0.5f -  val * ratio : 0.5f + val * ratio;
	}

	public Vector2 Test(Vector2 val, float ratio)
	{
		//float offset = ((ratio - 1f) * 0.5f) + 1;
		Vector2 temp = new Vector2(Test(val.x, ratio), Test(val.y, ratio));
		return temp;

		//Vector2 result = val;

		//if (val.x > 0.5f && val.y > 0.5)
		//{//1사분면
		//	result.x = val.x * ratio;
		//	result.y = val.y * ratio;
		//}
		//else if (val.x < 0.5f && val.y > 0.5f)
		//{//2사분면
		//	result.x = val.x * ratio;
		//	result.y = val.y * ratio;
		//}
		//else if (val.x < 0.5f && val.y < 0.5f)
		//{//3사분면
		//	result.x = val.x * ratio;
		//	result.y = val.y * ratio;
		//}
		//else if (val.x > 0.5f && val.y < 0.5f)
		//{ //4사분면
		//	result.x = val.x * ratio;
		//	result.y = val.y * ratio;
		//}

		//return result;
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Camera.main.transform.position, rayResultPoint);
	}
}


