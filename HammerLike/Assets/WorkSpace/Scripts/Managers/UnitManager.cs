using NuelLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// singleton class
public class UnitManager : SingletonMonoBehaviour<UnitManager>
{
    public List<B_UnitBase> unitList = new List<B_UnitBase>();

    public string pathDB = "Assets/WorkSpace/Scripts/Scriptable/Unit/Scripts/SO_UnitDB.asset";
    public SO_UnitDB unitDataBase;

    // Temp 20240425 - UnitStatus DB 생성 및 관리 예정, a.HG
    //public SO_UnitStatus baseUnitStatus;
    //public SO_UnitStatus slimeStatus;
    //public SO_RangerUnitStatus rangerUnitStatus;
    //public SO_UnitStatus devUnitStatus;

    protected override void Awake()
    {
        base.Awake();

        if (unitDataBase == null)
        {
            unitDataBase = (SO_UnitDB)UnityEditor.AssetDatabase.LoadAssetAtPath(pathDB, typeof(SO_UnitDB));
        }
    }

    // Get Status
    public SO_UnitStatus GetUnitStatus(int index)
    {
        foreach (var unitData in unitDataBase.unitDataList)
        {
            if (unitData.index == index)
            {
                return unitData.unitStatus;
            }
        }

        return null;
    }

    public void AddUnit(B_UnitBase unit)
    {
        // if B_UnitBase  is not B_player, AddUnit
        if(unit.GetType() != typeof(B_Player))
        {
            unitList.Add(unit);
        }
    }
}
