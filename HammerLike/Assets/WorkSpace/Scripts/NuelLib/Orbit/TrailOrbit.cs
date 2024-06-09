using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TrailOrbit : 궤도형 Trail 클래스
/// </summary>
public class TrailOrbit : MonoBehaviour
{
    /// <summary>
    /// 궤도 트래킹 타입
    /// </summary>
    public enum TrackType
    {
        None,
        NearPoint,
        DirBasedPoint
    }

    [Header("Orbit Settings")]
    [SerializeField] protected GameObject rootObj; // 움직이는 주체
    public bool isTracking = true;  // 트래킹 활성화 여부
    public float a = 5.0f;  // 타원의 긴 축
    public float b = 3.0f;  // 타원의 짧은 축
    public int resolution = 100;  // 궤도 해상도
    public float speed = 1.0f;  // 궤도를 도는 속도
    public Vector3 offset;  // 궤도 위치 오프셋

    private Vector3 bestMatchPoint;
    private float smallestAngle = 180.0f;  // 최대 각도 차이

    private Vector3 closestPoint;
    private float closestDistance = Mathf.Infinity;

    private float angle = 0.0f;  // 초기 각도

    [SerializeField] protected TrackType trackType = TrackType.DirBasedPoint;

    [Header("References")]
    public bool instanceMode = false;  // 인스턴스 모드
    public float trailLifeTime = 0.25f;  // 트레일 라이프 타임
    public TrailRenderer trailRenderer;  // 트레일 렌더러 컴포넌트
    public Material trailMaterial;  // 트레일 머티리얼
    
    protected Queue<TrailRenderer> trailRendererPool;  // 트레일 렌더러 풀

    protected float startWidth = 0.1f;  // 트레일 시작 폭
    protected float endWidth = 0.05f;  // 트레일 끝 폭

    [SerializeField] protected GameObject targetObj;  // 추적할 오브젝트
    public GameObject centerObj;  // 타겟 오브젝트
    public float startWidthScale = 0.1f;  // 트레일 시작 폭
    public float endWidthScale = 0.05f;  // 트레일 끝 폭

    [Header("Debug")]
    public int edgeCount = 32;  // 궤도 기즈모 각

    protected virtual void Start()
    {
        if (rootObj == null)
        {
            rootObj = gameObject;
        }

        if(!instanceMode)
        {
            if (trailRenderer == null)
            {
                trailRenderer = GetComponent<TrailRenderer>();
                
                if(trailRenderer == null)
                {
                    trailRenderer = gameObject.AddComponent<TrailRenderer>();
                }
            }

            if(trailRenderer != null)
            {
                trailRenderer.emitting = false;
                trailMaterial = trailRenderer.material;
            }
        }
        else
        {
            trailRendererPool = new Queue<TrailRenderer>();
        }
    }

    // 이후 거리 단위 AddPosition 및 FixedUpdate로 변경
    protected virtual void Update()
    {
        if(isTracking)
        {
            switch (trackType)
            {
                case TrackType.None:
                    UpdateOrbit(); 
                    break;
                case TrackType.NearPoint:
                    TrackNearPoint();
                    break;
                case TrackType.DirBasedPoint:
                    TrackDirBasedPoint();
                    break;
            }
        }
    }

    protected GameObject InstansiateTrailRendererObj(bool inEmitting = false, float inLifeTime = 0.25f, bool inAutoDestruct = false)
    {
        GameObject trailObj = new GameObject("PlayerWeaponTrail");
        trailObj.transform.SetParent(rootObj.transform);    
        trailRenderer = trailObj.AddComponent<TrailRenderer>();

        trailRenderer.material = trailMaterial;

        trailRenderer.startWidth = startWidth;
        trailRenderer.endWidth = endWidth;

        trailRenderer.emitting = inEmitting;
        trailRenderer.time = inLifeTime;
        //trailRenderer.autodestruct = inAutoDestruct;

        // Temp. 레이어 구분 일단 UI로 세팅
        trailRenderer.sortingLayerName = "UI";

        //trailRendererPool.Add(trailRenderer);
        trailRendererPool.Enqueue(trailRenderer);
        StartCoroutine(CoDestroyTrailRendererObj(trailObj, inLifeTime));
        

        return trailObj;
    }

    IEnumerator CoDestroyTrailRendererObj(GameObject inTrailObj, float inLifeTime)
    {
        yield return new WaitForSeconds(inLifeTime);
        Destroy(inTrailObj);
    }

    /// <summary>
    /// 트레일 폭 조정
    /// </summary>
    protected void AdjustTrailWidth(Renderer inRenderer)
    {
        float boundsLength = inRenderer.bounds.size.magnitude;  // 무기의 길이 계산

        startWidth = boundsLength * startWidthScale;  // 트레일의 시작 폭 설정
        endWidth = boundsLength * endWidthScale;  // 트레일의 끝 폭 설정

        if(!instanceMode)
        {
            trailRenderer.startWidth = startWidth;  // 트레일의 시작 폭 설정
            trailRenderer.endWidth = endWidth;  // 트레일의 끝 폭 설정
        }
    }

    protected void ApplyMaterial(Material inMaterial)
    {
        trailMaterial = inMaterial;
        
        if(!instanceMode)
            trailRenderer.material = trailMaterial;
    }

