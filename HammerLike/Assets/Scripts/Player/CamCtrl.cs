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

public enum FollowOption
{
    FollowToObject,
    LimitedInRoom
}

[System.Serializable]
public class ResolutionRef
{
    public int width;
    public int height;
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

    
    [Header("Cams")]
    public Camera mainCam;  //For RenderTex = mainCam
    public Camera subCam;


    [Space(10f)]
    [Header("Resolution")]
    public RenderTexture renderTex; //Only control zoom using Height vari, For Screen Render
    public ResolutionRef resolutionRef;

    [Space(10f)]
    [Header("Follow")]
    public FollowOption followOption;
    public Transform followObjTr;
    public AnimationCurve followSpdCurve;
    private float curveTime = 0f;
    public float followSpd;
    public float followDistOffset;
    public GameObject currentRoom; // 인스펙터에서 현재 방을 볼 수 있도록

    [Space(10f)]
    [Header("Zoom")]
    public bool zoomOption;
    public PixelPerfectCamera zoomCam; //Only control zoom using Height vari, For Screen Render
    public float zoomMin;
    public float zoomMax;
    public float zoomSpd;
    public AnimationCurve zoomSpdCrv;
    private float zoomCurveTime = 0f;
    private Bounds roomBounds;


    [Space(10f)]
    [Header("ETC")]
    public Vector3 worldScaleCrt;
    //public Vector3[] boundaryDir; //카메라의 모서리+중앙 Direction 
    public float distToGround;

    //MainCam의 바닥에 닿는 4군대 모서리
    public Vector3[] boundaryPosToRay = new Vector3[(int)eBoundary.End];
    //public Vector3[] boundaryDirToRay;
    //public Vector3[] boundaryPosToView;

    public void CameraCallibration(Vector3 centerPos)
    {
            
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

    // 현재 카메라가 비추고 있는 방 감지
    void DetectCurrentRoom()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit))
        {
            if (hit.collider.tag == "Room")
            {
                currentRoom = hit.collider.gameObject;
                CalculateRoomBounds();
            }
        }
        //Debug.Log("현재 방 :" + currentRoom);
    }

    void CalculateRoomBounds()
    {
        if (currentRoom != null)
        {
            Collider roomCollider = currentRoom.GetComponent<Collider>();
            if (roomCollider != null)
            {
                roomBounds = roomCollider.bounds;
            }
        }
    }

    bool CameraBoundaryCheck(Vector3 proposedPosition)
    {
        return roomBounds.Contains(proposedPosition);
    }

    public void Following()
    {
        float dist = Vector3.Distance(followObjTr.position, boundaryPosToRay[(int)eBoundary.Center]);

        if (dist <= followDistOffset)
        {
            // 카메라가 객체를 따라가지 않을 때 커브 시간을 초기화합니다
            curveTime = 0f;
            return;
        }

        // 커브 시간을 1초 동안 증가시킵니다 (1초가 지나면 다시 0으로 초기화)
        curveTime += Time.deltaTime;
        if (curveTime > 1f)
        {
            curveTime = 0f;
        }

        // 커브를 사용하여 속도를 계산합니다
        float speed = followSpdCurve.Evaluate(curveTime) * followSpd;

        Vector3 dir = (followObjTr.position - boundaryPosToRay[(int)eBoundary.Center]).normalized;
        Vector3 prePos = mainCam.transform.position;
        Vector3 tempPos = mainCam.transform.position + dir * speed * Time.unscaledDeltaTime;
        mainCam.transform.position = tempPos;

        bool isHorizontalBlocked = false;
        bool isVerticalBlocked = false;

        // 경계 체크를 위한 레이캐스팅
        for (int i = 0; i < (int)eBoundary.End; ++i)
        {
            Vector3 worldPos = mainCam.ViewportToWorldPoint(new Vector3(Defines.ViewportPos[i].x, Defines.ViewportPos[i].y, mainCam.nearClipPlane));
            Vector3 camDir = mainCam.transform.forward;
            var ray = new Ray(worldPos, camDir);
            var rayResult = new RaycastHit();
            if (!Physics.Raycast(ray, out rayResult, 100f, LayerMask.GetMask("Ground")))
            {
                if (i == (int)eBoundary.LT || i == (int)eBoundary.RT || i == (int)eBoundary.LB || i == (int)eBoundary.RB)
                {
                    isHorizontalBlocked = true;
                }
                else
                {
                    isVerticalBlocked = true;
                }
            }
        }

        // 수평 방향이 막혔을 경우
        if (isHorizontalBlocked)
        {
            mainCam.transform.position = new Vector3(prePos.x, mainCam.transform.position.y, mainCam.transform.position.z);
        }

        // 수직 방향이 막혔을 경우
        if (isVerticalBlocked)
        {
            mainCam.transform.position = new Vector3(mainCam.transform.position.x, prePos.y, mainCam.transform.position.z);
        }
    }




    public void Zoom()
    {
        float wheelVal = Input.GetAxis("Mouse ScrollWheel");

        if (wheelVal == 0f)
        {
            // 줌 상태가 아니면 커브 시간 초기화
            zoomCurveTime = 0f;
            return;
        }
        else
        {
            // 줌 상태면 커브 시간 업데이트
            zoomCurveTime += Time.deltaTime;
        }

        float zoomSpdValue = zoomSpdCrv.Evaluate(zoomCurveTime) * zoomSpd;
        float zoomVal = (-1f * wheelVal * zoomSpdValue) + zoomCam.targetCameraHalfHeight;

        zoomCam.targetCameraHalfHeight = Mathf.Clamp(zoomVal, zoomMin, zoomCam.maxCameraHalfHeight);

        zoomCam.adjustCameraFOV();
    }


    private void Awake()
    {
        if (!mainCam)
        {
            mainCam = Camera.main;
        }

        if (mainCam != null && renderTex != null)
        {
            mainCam.targetTexture = renderTex; // RenderTexture를 카메라의 targetTexture로 설정
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
        if (followOption == FollowOption.FollowToObject)
            CameraCallibration(followObjTr.position);

        resolutionRef.width = renderTex.width;
        resolutionRef.height = renderTex.height;
    }

    private void ManageZoomCamera()
    {
        if (zoomOption)
        {
            // ZoomOption이 true일 경우, PixelPerfectCamera 컴포넌트 추가
            if (!zoomCam)
            {
                zoomCam = gameObject.AddComponent<PixelPerfectCamera>();
                // 필요한 경우, zoomCam 변수의 초기 설정을 여기서 수행
            }
        }
        else
        {
            // ZoomOption이 false일 경우, PixelPerfectCamera 컴포넌트 제거
            if (zoomCam)
            {
                Destroy(zoomCam);
                zoomCam = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ManageZoomCamera();
    }

	private void LateUpdate()
	{
        DetectCurrentRoom();
        //플레이어 이동보다 느리게 ㄱㄱ

        for (int i = 0; i < (int)eBoundary.End; ++i)
		{
			ComputeCamBoundaryRay(Defines.ViewportPos[i], ref boundaryPosToRay[i]);
		}
		Zoom();
        if (followOption == FollowOption.FollowToObject)
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
