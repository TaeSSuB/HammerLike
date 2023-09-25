using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoom : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col2d)
    {
        if (col2d.CompareTag("SpawnPoint"))
            Destroy(col2d.gameObject);
    }
}
