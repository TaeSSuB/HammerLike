using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetFPS : MonoBehaviour
{
    public float deltaTime = 0.0f;
    public Text fpsText;

    // Start is called before the first frame update
    void Start()
    {
        if (fpsText == null)
            fpsText = GameObject.Find("FPSText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        fpsText.text = string.Format("FPS: {0:0.0}ms({1:0.}fps)", msec, fps);
    }
}
