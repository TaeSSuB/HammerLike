using UnityEngine;

public class FreezeTransform : MonoBehaviour
{
    private Vector3 fixedPosition;
    private Quaternion fixedRotation;
    private Vector3 fixedScale;

    [SerializeField] private bool freezePositionX = true;
    [SerializeField] private bool freezePositionY = true;
    [SerializeField] private bool freezePositionZ = true;

    [SerializeField] private bool freezeRotationX = true;
    [SerializeField] private bool freezeRotationY = true;
    [SerializeField] private bool freezeRotationZ = true;

    [SerializeField] private bool freezeScaleX = true;
    [SerializeField] private bool freezeScaleY = true;
    [SerializeField] private bool freezeScaleZ = true;

    void Start()
    {
        // 초기 위치와 회전 저장
        fixedPosition = transform.position;
        fixedRotation = transform.rotation;
        fixedScale = transform.localScale;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;
        Vector3 currentScale = transform.localScale;

        // 위치 고정
        transform.position = new Vector3(
            freezePositionX ? fixedPosition.x : currentPosition.x,
            freezePositionY ? fixedPosition.y : currentPosition.y,
            freezePositionZ ? fixedPosition.z : currentPosition.z
        );

        // 회전 고정
        transform.rotation = Quaternion.Euler(
            freezeRotationX ? fixedRotation.eulerAngles.x : currentRotation.eulerAngles.x,
            freezeRotationY ? fixedRotation.eulerAngles.y : currentRotation.eulerAngles.y,
            freezeRotationZ ? fixedRotation.eulerAngles.z : currentRotation.eulerAngles.z
        );

        // 스케일 고정
        transform.localScale = new Vector3(
            freezeScaleX ? fixedScale.x : currentScale.x,
            freezeScaleY ? fixedScale.y : currentScale.y,
            freezeScaleZ ? fixedScale.z : currentScale.z
        );
    }
}