    /// <summary>
    /// 방향 벡터를 이용한 궤도 위치 추적
    /// 무기-플레이어 방향 벡터를 계산, 궤도 위치와 방향 벡터 사이 각도 계산 및 가장 근접한 궤도 계산.
    /// </summary>
    void TrackDirBasedPoint()
    {
        // 해상도 값으로 디테일 조정. resolution 값이 클수록 정확도가 높아진다. 성능 영향 주의.
        float step = 2 * Mathf.PI / resolution;

        Vector3 centerPosition = centerObj.transform.position;
        Vector3 targetDirection = (targetObj.transform.position - centerPosition).normalized;  // 무기에서 플레이어로의 방향 벡터

        // 초기화
        smallestAngle = 180.0f;

        // 궤도 위치 계산. 2 * Mathf.PI는 360도. 즉, for문 루프 동안 Orbit을 한 바퀴 돌며 체크.
        for (float t = 0; t < 2 * Mathf.PI; t += step)
        {
            // 궤도 상 위치 계산. 타원 방정식 이용, 플레이어 중심점 기준 계산.
            Vector3 orbitPoint = new Vector3(centerPosition.x + a * Mathf.Cos(t), centerPosition.y, centerPosition.z + b * Mathf.Sin(t));
            // normalize
            Vector3 orbitDirection = (orbitPoint - centerPosition).normalized;

            // 무기와 궤도 위치 사이의 각도 계산
            float angleDifference = Vector3.Angle(orbitDirection, targetDirection);

            // 가장 방향이 일치하는 궤도 위치 찾기
            if (angleDifference < smallestAngle)
            {
                smallestAngle = angleDifference;
                bestMatchPoint = orbitPoint;
            }
        }

        rootObj.transform.position = bestMatchPoint + offset;
    }

    public void AddTrackInstanceTrail()
    {
        // 가장 방향이 일치하는 궤도 위치로 이동
        if(instanceMode)
        {
            var trailObj = InstansiateTrailRendererObj(true, trailLifeTime, true);
            trailObj.transform.position = bestMatchPoint + offset;
        }
    }

    public void ClearTrackInstanceTrail()
    {
        if(instanceMode)
        {
            if(trailRendererPool.Count > 0)
            {
                var trailObj = trailRendererPool.Dequeue();
                trailObj.transform.SetParent(null);
                //trailObj.Clear();
                //Destroy(trailObj);
            }
        }
    }

    #region Legacy

    /// <summary>
    /// 가장 가까운 궤도 위치를 추적
    /// resolution만큼의 각도로 궤도 위치를 계산하고, 무기와 가장 가까운 궤도 위치를 찾는다.
    /// </summary>
    void TrackNearPoint()
    {
        // 해상도 값으로 디테일 조정. resolution 값이 클수록 정확도가 높아진다. 성능 영향 주의.
        float step = 2 * Mathf.PI / resolution;

        closestDistance = Mathf.Infinity;
        Vector3 centerPosition = centerObj.transform.position;

        // 궤도 위치 계산. 2 * Mathf.PI는 360도. Orbit을 한 바퀴 돌며 체크한다.
        for (float t = 0; t < 2 * Mathf.PI; t += step)
        {
            // 궤도 상의 위치를 계산한다. 타원의 방정식을 이용하며, 플레이어 중심점을 기준으로 계산한다.
            Vector3 orbitPoint = new Vector3(centerPosition.x + a * Mathf.Cos(t), centerPosition.y, centerPosition.z + b * Mathf.Sin(t));

            // 무기와 궤도 위치 사이의 거리를 계산한다.
            float distance = Vector3.Distance(targetObj.transform.position, orbitPoint);

            // 가장 가까운 궤도 위치를 찾는다.
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = orbitPoint;
            }
        }

        // 업데이트된 가장 가까운 궤도 위치로 이동
        rootObj.transform.position = closestPoint + offset;
    }

    /// <summary>
    /// 일반적인 원형 궤도
    /// </summary>
    void UpdateOrbit()
    {
        // 각도 업데이트
        angle += speed * Time.deltaTime;

        // 타원 궤도 계산
        float x = a * Mathf.Cos(angle);
        float z = b * Mathf.Sin(angle);

        // 물체의 위치 설정
        rootObj.transform.position = new Vector3(x, 0, z) + centerObj.transform.position + offset;
    }

    #endregion

    #region DEBUG
    /// <summary>
    /// TrackDirBasedPoint Based 궤도 위치와 가장 가까운 궤도 위치를 시각화
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        // TrackNearPoint
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(closestPoint, 0.5f);

        // TrackDirBasedPoint2
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(bestMatchPoint, 0.5f);

        // 타원 궤도 시각화
        Gizmos.color = Color.blue;
        
        if(edgeCount < 3) edgeCount = 3;
        else if (edgeCount > 100) edgeCount = 100;

        float step = 2 * Mathf.PI / edgeCount;
        Vector3 centerPosition = centerObj.transform.position;
        for (float t = 0; t < 2 * Mathf.PI; t += step)
        {
            Vector3 orbitPoint = new Vector3(centerPosition.x + a * Mathf.Cos(t), centerPosition.y, centerPosition.z + b * Mathf.Sin(t));
            Gizmos.DrawSphere(orbitPoint, 0.1f);
        }
    }

    #endregion
    
}
