using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomGen;

public class RuntimeGenerator : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RegenerateRoom();
    }


    void RegenerateRoom()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            EventSystem.instance.SetGridSize(Random.Range(10, 20), Random.Range(10, 20));
            EventSystem.instance.SetRoomSeed(levelNumber: 0, Random.Range(0, 99999));
            EventSystem.instance.SetDecorSeed(0, Random.Range(0, 99999));
            EventSystem.instance.SetCharacterSeed(0, Random.Range(0, 99999));
            EventSystem.instance.SetDoorCount(0, Random.Range(2, 8));
            EventSystem.instance.SetWindowCount(0, Random.Range(5, 10));
            EventSystem.instance.SetLevelOffset(0, new Vector3(0, 5, 0));
            EventSystem.instance.SetLevelHeight(0, Random.Range(1, 4));
            EventSystem.instance.Generate();
        }
    }
}
