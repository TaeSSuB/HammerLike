using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    public SO_Item item;
    public bool isPickUpItem = false;
    public bool isPlayerNearby = false;
    public bool canPurcahse = false;
    public GameObject interactiveObj;
    public void Start()
    {
        interactiveObj = B_UIManager.Instance.CreateInteractiveWorldUI(transform);
        interactiveObj.transform.position += new Vector3(0, 3, 0);
        interactiveObj.SetActive(false);
    }

    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        GetComponentInChildren<SpriteRenderer>().sprite = item.itemIcon;
        EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
#endif
    }

    //private void OnEnable()
    //{
    //    // enable vfx
    //    B_VFXPoolManager.Instance.PlayVFX(VFXName.ItemOra, transform.position);
    //}

    private void OnDisable()
    {
        // disable vfx
        if (!B_VFXPoolManager.isQuit)
            B_VFXPoolManager.Instance?.PlayVFX(VFXName.PickUp, transform.position);

        if (!B_AudioManager.isQuit)
            B_AudioManager.Instance?.PlaySound(AudioCategory.SFX, AudioTag.PickUp);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enter triggered by: " + other.name);
            if (!isPlayerNearby) // 중복 Enter 방지
            {
                isPlayerNearby = true;
                interactiveObj.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Exit triggered by: " + other.name);
            if (isPlayerNearby) // 중복 Enter 방지
            {
                isPlayerNearby = false;
                interactiveObj.SetActive(false);
            }
        }
    }
}
