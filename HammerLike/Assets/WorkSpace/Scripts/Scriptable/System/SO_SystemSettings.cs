using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SystemSettings", menuName = "B_ScriptableObjects/SystemSettings", order = 1)]
public class SO_SystemSettings : ScriptableObject
{
    [Header("System Settings")]
    [SerializeField] private Vector3 coordinateScale = new Vector3(1, 1, 1);
    [SerializeField] private float timeScale = 1f;

    [Header("Gameplay Settings")]
    [SerializeField] private float knockBackScale = 10f;
    [SerializeField] private float maxKnockBackForce = 100f;
    [SerializeField] private float maxPartsBreakForce = 100f;
    [SerializeField] private AnimationCurve knockbackCurve;
    [SerializeField] private AnimationCurve partsBreakForceCurve;
    //[SerializeField] private float gravity = 9.8f;

    [Header("Debug")]
    [SerializeField] private bool isDebugMode = false;

    public Vector3 CoordinateScale { get => coordinateScale; }
    public float TimeScale { get => timeScale; }
    public float SetTimeScale { set => timeScale = value; }

    public float KnockBackScale { get => knockBackScale; }
    public float MaxKnockBackForce { get => maxKnockBackForce; }
    public float MaxPartsBreakForce { get => maxPartsBreakForce; }
    //public float Gravity { get => gravity; }
    public bool IsDebugMode { get => isDebugMode; }
    public AnimationCurve KnockbackCurve { get => knockbackCurve;}
    public AnimationCurve PartsBreakForceCurve { get => partsBreakForceCurve;}
}
