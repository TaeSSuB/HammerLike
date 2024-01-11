using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject objectToActivate; // 활성화할 오브젝트

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 현재 오브젝트 비활성화
            gameObject.SetActive(false);

            // 지정한 오브젝트 활성화
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }
        }
    }


}
