using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Johnson;

public enum eBoundary
{
    LT,
    RT,
    RB,
    LB,
    Center,
    End
}

//231025 1152 카메라 다른 연출이 있어서
//더 작업 안해도 될 듯.

public class CamCtrl : MonoBehaviour
{

    /// <summary>
    /// 231025 1152
    /// 테스트용 스크립트임!!!!
    /// 나중에 카메라 재 작업할때는 편하게 지우거나 수정해서 쓰십셔
    /// </summary>


    //1. 스테이지 바깥을 넘어가지 않을 것.
    //=> 카메라 모서리 4부분 계산해서 안나가도록?
    //바닥 쏴서 충돌 안하면 더 이동 안하도록? 
    //근데 이거 맵보다 커지면 곤란 + 맵 바닥이 겹치면 무한정으로 이동함.
    //=> 이건 현재 입장된 맵만 활성화하는 방법으루다가 하면 될듯

    //아니면
    //2.현재 맵의 사이즈를 받아와서 MainCam의 Zoom값으로 렌더링 사이즈를 계산해서 체크하기?
    //기울기가 있어서 그러기 쉽지 않음. 그냥 1번 ㄱㄱ


    //순서
    //1. 스테이지 시작 혹은 초기화 시점에 플레이어를 중앙에 두기

    [Header("Cams")]
    public Camera mainCam;  //For RenderTex = mainCam
    public Transform followObjTr;
    public float followSpd;
    public float followDistOffset;

    [Space(10f)]
    [Header("Zoom")]
    public PixelPerfectCamera zoomCam; //Only control zoom using Height vari, For Screen Render
    public float zoomMin;
    public float zoomSpd;
    

    [Space(10f)]
    //public Vector3[] boundaryDir; //카메라의 모서리+중앙 Direction 
    public float distToGround;

    //MainCam의 바닥에 닿는 4군대 모서리
    public Vector3[] boundaryPosToRay = new Vector3[(int)eBoundary.End];
    //public Vector3[] boundaryDirToRay;
    //public Vector3[] boundaryPosToView;

    public void CameraCallibration(Vector3 centerPos)
    {
        //씬 초기화, 스테이지 초기화등
        //특정 오브젝트를 카메라 중앙에 두어야 할 때. (보통 플레이어 일 듯)
        //호출 한번씩 해주쇼잉

        //1. centerPos의 y값 (오브젝트 위치)에 맞춰 Ray 쏴서 차이벡터 구함

        Vector3 dir;

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        var rayResult = new RaycastHit();

        if (Physics.Raycast(ray, out rayResult, 100f, LayerMask.GetMask("Ground")))
        {
            distToGround = Vector3.Distance(mainCam.transform.position, rayResult.point);
            
            dir = mainCam.transform.position - rayResult.point;

            mainCam.transform.position = centerPos + dir;
        }
        else
        { 
            //만약 바닥 체크 안되면 그냥 임시로 하나 만들고 체크하고 나서 없애셈 ㅋ;
            
        
        }
    }

    void ComputeCamBoundaryRay(Vector2 viewPortPos, ref Vector3 renderBoundary/*, ref Vector3 boundaryDir*/)
    {
        Vector3 worldPos = mainCam.ViewportToWorldPoint(new Vector3(viewPortPos.x, viewPortPos.y, mainCam.nearClipPlane));
        Vector3 camDir = mainCam.transform.forward;

        var ray = new Ray(worldPos, camDir);
        var rayResult = new RaycastHit();

        if (Physics.Raycast(ray, out rayResult, 100f, LayerMask.GetMask("Ground")))
        {
            renderBoundary = rayResult.point;
            //boundaryDir = mainCam.transform.position - rayResult.point;
        }
    }

