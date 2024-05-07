using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using BennyKok.NotionAPI;
using static BennyKok.NotionAPI.NotionAPITest;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEditor;
using System.Threading.Tasks;

public class DatabaseFromNotion : MonoBehaviour
{
    private string apiKey = "secret_bUwCv2kPZmbMpwVzdzClGoGGj837Fl9skoLNN4nMfB1";    // HammerLike API
    private string database_id = "b20a40046f3c442e82456b0bd5cd8cf1"; //Entity Database
    private string database_id2 = "2602de8a1c1d4153ad132ad206bcd45d"; //Item Database
    public bool onlyBuildable = false;
    public static readonly List<string> PropertyNames = new List<string>
    {
        "armor", "detectRange", "knockbackPower", "movementSpeed",
        "healthPoint", "attackSpeed", "attackRange", "knockbackResistance",
        "index", "attackPower", "weight"
    };
    [SerializeField] public FilterConfig filterConfig;
    private void Start()
    {
        StartCoroutine(LoadDatas());
    }

    public IEnumerator LoadDatas()
    {
        var api = new NotionAPI(apiKey);
        yield return api.QueryDatabase<EntityDatabase>(database_id, db =>
        {
            var records = db.results.AsEnumerable();

            // 조건에 맞는 모든 데이터베이스 수집
            foreach (var condition in filterConfig.conditions)
            {
                records = FilterByProperty(records, condition.propertyName, condition.threshold, condition.comparator.ToString().ToLower());
            }

            // 정렬
            if (filterConfig.sortConfig != null && !string.IsNullOrEmpty(filterConfig.sortConfig.sortByProperty))
            {
                records = SortByProperty(records, filterConfig.sortConfig.sortByProperty, filterConfig.sortConfig.ascending);
            }

            // 노션의 Checkbox인 Build 가 true인 애들만 색인
            if (onlyBuildable)
            {
                records = records.Where(record => record.properties.Build.Value);
            }

            foreach (var record in records)
            {
                //LogDatabaseRecord(record);
                Debug.Log(GetPropertyValue(record, "healthPoint"));
            }
        });
    }

    private IEnumerable<Page<EntityDatabase>> SortByProperty(IEnumerable<Page<EntityDatabase>> records, string propertyName, bool ascending)
    {
        if (ascending)
        {
            return records.OrderBy(r => GetPropertyValue(r, propertyName));
        }
        else
        {
            return records.OrderByDescending(r => GetPropertyValue(r, propertyName));
        }
    }

    private IEnumerable<Page<EntityDatabase>> FilterByProperty(IEnumerable<Page<EntityDatabase>> records, string propertyName, float threshold, string comparisonType)
    {
        return records.Where(record => EvaluateCondition(record, new FilterCondition { propertyName = propertyName, comparator = (FilterCondition.Comparator)Enum.Parse(typeof(FilterCondition.Comparator), comparisonType, true), threshold = threshold }));
    }

    

