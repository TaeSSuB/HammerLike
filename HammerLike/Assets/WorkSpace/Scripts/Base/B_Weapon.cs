using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Weapon : MonoBehaviour
{
    [SerializeField] private GameObject vfxObj;
    //[SerializeField] private GameObject meshObj;
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] private Material weaponTrailMat;

    public GameObject VFXObj => vfxObj;
    //public GameObject MeshObj => meshObj;
    public Renderer MeshRenderer => meshRenderer;
    public Material WeaponTrailMat => weaponTrailMat;

    public void SetVFX(bool isOn)
    {
        vfxObj.SetActive(isOn);
    }

    private void Start()
    {
        SetVFX(false);
    }
}
