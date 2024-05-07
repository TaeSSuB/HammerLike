using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// 아이템 데이터 베이스 오브젝트 클래스
/// Scriptable로 제작되어 정적 데이터 관리에 용이, a.HG
/// </summary>
[CreateAssetMenu(fileName = "New Item Database", menuName = "B_ScriptableObjects/Database")]
public class SO_ItemDataBase : ScriptableObject, ISerializationCallbackReceiver
{
    public SO_Item[] Items;
    public Dictionary<int, SO_Item> GetItem = new Dictionary<int, SO_Item>();

    // 오브젝트 생성 및 수정 이후 호출
    public void OnAfterDeserialize()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            //Items[i].data.Id = i;
            Items[i].itemData.index = i;
            GetItem.Add(i, Items[i]);
        }
    }

    // 오브젝트 수정 이전, 도중 호출
    public void OnBeforeSerialize()
    {
        GetItem = new Dictionary<int, SO_Item>();
    }
}
