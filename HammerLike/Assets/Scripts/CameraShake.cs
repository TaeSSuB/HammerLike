using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeIntensity = 1.5f;
    public float shakeTime = 0.2f;

    private float timer;
    private Vector3 originalPosition;

    private void Start()
    {
        timer = 0f;
    }

    public void ShakeCamera()
    {
        originalPosition = transform.localPosition;
        timer = shakeTime;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            ShakeCamera();


        if (timer > 0)
        {
            timer -= Time.deltaTime;

            // Randomly shake the camera within a certain range
            float offsetX = Random.Range(-1f, 1f) * shakeIntensity;
            float offsetY = Random.Range(-1f, 1f) * shakeIntensity;

            transform.localPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

            // Reset the position after shaking is done
            if (timer <= 0)
            {
                transform.localPosition = originalPosition;
            }
        }
    }
}
