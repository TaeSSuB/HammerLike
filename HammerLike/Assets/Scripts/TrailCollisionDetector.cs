using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCollisionDetector : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    public LayerMask collisionLayer; // 충돌을 감지할 레이어
    public float checkInterval = 0.1f; // 충돌 검사 간격
    private float nextCheckTime;

    void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            CheckCollision();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    void CheckCollision()
    {
        Vector3[] positions = new Vector3[trailRenderer.positionCount];
        trailRenderer.GetPositions(positions);

        for (int i = 0; i < positions.Length - 1; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(positions[i], positions[i + 1] - positions[i], out hit, Vector3.Distance(positions[i], positions[i + 1]), collisionLayer))
            {
                Debug.Log("Trail collided with " + hit.collider.gameObject.name);
                // 여기서 충돌에 대한 처리를 진행하세요.
            }
        }
    }
}
