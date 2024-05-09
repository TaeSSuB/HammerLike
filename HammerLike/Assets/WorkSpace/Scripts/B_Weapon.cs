using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Weapon : MonoBehaviour
{
    [SerializeField] private GameObject vfxObj;
    [SerializeField] private GameObject meshObj;

    public GameObject VFXObj => vfxObj;
    public GameObject MeshObj => meshObj;

    public void SetVFX(bool isOn)
    {
        vfxObj.SetActive(isOn);
    }

    private void Start()
    {
        SetVFX(false);
    }
}
