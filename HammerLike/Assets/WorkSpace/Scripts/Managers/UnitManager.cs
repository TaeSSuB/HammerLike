using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// singleton class
public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

    public List<B_UnitBase> unitList = new List<B_UnitBase>();

    // only SO_UnitStatus
    public SO_UnitStatus baseUnitStatus;
    public SO_RangerUnitStatus rangerUnitStatus;
    public SO_UnitStatus devUnitStatus;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddUnit(B_UnitBase unit)
    {
        // if B_UnitBase  is not B_player, AddUnit
        if(unit.GetType() != typeof(B_Player))
        {
            unitList.Add(unit);
        }
    }

    public void RemoveUnit(B_UnitBase unit)
    {
        unitList.Remove(unit);
    }

    public void RemoveAllUnit()
    {
        unitList.Clear();
    }

    public B_UnitBase GetUnit(int index)
    {
        return unitList[index];
    }

    public int GetUnitCount()
    {
        return unitList.Count;
    }
}
