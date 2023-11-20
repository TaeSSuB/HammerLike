using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DeformOnCollision : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] currentVertices;

    private void Start()
    {
        Debug.Log("Start method called");
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
        currentVertices = mesh.vertices;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌 포인트에 가장 가까운 버텍스를 찾아 변형
        foreach (ContactPoint contact in collision.contacts)
        {
            for (int i = 0; i < originalVertices.Length; i++)
            {
                float distance = Vector3.Distance(transform.TransformPoint(currentVertices[i]), contact.point);
                if (distance < 0.1f)  // 이 값은 변형의 반경을 결정
                {
                    Vector3 direction = currentVertices[i] - transform.InverseTransformPoint(contact.point);
                    currentVertices[i] -= direction * 0.5f;  // 0.1f는 변형의 강도
                }
            }
        }
        Debug.Log("충돌했음");
        mesh.vertices = currentVertices;
        mesh.RecalculateNormals();  // 노멀 재계산
    }
}
