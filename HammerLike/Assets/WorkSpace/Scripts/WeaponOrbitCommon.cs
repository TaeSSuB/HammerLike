using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// WeaponOrbit : 무기 궤도 클래스
/// </summary>
public class WeaponOrbitCommon : MonoBehaviour
{
    /// <summary>
    /// 궤도 트래킹 타입
    /// </summary>
    public enum TrackType
    {
        None,
        DirBasedPoint
    }

    [Header("Orbit Settings")]
    public float a = 5.0f;  // 타원의 긴 축
    public float b = 3.0f;  // 타원의 짧은 축
    public int resolution = 100;  // 궤도 해상도
    public Vector3 offset;  // 궤도 위치 오프셋

    private Vector3 bestMatchPoint;
    private float smallestAngle = 180.0f;  // 최대 각도 차이

    public float speed = 1.0f;  // 궤도를 도는 속도
    private float angle = 0.0f;  // 초기 각도

    public TrackType trackType = TrackType.DirBasedPoint;

    public bool isTracking = true;


    [Header("References")]
    [SerializeField] private GameObject centerObj;
    [SerializeField] private GameObject targetObj;  // 추적할 오브젝트. 철퇴 등 할당

    [Header("Debug")]
    public bool debugMode = false;
    public int edgeCount = 32;  // 궤도 기즈모 각

    public GameObject TargetObj { get => targetObj; }

    private void Update()
    {
        if(isTracking)
        {
            switch (trackType)
            {
                case TrackType.None:
                    UpdateOrbit(); 
                    break;
                case TrackType.DirBasedPoint:
                    TrackDirBasedPoint();
                    break;
                default:
                    break;
            }
        }
        
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
        Vector3 weaponDirection = (targetObj.transform.position - centerPosition).normalized;  // 무기에서 플레이어로의 방향 벡터

        // 초기화
        smallestAngle = 180.0f;

        // 궤도 위치 계산. 2 * Mathf.PI는 360도. 즉, for문 루프 동안 Orbit을 한 바퀴 돌며 체크.
        for (float t = 0; t < 2 * Mathf.PI; t += step)
        {
            // 궤도 상 위치 계산. 타원 방정식 이용, 중심점 기준 계산.
            Vector3 orbitPoint = new Vector3(centerPosition.x + a * Mathf.Cos(t), centerPosition.y, centerPosition.z + b * Mathf.Sin(t));
            // normalize
            Vector3 orbitDirection = (orbitPoint - centerPosition).normalized;

            // 무기와 궤도 위치 사이의 각도 계산
            float angleDifference = Vector3.Angle(orbitDirection, weaponDirection);

            // 가장 방향이 일치하는 궤도 위치 찾기
            if (angleDifference < smallestAngle)
            {
                smallestAngle = angleDifference;
                bestMatchPoint = orbitPoint;
            }
        }

        // 가장 방향이 일치하는 궤도 위치로 이동
        targetObj.transform.position = bestMatchPoint + offset;
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
        targetObj.transform.position = new Vector3(x, 0, z) + centerObj.transform.position + offset;
    }

    /// <summary>
    /// TrackDirBasedPoint Based 궤도 위치와 가장 가까운 궤도 위치를 시각화
    /// </summary>
    private void OnDrawGizmos()
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
        Vector3 centerObjPosition = centerObj.transform.position;
        for (float t = 0; t < 2 * Mathf.PI; t += step)
        {
            Vector3 orbitPoint = new Vector3(centerObjPosition.x + a * Mathf.Cos(t), centerObjPosition.y, centerObjPosition.z + b * Mathf.Sin(t));
            Gizmos.DrawSphere(orbitPoint, 0.1f);
        }

    }
}
