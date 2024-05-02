using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestCutting : MonoBehaviour
{
    [SerializeField]
    GrassComputeScript grassComputeScript;

    [SerializeField]
    float radius = 1f;

    public bool updateCuts;
    Vector3 cachedPos;
    // Start is called before the first frame update

    private void Start()
    {
              
    }

    // Update is called once per frame
    void Update()
    {
        if (updateCuts && transform.position != cachedPos&& Input.GetMouseButton(0))
        {

            grassComputeScript.UpdateCutBuffer(transform.position, radius);
            cachedPos = transform.position;

        }

        if(Input.GetMouseButtonDown(1))
        {
            Vector3 currentPosition = transform.position;

            // y좌표를 0.5만큼 증가시킵니다.
            currentPosition.y += 0.5f;

            // 변경된 위치를 오브젝트에 적용합니다.
            transform.position = currentPosition;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
