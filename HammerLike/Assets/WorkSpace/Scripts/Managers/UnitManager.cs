using NuelLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UnitManager : 유닛 생성 및 관리 클래스 (Singleton MonoBehavior)
/// - NotionAPI를 통해 생성된 경로 상의 유닛 DataBase를 할당 및 활용
/// - DB 내의 유닛 정보를 통해 유닛 생성
///     - DB 유닛 정보 - 프리팹 정보, 상태 정보
///     - 프리팹 내 UnitBase 컴포넌트 체크 후 데이터 할당 및 인스턴스 생성
/// - 유닛 리스트 추가, 관리
/// </summary>
public class UnitManager : SingletonMonoBehaviour<UnitManager>
{
    public List<B_UnitBase> unitList = new List<B_UnitBase>();

    public string pathDB = "Assets/WorkSpace/Scripts/Scriptable/Unit/Scripts/SO_UnitDB.asset";
    public SO_UnitDB unitDataBase;

    /// <summary>
    /// Awake : DB 할당
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

    #if UNITY_EDITOR
        if (unitDataBase == null)
        {
            unitDataBase = (SO_UnitDB)UnityEditor.AssetDatabase.LoadAssetAtPath(pathDB, typeof(SO_UnitDB));
        }
    #endif
    }

    /// <summary>
    /// GetUnitStatus : DB 내의 유닛 상태 정보 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public SO_UnitStatus GetUnitStatus(int index)
    {
        return unitDataBase.GetUnitStatus(index);
    }

    /// <summary>
    /// GetUnitPrefab : DB 내의 유닛 프리팹 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GetUnitPrefab(int index)
    {
        return unitDataBase.GetUnitPrefab(index);
    }

    /// <summary>
    /// CreateUnit : 유닛 생성 및 데이터 바인딩
    /// </summary>
    /// <param name="index"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject CreateUnit(int index, Vector3 position, Quaternion rotation)
    {
        // Get Unit Prefab
        GameObject unitPrefab = GetUnitPrefab(index);
        
        B_UnitBase unit = unitPrefab.GetComponent<B_UnitBase>();
        
        if(unit == null)
        {
            // try get child
            unit = unitPrefab.GetComponentInChildren<B_UnitBase>();
            
            if(unit == null)
            {
                Debug.LogError("unitObj is not B_UnitBase or B_UnitBase is not in child.");
                return null;
            }
        }

        unit.SetUnitIndex = index;

        // Create Unit
        GameObject unitObj = Instantiate(unitPrefab, position, rotation);
        unit = unitObj.GetComponentInChildren<B_UnitBase>();

        // Unit List Add
        AddUnit(unit);

        return unitObj;
    }

    /// <summary>
    /// AddUnit : 유닛 리스트에 유닛 추가
    /// </summary>
    /// <param name="unit"></param>
    public void AddUnit(B_UnitBase unit)
    {
        // if B_UnitBase  is not B_player, AddUnit
        if(unit.GetType() != typeof(B_Player))
        {
            unitList.Add(unit);
        }
    }

    /// <summary>
    /// RemoveUnit : 유닛 리스트에서 유닛 제거
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnit(B_UnitBase unit)
    {
        unitList.Remove(unit);
    }
    
    /// <summary>
    /// RemoveAllUnit : 유닛 리스트 초기화
    /// </summary>
    public void RemoveAllUnit()
    {
        unitList.Clear();
    }

    /// <summary>
    /// SetAllUnitActive : 유닛 리스트 전체 활성화 여부 설정
    /// </summary>
    /// <param name="isActive"></param>
    public void SetAllUnitActive(bool isActive)
    {
        foreach (var unit in unitList)
        {
            unit.gameObject.SetActive(isActive);
        }
    }
}
