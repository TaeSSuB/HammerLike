using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SO_SystemSettings : 시스템 설정용 스크립터블 오브젝트
/// - 시스템 설정값을 저장하고, 게임 내에서 사용할 수 있도록 함
/// </summary>
[CreateAssetMenu(fileName = "SystemSettings", menuName = "B_ScriptableObjects/SystemSettings", order = 1)]
public class SO_SystemSettings : ScriptableObject
{
    /// <summary>
    /// coordinateScale : 좌표 스케일
    /// - 커스텀 좌표계를 활용하기 위한 스케일 값
    /// - 기본값 : (1, 1.15471, 2)
    /// </summary>
    [Header("System Settings")]
    [SerializeField] private Vector3 coordinateScale = new Vector3(1, 1, 1);
    [SerializeField] private float timeScale = 1f;

    [Header("Gameplay Settings")]
    [Space(10)]
    [Header("Ground Settings")]
    [SerializeField] private LayerMask groundLayer; // Define which layer is considered as ground
    [SerializeField] private float groundCheckDistance = 0.1f; // Distance to check for ground
    [Space(10)]
    [Header("KnockBack Settings")]
    [SerializeField] private ForceMode knockbackForceMode = ForceMode.Force;
    [SerializeField] private float knockbackDuration = 0.5f;
    [SerializeField] private float knockBackScale = 10f;
    private float maxKnockBackForce = 300f; // 넉백 최대 Force 값
    [SerializeField] private AnimationCurve knockbackCurve;
    [Space(10)]
    [SerializeField] private float partsKnockBackScale = 10f;
    private float maxPartsBreakForce = 100f; // 파츠 넉백 최대 Force 값
    [SerializeField] private AnimationCurve partsBreakForceCurve;
    //[SerializeField] private float gravity = 9.8f;

    [Header("Debug")]
    [SerializeField] private bool isDebugMode = false;


    #region Getters
    public Vector3 CoordinateScale { get => coordinateScale; }
    public float TimeScale { get => timeScale; }
    public float SetTimeScale { set => timeScale = value; }

    public LayerMask GroundLayer { get => groundLayer; }
    public float GroundCheckDistance { get => groundCheckDistance; }

    public ForceMode KnockbackForceMode { get => knockbackForceMode; }
    public float KnockbackDuration { get => knockbackDuration; }
    public float KnockBackScale { get => knockBackScale; }
    public float PartsKnockBackScale { get => partsKnockBackScale; }
    public float MaxKnockBackForce { get => maxKnockBackForce; }
    public float MaxPartsBreakForce { get => maxPartsBreakForce; }
    //public float Gravity { get => gravity; }
    public bool IsDebugMode { get => isDebugMode; }
    public AnimationCurve KnockbackCurve { get => knockbackCurve;}
    public AnimationCurve PartsBreakForceCurve { get => partsBreakForceCurve;}



    #endregion
}
