using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    public SO_Item item;

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
        B_VFXPoolManager.Instance.PlayVFX(VFXName.PickUp, transform.position);
        B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.PickUp);
    }
}
