using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLineCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.name == "Interactive Modules")
        {
            string entranceName = other.name;
            if (entranceName == "Entrance_N" || entranceName == "Entrance_E" ||
                entranceName == "Entrance_W" || entranceName == "Entrance_S")
            {
                other.gameObject.SetActive(false);
            }
        }
    }
}
