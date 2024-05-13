using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadObject : MonoBehaviour
{

    [SerializeField] private SceneLoader sceneLoader;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            sceneLoader.ChangeScene("Test_RoomSetting");
        }
    }
}
