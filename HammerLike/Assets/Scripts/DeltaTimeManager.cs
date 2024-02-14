using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaTimeManager : MonoBehaviour
{
    private float isScale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.H))
        {
            SlowDownGame();
        }
        else if(Input.GetKeyUp(KeyCode.V))
        {
            NormalizeGameSpeed();
        }
    }

    void SlowDownGame()
    {
        Time.timeScale = 0.1f;
    }

    // 게임 속도를 정상으로 복구하기
    void NormalizeGameSpeed()
    {
        Time.timeScale = 1.0f;
    }
}
