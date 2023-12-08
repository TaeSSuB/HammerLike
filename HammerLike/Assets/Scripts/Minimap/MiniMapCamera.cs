using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MiniMapCamera : MonoBehaviour
{
    private Camera cam;
    private Vector3 fixedRotation;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        // 초기 회전 값을 저장
        fixedRotation = transform.eulerAngles;
        AdjustProjectionMatrix();
    }

    void AdjustProjectionMatrix()
    {
        Matrix4x4 matrix = cam.projectionMatrix;

        // 기울기에 따른 왜곡 보정
        // 이 값들은 실험적으로 조정할 수 있습니다.
        float tiltCorrection = Mathf.Cos(30f * Mathf.Deg2Rad); // 30도 기울기
        matrix.m11 *= 0.866025404f; // y축 스케일 조정 (cos(30도))
        matrix.m22 *= 0.5f / tiltCorrection; // z축 스케일 조정 (1/2배 보정)

        cam.projectionMatrix = matrix;
    }

    void LateUpdate()
    {
        // 매 프레임마다 카메라의 회전을 초기 값으로 고정
        transform.eulerAngles = fixedRotation;
    }
}
