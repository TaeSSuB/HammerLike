using UnityEngine;
using UnityEngine.UI;

public class DisplayFrameRate : MonoBehaviour
{
    public Text frameRateText;

    void Update()
    {
        float currentFrameRate = 1.0f / Time.deltaTime;
        frameRateText.text = "FPS: " + currentFrameRate.ToString("F1");
    }
}