using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SortConfig
{
    public string sortByProperty;
    public bool ascending = true; // true for ascending, false for descending
}

[System.Serializable]
public class FilterCondition
{
    public string propertyName;
    public enum Comparator { Greater, Less, Equal, GreaterOrEqual, LessOrEqual };
    public Comparator comparator;
    public float threshold;
}

[System.Serializable]
public class FilterConfig
{
    public List<FilterCondition> conditions = new List<FilterCondition>();
    public SortConfig sortConfig;
}