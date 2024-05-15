using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Johnson;
using Unity.VisualScripting;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;
using TMPro;

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


public class CamCtrl : MonoBehaviour
{

    public float cursorFollowThreshold = 5.0f;
    public float viewportThreshold = 0.3f;


    [Header("Cams")]
    public Camera mainCam;  //For RenderTex = mainCam
    //public Camera subCam; // Delete Zoom Camera

    private Vector3 lastCursorPosition;
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
    public GameObject currentRoom; 

    [Space(10f)]
    [Header("Zoom")]
    public bool cursorFollow=false;
    public bool zoomOption;
    private float zoomDuration = 2.0f;
    private float orthographicSize;
    public PixelPerfectCamera zoomCam; //Only control zoom using Height vari, For Screen Render
    public float zoomMin;
    public float zoomMax;
    public float zoomSpd;
    public AnimationCurve zoomSpdCrv;
    private float zoomTimer  = 0f;
    private Bounds roomBounds;
    //private PlayerAtk playerAtk;
    private bool shouldReturnToInitialZoom = false; 
    private float returnZoomStartTime;
    [Space(10f)]
    [Header("ETC")]
    public Vector3 worldScaleCrt;

    public float distToGround;
    private int initialCullingMask;
    [Space(10f)]
    [Header("Camera Limits")]
    public CameraBounds cameraBounds;

    private Vector3 cameraOffset;



    
    public Vector3[] boundaryPosToRay = new Vector3[(int)eBoundary.End];
    

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
    


    private void Awake()
    {
        if (!mainCam)
        {
            mainCam = Camera.main;
        }

        if (mainCam != null && renderTex != null)
        {
            mainCam.targetTexture = renderTex; 
        }
        
        initialCullingMask = mainCam.cullingMask;

        if (!zoomCam)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                GameObject child = transform.GetChild(i).gameObject;

                if (child.name.Equals("Zoom Camera"))
                {
                    
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



    }

    void Start()
    {
        orthographicSize = mainCam.orthographicSize;
        lastCursorPosition = Input.mousePosition;
        resolutionRef.width = renderTex.width;
        resolutionRef.height = renderTex.height;
        cameraOffset = mainCam.transform.position;
        if (zoomCam != null)
        {
            //initialZoom = zoomCam.targetCameraHalfHeight;
        }
    }

    private void ManageZoomCamera()
    {
        if (zoomOption)
        {
            
            if (!zoomCam)
            {
                //zoomCam = gameObject.AddComponent<PixelPerfectCamera>();
                
            }
        }
        else
        {
            
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

    }

    void ZoomIn()
    {
        if (zoomOption)
        {
            float targetZoom = Mathf.Max(mainCam.orthographicSize - zoomSpd, zoomMin);
            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, targetZoom, Time.deltaTime * zoomSpd);
        }
    }

    void ZoomOut()
    {
        if (zoomOption && shouldReturnToInitialZoom)
        {
            if (Mathf.Abs(mainCam.orthographicSize - orthographicSize) < 0.01f)
            {
                mainCam.orthographicSize = orthographicSize; 
                shouldReturnToInitialZoom = false; 
            }
            else
            {
                mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, orthographicSize, Time.deltaTime * zoomSpd);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        ManageZoomCamera();
        HandleZoomWithMouseWheel();



        /*if (Input.GetMouseButtonUp(0) && cursorFollow)
        {
            cursorFollow = false;
            shouldReturnToInitialZoom = true; // Start zooming out
        }
        else if (Input.GetMouseButton(0))
        {
            ZoomIn();
            if (!cursorFollow)
                cursorFollow = true;
        }

        if (shouldReturnToInitialZoom)
        {
            ZoomOut();
        }*/

         if (Input.GetMouseButton(0))
        {
            ZoomIn();
            if (!cursorFollow)
                cursorFollow = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            shouldReturnToInitialZoom = true;
        }
         else
        {
            ZoomOut();

            if (cursorFollow)
                cursorFollow= false;
        }
    }

    private void LateUpdate()
    {
        DetectCurrentRoom();


        for (int i = 0; i < (int)eBoundary.End; ++i)
        {
            ComputeCamBoundaryRay(Defines.ViewportPos[i], ref boundaryPosToRay[i]);
        }

        if (cursorFollow)
        {
            CursorFollowMode();
            
        }
        else
        {
            if (followOption == FollowOption.FollowToObject)
                Following();
            if (followOption == FollowOption.LimitedInRoom)
                LimitedInRoomFollow();
            if (followOption == FollowOption.DirectFollow)
                DirectFollow();
        }
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

    private void CursorFollowMode()
    {

        Vector3 mousePos = mainCam.ScreenToViewportPoint(Input.mousePosition);
        


        Vector3 cursorPos = (new Vector3(mousePos.x, 0, mousePos.y) - new Vector3(0.5f, 0, 0.5f)) * 2f*cursorFollowThreshold;
        
        Vector3 targetPos = followObjTr.position + cursorPos;

        targetPos.x = Mathf.Clamp(targetPos.x, -cursorFollowThreshold + followObjTr.position.x, cursorFollowThreshold + followObjTr.position.x);
        targetPos.y = cameraOffset.y;
        targetPos.z = Mathf.Clamp(targetPos.z, -cursorFollowThreshold*1.6f + followObjTr.position.z, cursorFollowThreshold*1.6f + followObjTr.position.z);

        targetPos.z += cameraOffset.z;

        float speed = followSpdCurve.Evaluate(curveTime) * followSpd;
        
        // 카메라를 목표 위치로 부드럽게 이동
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, targetPos, speed * Time.deltaTime);
    }

    public void AddLayerToCullingMask(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogError("Layer not found: " + layerName);
            return;
        }
        mainCam.cullingMask |= (1 << layer);
    }


    public void RemoveLayerFromCullingMask(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogError("Layer not found: " + layerName);
            return;
        }
        mainCam.cullingMask &= ~(1 << layer);
    }

    public void ExcludeAllLayers()
    {
        mainCam.cullingMask = 0; // 모든 레이어 제외
    }



    public void ResetCullingMask()
    {
        mainCam.cullingMask = initialCullingMask; // 초기 상태로 복원
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