    private bool EvaluateCondition(Page<EntityDatabase> record, FilterCondition condition)
    {
        float value = GetPropertyValue(record, condition.propertyName);
        switch (condition.comparator)
        {
            case FilterCondition.Comparator.Greater:
                return value > condition.threshold;
            case FilterCondition.Comparator.GreaterOrEqual:
                return value >= condition.threshold;
            case FilterCondition.Comparator.Less:
                return value < condition.threshold;
            case FilterCondition.Comparator.LessOrEqual:
                return value <= condition.threshold;
            case FilterCondition.Comparator.Equal:
                return Math.Abs(value - condition.threshold) < float.Epsilon;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerable<Page<EntityDatabase>> SortByProperty(IEnumerable<Page<EntityDatabase>> records, string propertyName, string order)
    {
        return order == "asc" ?
            records.OrderBy(r => GetPropertyValue(r, propertyName)) :
            records.OrderByDescending(r => GetPropertyValue(r, propertyName));
    }

    private IEnumerable<Page<EntityDatabase>> FilterByEntityName(IEnumerable<Page<EntityDatabase>> records, string name)
    {
        return records.Where(r => r.properties.entityName.title.FirstOrDefault()?.text.content.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
    }


    private float GetPropertyValue(Page<EntityDatabase> record, string propertyName)
    {
        switch (propertyName)
        {
            case "armor":
                return record.properties.armor.Value;
            case "detectRange":
                return record.properties.detectRange.Value;
            case "knockbackPower":
                return record.properties.knockbackPower.Value;
            case "movementSpeed":
                return record.properties.movementSpeed.Value;
            case "healthPoint":
                return record.properties.healthPoint.Value;
            case "attackSpeed":
                return record.properties.attackSpeed.Value;
            case "attackRange":
                return record.properties.attackRange.Value;
            case "knockbackResistance":
                return record.properties.knockbackResistance.Value;
            case "index":
                return record.properties.index.Value;
            case "attackPower":
                return record.properties.attackPower.Value;
            case "weight":
                return record.properties.weight.Value;
            default:
                Debug.LogError($"Property {propertyName} not found.");
                return 0; 
        }
    }

    private void LogDatabaseRecord(Page<EntityDatabase> record)
    {
        string entityName = record.properties.entityName.title.FirstOrDefault()?.text.content ?? "No Name";
        Debug.Log($"Index: {record.properties.index.Value}, Name: {entityName}, Armor: {record.properties.armor.Value}, " +
                  $"Detect Range: {record.properties.detectRange.Value}, Knockback Power: {record.properties.knockbackPower.Value}, " +
                  $"Movement Speed: {record.properties.movementSpeed.Value}, Health Point: {record.properties.healthPoint.Value}, " +
                  $"Attack Speed: {record.properties.attackSpeed.Value}, Attack Range: {record.properties.attackRange.Value}, " +
                  $"Knockback Resistance: {record.properties.knockbackResistance.Value}, Index: {record.properties.index.Value}, " +
                  $"Attack Power: {record.properties.attackPower.Value}, Weight: {record.properties.weight.Value}");

        // ScriptableObject 생성
        SO_UnitStatus newSO = CreateAndSaveSOFromEntity(record);

        // 생성된 ScriptableObject 정보 로깅
        Debug.Log($"Created SO with Index: {newSO.index} and Name: {newSO.entityName}");
    }

    // ScriptableObject를 생성하고 데이터를 할당하는 메서드
    public SO_UnitStatus CreateAndSaveSOFromEntity(Page<EntityDatabase> record)
    {
        string assetPath = "Assets/WorkSpace/Scripts/Scriptable/Unit";

        string fullPath = $"{assetPath}/{record.properties.entityName.title.FirstOrDefault()?.text.content ?? "UnnamedEntity"}_SO.asset";

        SO_UnitStatus unitStatus = AssetDatabase.LoadAssetAtPath<SO_UnitStatus>(fullPath);

        if (unitStatus == null)
        {
            // 새 ScriptableObject 인스턴스 생성
            unitStatus = ScriptableObject.CreateInstance<SO_UnitStatus>();
            AssetDatabase.CreateAsset(unitStatus, fullPath);
            Debug.Log("Created new SO_UnitStatus");
        }
        else
        {
            Debug.Log("Updating existing SO_UnitStatus");
        }

        // EntityDatabase에서 데이터를 읽어와 SO_UnitStatus에 할당
        unitStatus.index = (int)record.properties.index.Value;
        unitStatus.entityName = record.properties.entityName.title.FirstOrDefault()?.text.content ?? "";
        unitStatus._koreanName = record.properties._koreanName.Value;
        unitStatus.mass = record.properties.weight.Value;
        unitStatus.armor = record.properties.armor.Value;
        unitStatus.detectRange = record.properties.detectRange.Value;
        unitStatus.moveSpeed = record.properties.movementSpeed.Value;
        unitStatus.maxHP = (int)record.properties.healthPoint.Value;
        unitStatus.atkDamage = (int)record.properties.attackPower.Value;
        unitStatus.atkRange = record.properties.attackRange.Value;
        unitStatus.atkSpeed = record.properties.attackSpeed.Value;
        unitStatus.knockbackPower = record.properties.knockbackPower.Value;
        unitStatus.knockbackResistance = record.properties.knockbackResistance.Value;

        // 파일 시스템에 변경사항 저장
        EditorUtility.SetDirty(unitStatus);
        AssetDatabase.SaveAssets();

        // create DB or Load DB
        SO_UnitDB unitDB = AssetDatabase.LoadAssetAtPath<SO_UnitDB>("Assets/WorkSpace/Scripts/Scriptable/Unit/Scripts/SO_UnitDB.asset");

        if (unitDB == null)
        {
            unitDB = ScriptableObject.CreateInstance<SO_UnitDB>();
            AssetDatabase.CreateAsset(unitDB, "Assets/WorkSpace/Scripts/Scriptable/Unit/Scripts/SO_UnitDB.asset");
        }

        IndexWithUnitStatus indexWithUnitStatus = new IndexWithUnitStatus(unitStatus, unitStatus.index);

        foreach (var unitData in unitDB.unitDataList)
        {
            if (unitData.index == unitStatus.index)
            {
                Debug.Log(unitStatus.entityName + " is Already Bind!!");
                return unitData.unitStatus;
            }
        }

        unitDB.unitDataList.Add(indexWithUnitStatus);
        AssetDatabase.Refresh();

        return unitStatus;
    }

}
