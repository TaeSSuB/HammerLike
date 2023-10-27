using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAim : MonoBehaviour
{
    Monster monster;
    Transform target; // This is who the monster wants to aim at, typically the player

    private void Awake()
    {
        monster = GetComponent<Monster>();
    }

    public Vector3 Aiming()
    {
        if (target == null)
        {
            Debug.LogWarning("No target set for monster.");
            return Vector3.zero;
        }
        Vector3 aimDirection = (target.position - transform.position).normalized;
        return aimDirection;
    }

    // AI or some other script sets the target, e.g., the player
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public Transform CurrentTarget
    {
        get { return target; }
    }
}
