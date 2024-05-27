using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// WeaponOrbitCommon : 무기 궤도 클래스
/// </summary>
public class WeaponOrbitCommon : TrailOrbit
{
    [SerializeField] private Collider targetCollider;  // 추적할 오브젝트의 콜라이더

    public GameObject TargetObj => targetObj;
    public GameObject CenterObj => centerObj;
    public Collider TargetCollider => targetCollider;

    public void SetTrackType(TrackType trackType)
    {
        this.trackType = trackType;
    }


    protected override void Start()
    {
        base.Start();

        targetCollider = targetObj.GetComponent<Collider>();
    }

    public void DisableCollider()
    {
        targetCollider.enabled = false;
    }

    public void EnableCollider()
    {
        targetCollider.enabled = true;
    }

    public void SetRigidKinematic(bool isKinematic)
    {
        targetObj.GetComponent<Rigidbody>().isKinematic = isKinematic;
    }

    /// <summary>
    /// 트래킹 활성화/비활성화
    /// </summary>
    /// <param name="isTracking"></param>
    public void SetTracking(bool isTracking)
    {
        this.isTracking = isTracking;
    }

    public void SetWeaponPos(Vector3 inPos)
    {
        targetObj.transform.position = inPos;
    }
}
