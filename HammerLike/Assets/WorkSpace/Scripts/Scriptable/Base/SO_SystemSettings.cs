using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SystemSettings", menuName = "ScriptableObjects/SystemSettings", order = 1)]
public class SO_SystemSettings : ScriptableObject
{
    [SerializeField] private Vector3 coordinateScale = new Vector3(1, 1, 1);
    [SerializeField] private float timeScale = 1f;
    //[SerializeField] private float gravity = 9.8f;
    [SerializeField] private bool isDebugMode = false;

    public Vector3 CoordinateScale { get => coordinateScale; }
    public float TimeScale { get => timeScale; }
    //public float Gravity { get => gravity; }
    public bool IsDebugMode { get => isDebugMode; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
