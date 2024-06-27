using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    public float shakeIntensity = 1.5f;
    public float shakeTime = 0.2f;

    private float timer;
    private Vector3 originalPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 게임 오브젝트가 씬 전환 시 파괴되지 않게 합니다.
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 새로운 것을 파괴합니다.
        }
    }

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
        /*if (Input.GetKeyDown(KeyCode.H))
            ShakeCamera();*/


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
