using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFallow2D : MonoBehaviour
{
    public Transform targetTr;
    public Transform camTr;

    public Vector3 offset;

    public float smoothTime = 0.1f;

    Vector2 velocity = Vector2.zero;

    void Start()
    {
        targetTr = GameObject.FindGameObjectWithTag("Player").transform;
        offset = camTr.position - targetTr.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector2 targetPos = targetTr.position + offset;

        camTr.position = Vector2.SmoothDamp(transform.position, targetTr.position, ref velocity, smoothTime);
        camTr.position = new Vector3(camTr.position.x, camTr.position.y, -10f);
    }
}
