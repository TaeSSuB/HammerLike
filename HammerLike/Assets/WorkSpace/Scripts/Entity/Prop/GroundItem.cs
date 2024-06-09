using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    public SO_Item item;
    public bool canPickUp = false;

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
}
