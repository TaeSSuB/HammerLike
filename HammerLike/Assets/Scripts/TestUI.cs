using UnityEngine;

public class TestUI : MonoBehaviour
{
    public GameObject targetObject; // SetActive를 반전시킬 대상 오브젝트

    // Update is called once per frame
    void Update()
    {
        // ESC 키가 눌렸는지 확인
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // targetObject의 활성 상태를 반전
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}
