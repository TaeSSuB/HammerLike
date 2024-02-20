using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrail : MonoBehaviour
{
    private TrailRenderer trailRenderer;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        //trailRenderer.enabled = false;
    }

    public void EnableTrail()
    {
        trailRenderer.enabled = true;
        trailRenderer.emitting = true;
    }

    public void DisableTrail()
    {
        trailRenderer.emitting = false;
    }
}