    public void Following()
    {
		//Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
		//var rayResult = new RaycastHit();

		//Vector3 dir;
		//if (Physics.Raycast(ray, out rayResult, 100f, LayerMask.GetMask("Ground")))
		//{
		//    dir = followObjTr.position - rayResult.point;
		//    float dist = Vector3.Distance(followObjTr.position, rayResult.point);
		//    if (dist <= 0.05f)
		//    {
		//        return;
		//    }
		//    mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, mainCam.transform.position + dir, Time.unscaledDeltaTime * followSpd);
		//}

		float dist = Vector3.Distance(followObjTr.position, boundaryPosToRay[(int)eBoundary.Center]);
        if (dist <= followDistOffset)
        {
            return;
        }

		Vector3 dir = (followObjTr.position - boundaryPosToRay[(int)eBoundary.Center]).normalized;
        Vector3 prePos = mainCam.transform.position;
        Vector3 tempPos = mainCam.transform.position + dir;
        Vector3 goalPos = Vector3.Lerp(mainCam.transform.position, tempPos, Time.unscaledDeltaTime * followSpd);

        mainCam.transform.position = goalPos;

        for (int i = 0; i < (int)eBoundary.End; ++i)
        {
            Vector3 worldPos = mainCam.ViewportToWorldPoint(new Vector3(Defines.ViewportPos[i].x, Defines.ViewportPos[i].y, mainCam.nearClipPlane));
            Vector3 camDir = mainCam.transform.forward;

            var ray = new Ray(worldPos, camDir);
            var rayResult = new RaycastHit();

            if (Physics.Raycast(ray, out rayResult, 100f, LayerMask.GetMask("Ground")))
            {
                //boundaryPosToRay[i] = rayResult.point;
            }
            else
            {
                mainCam.transform.position = prePos;

                //return;
                //그냥 안움직이게 하는게 아니라, 해당 방향으로만 이동이 안되게 해야함.
                //근데 이게 되게 애매함.
                //1. 캐릭터의 '이동방향' 도 파악해야함.
                //2. 모서리 4군데 중 어디방향이 막힌건지 파악해야함.
                //3. 그리고 어디 방향으로 갈 수 있는지 파악해야함.
                
                //아니다
                //어디 방향이 막혔는지 파악해서?
                

                //가장 좋은건 맵 데이터를 받아와서 아예 카메라 위치 제한을 만드는거.
                //화이팅! 훗날의 명진씨!
                
            }
        }

    }

    public void Zoom()
    {
        float wheelVal = Input.GetAxis("Mouse ScrollWheel");

        if (wheelVal == 0f)
        {
            return;
        }

        //float preSize = zoomCam.targetCameraHalfHeight;

        float zoomVal = (-1f * wheelVal * zoomSpd) + zoomCam.targetCameraHalfHeight;

        zoomCam.targetCameraHalfHeight = Mathf.Clamp(zoomVal, zoomMin, zoomCam.maxCameraHalfHeight);

        zoomCam.adjustCameraFOV();
    }


 

    //public void Test()
    //{
    //    for (int i = 0; i < (int)eBoundary.End; ++i)
    //    {
    //        ComputeCamBoundaryRay(Defines.ViewportPos[i], ref boundaryPosToRay[i], ref boundaryDirToRay[i]);
    //        boundaryPosToView[i] = mainCam.ViewportToWorldPoint(new Vector3(Defines.ViewportPos[i].x, Defines.ViewportPos[i].y, distToGround));
    //    }
    //}


	private void Awake()
	{
        if (!mainCam)
        {
            mainCam = Camera.main;
        }


        if (!zoomCam)
        { 
            for (int i = 0; i < transform.childCount; ++i)
            {
                GameObject child = transform.GetChild(i).gameObject;

                if (child.name.Equals("Zoom Camera"))
                {
                    //어차피 씬 초기화 단계에서 찾는거기에 크게 신경 안쓰는 편.
                    //string형으로 오브젝트 찾는거 도저히 못참을 경우 없애도 무방.
                    zoomCam = child.GetComponent<PixelPerfectCamera>();
                }

            }
        }

        if (!followObjTr)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player)
            {
                followObjTr = player.transform;
            }
        }
        
            //boundaryPosToRay = new Vector3[4];
            //boundaryDirToRay = new Vector3[4];
            //boundaryPosToView = new Vector3[4];

    }

    void Start()
    {
        //1. 플레이어를 중앙에 두는 위치로 카메라 이동
        CameraCallibration(followObjTr.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void LateUpdate()
	{
		//플레이어 이동보다 느리게 ㄱㄱ

		for (int i = 0; i < (int)eBoundary.End; ++i)
		{
			ComputeCamBoundaryRay(Defines.ViewportPos[i], ref boundaryPosToRay[i]);
		}
		Zoom();
        Following();
    }

	private void FixedUpdate()
	{
        
    }


	private void OnDrawGizmosSelected()
	{

#if UNITY_EDITOR
        Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
		for (int i = 0; i < (int)eBoundary.End; ++i)
		{
			Gizmos.DrawSphere(boundaryPosToRay[i], 1f);
		}
#endif
    }
}
