using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Johnson;
using Unity.VisualScripting;

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
    LimitedInRoom,
    DirectFollow
}

[System.Serializable]
public class ResolutionRef
{
    public int width;
    public int height;
}

[System.Serializable]
public class CameraBounds
{
    public Vector3 min; // Minimum XYZ coordinates of the camera boundary
    public Vector3 max; // Maximum XYZ coordinates of the camera boundary
}


//231025 1152 移대찓???ㅻⅨ ?곗텧???덉뼱??
//???묒뾽 ?덊빐??????

public class CamCtrl : MonoBehaviour
{

    /// <summary>
    /// 231025 1152
    /// ?뚯뒪?몄슜 ?ㅽ겕由쏀듃??!!!
    /// ?섏쨷??移대찓?????묒뾽?좊븣???명븯寃?吏?곌굅???섏젙?댁꽌 ?곗떗??
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
    public GameObject currentRoom; // ?몄뒪?숉꽣?먯꽌 ?꾩옱 諛⑹쓣 蹂????덈룄濡?

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
    //private PlayerAtk playerAtk;
    public float initialZoom; // 珥덇린 以?媛???μ쓣 ?꾪븳 蹂??
    private bool shouldReturnToInitialZoom = false; // 珥덇린 以?媛믪쑝濡??뚯븘媛?붿? ?щ?
    private float returnZoomStartTime;
    [Space(10f)]
    [Header("ETC")]
    public Vector3 worldScaleCrt;
    //public Vector3[] boundaryDir; //移대찓?쇱쓽 紐⑥꽌由?以묒븰 Direction 
    public float distToGround;

    [Space(10f)]
    [Header("Camera Limits")]
    public CameraBounds cameraBounds;

    private Vector3 cameraOffset;

    //MainCam??諛붾떏???용뒗 4援곕? 紐⑥꽌由?
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
            //留뚯빟 諛붾떏 泥댄겕 ?덈릺硫?洹몃깷 ?꾩떆濡??섎굹 留뚮뱾怨?泥댄겕?섍퀬 ?섏꽌 ?놁븷????


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

    // ?꾩옱 移대찓?쇨? 鍮꾩텛怨??덈뒗 諛?媛먯?
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
        //Debug.Log("?꾩옱 諛?:" + currentRoom);
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
        Vector3 targetPosition = followObjTr.position + cameraOffset;

        // 카메라와 목표 위치 사이의 거리 계산
        float dist = Vector3.Distance(mainCam.transform.position, targetPosition);

        // 목표 위치와의 거리가 너무 작으면 카메라 이동을 중지
        if (dist <= followDistOffset)
        {
            curveTime = 0f;  // 커브 시간 초기화
            return;
        }

        // 커브 시간을 1초 동안 증가 (1초 후 초기화)
        curveTime += Time.deltaTime;
        if (curveTime > 1f)
        {
            curveTime = 0f;
        }

        // 속도를 커브 함수로 계산
        float speed = followSpdCurve.Evaluate(curveTime) * followSpd;

