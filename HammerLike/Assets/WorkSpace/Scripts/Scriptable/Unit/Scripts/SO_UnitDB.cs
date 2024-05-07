using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IndexWithUnitStatus
{
    public SO_UnitStatus unitStatus;
    public int index;

    public IndexWithUnitStatus(SO_UnitStatus unitStatus, int index)
    {
        this.unitStatus = unitStatus;
        this.index = index;
    }
}

[CreateAssetMenu(fileName = "UnitDB", menuName = "B_ScriptableObjects/Unit/UnitDB", order = 1)]
public class SO_UnitDB : ScriptableObject
{
    public List<IndexWithUnitStatus> unitDataList = new List<IndexWithUnitStatus>();
}
