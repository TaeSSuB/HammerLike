
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f; // 플레이어의 이동 속도를 정의합니다.
    public Camera cam; // 메인 카메라를 할당하기 위한 변수입니다.

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); // 수평 방향의 입력을 받습니다.
        float vertical = Input.GetAxis("Vertical"); // 수직 방향의 입력을 받습니다.

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized; // 입력값을 바탕으로 이동 방향을 결정합니다.

        // 마우스 버튼을 누르고 있는 경우
        if (Input.GetMouseButton(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); // 마우스 위치로부터 Ray를 쏘아줍니다.
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // Ray와 물체가 충돌한 경우
            {
                Vector3 targetPosition = hit.point; // Ray가 물체와 충돌한 위치입니다.
                targetPosition.y = transform.position.y; // y 좌표를 플레이어의 y 좌표로 변경합니다.
                transform.LookAt(targetPosition); // 플레이어가 마우스 커서의 방향을 바라보게 합니다.
            }
        }

        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World); // 이동 방향과 속도를 바탕으로 플레이어를 이동시킵니다.
    }
}