        // 카메라를 목표 위치로 부드럽게 이동
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, targetPosition, speed * Time.deltaTime);

    }
    public void Zoom()
    {
        /*bool isLeftMouseDown = Input.GetMouseButton(0);

        if (!isLeftMouseDown && !shouldReturnToInitialZoom)
        {
            // 留덉슦???쇱そ 踰꾪듉???볦뿬吏怨? ?꾩쭅 珥덇린 以뚯쑝濡??뚯븘媛??怨쇱젙???쒖옉?섏? ?딆븯?ㅻ㈃
            shouldReturnToInitialZoom = true;
            returnZoomStartTime = Time.time;
        }

        if (shouldReturnToInitialZoom)
        {
            // 珥덇린 以?媛믪쑝濡??뚯븘媛??怨쇱젙
            float timeSinceReturnStart = Time.time - returnZoomStartTime;
            float t = timeSinceReturnStart / 1.0f; // 1珥??숈븞??蹂?붾? 怨꾩궛

            if (t >= 1.0f)
            {
                t = 1.0f;
                shouldReturnToInitialZoom = false; // ?뚯븘媛??怨쇱젙 ?꾨즺
            }

            zoomCam.targetCameraHalfHeight = Mathf.Lerp(zoomCam.targetCameraHalfHeight, initialZoom, t);
            zoomCam.adjustCameraFOV();

            return; // 異붽??곸씤 以?議곗젙??以묐떒?⑸땲??
        }

        if (isLeftMouseDown)
        {
            // 留덉슦???쇱そ 踰꾪듉???뚮젮?덉쑝硫?以?議곗젅
            zoomCurveTime += Time.deltaTime;
            float zoomSpdValue = zoomSpdCrv.Evaluate(zoomCurveTime) * zoomSpd;
            //float zoomSpdValue = zoomSpd;
            float zoomVal = (-1f * 0.5f * zoomSpdValue) + zoomCam.targetCameraHalfHeight;

            zoomCam.targetCameraHalfHeight = Mathf.Clamp(zoomVal, zoomMin, zoomCam.maxCameraHalfHeight);
            zoomCam.adjustCameraFOV();
        }*/
    }
    /*public void Zoom()
    {
        bool isLeftMouseDown = Input.GetMouseButton(0);

        if (!isLeftMouseDown)
        {
            // 留덉슦???쇱そ 踰꾪듉???뚮젮?덉? ?딆쑝硫?而ㅻ툕 ?쒓컙 珥덇린??
            zoomCurveTime = 0f;

            // 移대찓??以뚯쓣 珥덇린 媛믪쑝濡?蹂듭썝
            zoomCam.targetCameraHalfHeight = initialZoom;
            zoomCam.adjustCameraFOV(); // 移대찓??FOV瑜?議곗젙?⑸땲??
            return; // 異붽??곸씤 以?議곗젙??以묐떒?⑸땲??
        }
        else
        {
            // 留덉슦???쇱そ 踰꾪듉???뚮젮?덉쑝硫?而ㅻ툕 ?쒓컙 ?낅뜲?댄듃
            zoomCurveTime += Time.deltaTime;
        }


        float zoomSpdValue = zoomSpdCrv.Evaluate(zoomCurveTime) * zoomSpd;
        //float zoomSpdValue = zoomSpd;
        float zoomVal = (-1f * 1f * zoomSpdValue) + zoomCam.targetCameraHalfHeight;

        zoomCam.targetCameraHalfHeight = Mathf.Clamp(zoomVal, zoomMin, zoomCam.maxCameraHalfHeight);

        zoomCam.adjustCameraFOV();
    }*/


    private void Awake()
    {
        if (!mainCam)
        {
            mainCam = Camera.main;
        }

        if (mainCam != null && renderTex != null)
        {
            mainCam.targetTexture = renderTex; // RenderTexture瑜?移대찓?쇱쓽 targetTexture濡??ㅼ젙
        }


        if (!zoomCam)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                GameObject child = transform.GetChild(i).gameObject;

                if (child.name.Equals("Zoom Camera"))
                {
                    //?댁감????珥덇린???④퀎?먯꽌 李얜뒗嫄곌린???ш쾶 ?좉꼍 ?덉벐????
                    //string?뺤쑝濡??ㅻ툕?앺듃 李얜뒗嫄??꾩???紐살갭??寃쎌슦 ?놁븷??臾대갑.
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
        //1. ?뚮젅?댁뼱瑜?以묒븰???먮뒗 ?꾩튂濡?移대찓???대룞
        /*if (followOption == FollowOption.FollowToObject)
            CameraCallibration(followObjTr.position);
*/
        resolutionRef.width = renderTex.width;
        resolutionRef.height = renderTex.height;
        cameraOffset = mainCam.transform.position;
        if (zoomCam != null)
        {
            initialZoom = zoomCam.targetCameraHalfHeight;
        }
    }

    private void ManageZoomCamera()
    {
        if (zoomOption)
        {
            // ZoomOption??true??寃쎌슦, PixelPerfectCamera 而댄룷?뚰듃 異붽?
            if (!zoomCam)
            {
                zoomCam = gameObject.AddComponent<PixelPerfectCamera>();
                // ?꾩슂??寃쎌슦, zoomCam 蹂?섏쓽 珥덇린 ?ㅼ젙???ш린???섑뻾
            }
        }
        else
        {
            // ZoomOption??false??寃쎌슦, PixelPerfectCamera 而댄룷?뚰듃 ?쒓굅
            if (zoomCam)
            {
                Destroy(zoomCam);
                zoomCam = null;
            }
        }
    }

    public void ChangeFollowOption()
    {
        if (followOption == FollowOption.FollowToObject)
        {
            followOption = FollowOption.LimitedInRoom;
        }
        else if (followOption == FollowOption.LimitedInRoom)
        {
            followOption = FollowOption.FollowToObject;
        }

    }

    void HandleZoomWithMouseWheel()
    {
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0 && zoomOption)
        {
            Zoom(mouseScroll);
        }
    }

    void Zoom(float scrollAmount)
    {
        // subCam??projection size瑜?留덉슦?????낅젰???곕씪 議곗젅?⑸땲??
        if (subCam != null)
        {
            // ?꾩옱 ?ъ씠利덉뿉 ?낅젰 媛믪뿉 ?곕Ⅸ 蹂?붾웾??異붽??⑸땲?? scrollAmount媛 ?묒닔硫??뺣?, ?뚯닔硫?異뺤냼
            float newSize = subCam.orthographicSize - scrollAmount * zoomSpd;
            // newSize媛 理쒖냼媛믨낵 理쒕?媛??ъ씠???덈뒗吏 ?뺤씤?섍퀬 議곗젅?⑸땲??
            newSize = Mathf.Clamp(newSize, zoomMin, zoomMax);
            // 怨꾩궛?????ъ씠利덈? subCam??projection size濡??ㅼ젙?⑸땲??
            subCam.orthographicSize = newSize;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ManageZoomCamera();
        HandleZoomWithMouseWheel();
    }

    private void LateUpdate()
    {
        DetectCurrentRoom();
        //?뚮젅?댁뼱 ?대룞蹂대떎 ?먮━寃??긱꽦

        for (int i = 0; i < (int)eBoundary.End; ++i)
        {
            ComputeCamBoundaryRay(Defines.ViewportPos[i], ref boundaryPosToRay[i]);
        }
        Zoom();
        if (followOption == FollowOption.FollowToObject)
            Following();
        if (followOption == FollowOption.LimitedInRoom)
            LimitedInRoomFollow();
        if (followOption == FollowOption.DirectFollow)
            DirectFollow();
    }

    public void DirectFollow()
    {
        Vector3 prePos = mainCam.transform.position;
        Vector3 targetPosition = followObjTr.transform.position;

        // Y異뺤쓽 ?꾩튂瑜?怨좎젙?⑸땲??
        targetPosition.y = prePos.y;

        // 怨듭떇 = a=hcot(罐)
        float angle = mainCam.transform.eulerAngles.x;
        float radian = angle * Mathf.Deg2Rad;
        float height = prePos.y;
        height = Mathf.Abs(height);

        float distance = height / Mathf.Tan(radian);

        targetPosition.z += -distance;

        mainCam.transform.position = targetPosition;

    }

    private void LimitedInRoomFollow()
    {
        // 카메라를 제한된 방 경계 내에서만 이동하도록 구현
        Vector3 targetPosition = followObjTr.position + cameraOffset;
        targetPosition.x = Mathf.Clamp(targetPosition.x, cameraBounds.min.x, cameraBounds.max.x);
        targetPosition.z = Mathf.Clamp(targetPosition.z, cameraBounds.min.z, cameraBounds.max.z);
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, targetPosition, followSpd * Time.deltaTime);
    }


    private bool IsWithinBounds(Vector3 position)
    {
        return position.x >= cameraBounds.min.x && position.x <= cameraBounds.max.x &&
               position.y >= cameraBounds.min.y && position.y <= cameraBounds.max.y &&
               position.z >= cameraBounds.min.z && position.z <= cameraBounds.max.z;
    }

    public void UpdateCameraBounds(Bounds roomBounds)
    {
        // 카메라의 최소 및 최대 XYZ 좌표 설정
        cameraBounds.min = new Vector3(roomBounds.min.x + 15.0f, cameraBounds.min.y, roomBounds.min.z);
        cameraBounds.max = new Vector3(roomBounds.max.x - 15.0f, cameraBounds.max.y, roomBounds.max.z - 45.0f);
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