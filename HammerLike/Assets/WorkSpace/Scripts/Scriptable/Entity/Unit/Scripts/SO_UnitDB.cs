using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛 데이터베이스 인덱스, 상태 정보, 프리팹 정보
/// </summary>
[System.Serializable]
public class IndexWithUnitStatus
{
    [Header("Unit Data")]
    public SO_UnitStatus unitStatus;
    public int index;
    public GameObject unitPrefab;
    public GameObject dropItemPrefab;

    public IndexWithUnitStatus(SO_UnitStatus unitStatus, int index)
    {
        this.unitStatus = unitStatus;
        this.index = index;
    }
}

/// <summary>
/// 유닛 데이터베이스
/// - 유닛의 인덱스, 상태 정보, 프리팹 정보 담은 IndexWithUnitStatus List
/// </summary>
[CreateAssetMenu(fileName = "UnitDB", menuName = "B_ScriptableObjects/Unit/UnitDB", order = 1)]
public class SO_UnitDB : ScriptableObject
{
    public List<IndexWithUnitStatus> unitDataList = new List<IndexWithUnitStatus>();

    /// <summary>
    /// GetUnitStatus : DB 내의 유닛 상태 정보 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public SO_UnitStatus GetUnitStatus(int index)
    {
        for (int i = 0; i < unitDataList.Count; i++)
        {
            if (unitDataList[i].index == index)
            {
                return unitDataList[i].unitStatus;
            }
        }

        return null;
    }

    /// <summary>
    /// GetUnitPrefab : DB 내의 유닛 프리팹 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GetUnitPrefab(int index)
    {
        for (int i = 0; i < unitDataList.Count; i++)
        {
            if (unitDataList[i].index == index)
            {
                return unitDataList[i].unitPrefab;
            }
        }

        Debug.LogError("UnitPrefab is not found.");
        return unitDataList[0].unitPrefab;
    }

    /// <summary>
    /// GetDropItemPrefab : DB 내의 드랍 아이템 프리팹 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GetDropItemPrefab(int index)
    {
        for (int i = 0; i < unitDataList.Count; i++)
        {
            if (unitDataList[i].index == index)
            {
                return unitDataList[i].dropItemPrefab;
            }
        }

        Debug.LogError("DropItemPrefab is not found.");
        return unitDataList[0].dropItemPrefab;
    }
}
