using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    public TrailRenderer trailRenderer;

    public void StartTrail()
    {
        trailRenderer.enabled = true;
    }

    public void EndTrail()
    {
        trailRenderer.enabled = false;
    }
}
