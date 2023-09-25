using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class CreateRoom
{
    public float radius = 10f;

    Vector2 getRandomPointInCircle()
    {
        return Random.insideUnitCircle;
    }
}
