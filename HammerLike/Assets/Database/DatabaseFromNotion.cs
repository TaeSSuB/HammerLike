using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using BennyKok.NotionAPI;
using static BennyKok.NotionAPI.NotionAPITest;
using Unity.VisualScripting;
using System.Collections.Generic;

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
    private IEnumerator Start()
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
    }

}
